using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


//월드 전체를 관리하는 컴포넌트
//플레이어 위치에 따라 청크 생성을 관리
//백그라운드 스레드에서 청크 데이터 생성 후 메인 스레드에 반영
public class World : MonoBehaviour
{   
    public Material material; // 텍스쳐 아틀라스
    public BlockType[] blockTypes; // 블록타입 정의 (solid, texture 등)
    public BiomeData biome; // 바이옴 데이터 (지형, 광물 등)

    private Dictionary<ChunkCoord, Chunk> chunks = new Dictionary<ChunkCoord, Chunk>(); // 생성된 청크를 관리하는 딕셔너리 맵

    public Transform player;
    public Vector3 spawnPosition;

    List<ChunkCoord> currentActiveChunkList = new List<ChunkCoord>(); //현재 활성화된 청크 좌표 목록

    // 플레이어의 위치 정보
    private (int, int) prevPlayerCoord;
    private (int, int) currentPlayerCoord;

    public int seed;

    #region Thread
    private readonly Queue<Action> mainThreadActions = new Queue<Action>();
    public void EnqueueMainThreadAction(Action action)
    {

        lock (mainThreadActions)
        {
            mainThreadActions.Enqueue(action);
        }
    }

    // 청크의 메시데이터 생성을 요청
    public void RequestChunkGeneration(ChunkCoord coord)
    {

        ThreadPool.QueueUserWorkItem((_) =>
        {
            byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
            for (int y = 0; y < VoxelData.ChunkHeight; y++)
            {
                for (int x = 0; x < VoxelData.ChunkWidth; x++)
                {
                    for (int z = 0; z < VoxelData.ChunkWidth; z++)
                    {
                        Vector3 worldPos = new Vector3(x + coord.x * VoxelData.ChunkWidth, y, z + coord.z * VoxelData.ChunkWidth);
                        voxelMap[x, y, z] = GetBlockType(worldPos);
                    }
                }
            }
            Vector3 chunkOrig = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
            ChunkMeshData meshData = ChunkMeshBuilder.Build(voxelMap, this, chunkOrig);

            EnqueueMainThreadAction(() =>
            {
                Chunk chunk = new Chunk(coord, this, voxelMap, meshData);
                chunks[coord] = chunk; 
                chunk.Init();
            });
        });
    }
    #endregion

    private void Start()
    {
        UnityEngine.Random.InitState(seed); // 시드값 초기화
        InitPositions();
    }

    private void Update()
    {
        currentPlayerCoord = GetChunkCoordFromWorldPos(player.position);
        lock (mainThreadActions)
        {
            while (mainThreadActions.Count > 0)
            {
                var action = mainThreadActions.Dequeue();
                action?.Invoke();
            }
        }
        // 플레이어가 청크 위치를 이동한 경우, 시야 범위 갱신
        if (!currentPlayerCoord.Equals(prevPlayerCoord))
        {

            UpdateChunksInViewRange();
        }

    }

    //블록 타입 정의
    public byte GetBlockType(in Vector3 worldPos)
    {
 
        int yPos = (int)worldPos.y;
        byte blockType = 0; // 기본 블록 타입 (공기)

        /* -----------------------------------------------
                            Immutable Pass
        ----------------------------------------------- */
        // 월드 밖 : 공기
        if (!IsBlockInWorld(worldPos))
            return 0;

        // 높이 0은 기반암
        if (yPos == 0)
            return 1;

        /* -----------------------------------------------
                        Basic Terrain Pass
        ----------------------------------------------- */

        float noise = Noise.Get2DPerlin(new Vector2(worldPos.x, worldPos.z), 0f, biome.terrainScale); //오프셋 값에 seed 사용 가능
        float terrainHeight = (int)(biome.terrainHeightRange * noise) + biome.solidGroindHeight;

        // teerainHeight 를 기준으로 결정
        if (yPos > terrainHeight)
        {
            return 0;
        }

        // 지면
        if (yPos == terrainHeight)
        {
            return 2;
        }
        // 땅속
        else if (terrainHeight - 4 < yPos && yPos < terrainHeight)
        {
            return 3;
        }
        else
        {
            blockType = 5;

        }
        /* --------------------------------------------- *
        *              Second Terrain Pass              *
        * --------------------------------------------- */

        //광물 생성
        if (blockType == 5)
        {
            foreach (var lode in biome.lodes)
            {
                if (lode.minHeight < yPos && yPos < lode.maxHeight)
                {
                    if (Noise.Get3DPerlin(worldPos, lode.noiseOffset, lode.scale, lode.threshold))
                    {
                        blockType = lode.blockID;
                    }
                }
            }
        }
        return blockType;

    }

    private bool IsBlockInWorld(Vector3 pos)
    {
        return pos.y >= 0 && pos.y < VoxelData.ChunkHeight;
    }

    //해당 위치의 블록이 공기층인지 검사
    public bool IsBlockSolid(in Vector3 worldPos)
    {
        return blockTypes[GetBlockType(worldPos)].isSolid;
    }

    private (int, int) GetChunkCoordFromWorldPos(in Vector3 worldPos)
    {
        int x = (int)(worldPos.x / VoxelData.ChunkWidth);
        int z = (int)(worldPos.z / VoxelData.ChunkWidth);
        return (x, z);
    }

    private void InitPositions()
    {
        spawnPosition = new Vector3(0.5f, VoxelData.ChunkHeight, 0.5f);
        player.position = spawnPosition;

        prevPlayerCoord = (-1, -1);
        currentPlayerCoord = GetChunkCoordFromWorldPos(player.position);
    }


    // 시야범위 내의 청크 생성
    private void UpdateChunksInViewRange()
    {
        var location = GetChunkCoordFromWorldPos(player.position);
        ChunkCoord centerCoord = new ChunkCoord(location.Item1, location.Item2);
        prevPlayerCoord = currentPlayerCoord; // 기준 좌표 갱신
        int viewDist = VoxelData.ViewDistanceInChunks;


        // 활성 목록 : 현재 -> 이전으로 이동
        List<ChunkCoord> prevActiveChunkList = new List<ChunkCoord>(currentActiveChunkList);
        currentActiveChunkList.Clear();

        for (int x = centerCoord.x - viewDist; x < centerCoord.x + viewDist; x++)
        {
            for (int z = centerCoord.z - viewDist; z < centerCoord.z + viewDist; z++)
            {
                ChunkCoord coord = new ChunkCoord(x, z);


                // 시야 범위 내에 청크가 생성되지 않은 영역이 있을 경우, 새로 생성
                if (!chunks.ContainsKey(coord))
                {
                    chunks.Add(coord, null);
                    RequestChunkGeneration(coord); // 스레드에서 청크 생성 요청
                }
                else if (chunks[coord] != null && !chunks[coord].IsActive)
                {
                    chunks[coord].IsActive = true;
                }

                currentActiveChunkList.Add(coord);

                for (int i = 0; i < prevActiveChunkList.Count; i++)
                {

                    if (prevActiveChunkList[i].Equals(coord))
                        prevActiveChunkList.RemoveAt(i);

                }
            }

        }
        foreach (ChunkCoord c in prevActiveChunkList)
        {
            if (chunks.TryGetValue(c, out Chunk chunk))
            {
                if (chunk != null)
                    chunk.IsActive = false; 

            }

        }
    }
}


