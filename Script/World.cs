using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


//���� ��ü�� �����ϴ� ������Ʈ
//�÷��̾� ��ġ�� ���� ûũ ������ ����
//��׶��� �����忡�� ûũ ������ ���� �� ���� �����忡 �ݿ�
public class World : MonoBehaviour
{   
    public Material material; // �ؽ��� ��Ʋ��
    public BlockType[] blockTypes; // ���Ÿ�� ���� (solid, texture ��)
    public BiomeData biome; // ���̿� ������ (����, ���� ��)

    private Dictionary<ChunkCoord, Chunk> chunks = new Dictionary<ChunkCoord, Chunk>(); // ������ ûũ�� �����ϴ� ��ųʸ� ��

    public Transform player;
    public Vector3 spawnPosition;

    List<ChunkCoord> currentActiveChunkList = new List<ChunkCoord>(); //���� Ȱ��ȭ�� ûũ ��ǥ ���

    // �÷��̾��� ��ġ ����
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

    // ûũ�� �޽õ����� ������ ��û
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
        UnityEngine.Random.InitState(seed); // �õ尪 �ʱ�ȭ
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
        // �÷��̾ ûũ ��ġ�� �̵��� ���, �þ� ���� ����
        if (!currentPlayerCoord.Equals(prevPlayerCoord))
        {

            UpdateChunksInViewRange();
        }

    }

    //��� Ÿ�� ����
    public byte GetBlockType(in Vector3 worldPos)
    {
 
        int yPos = (int)worldPos.y;
        byte blockType = 0; // �⺻ ��� Ÿ�� (����)

        /* -----------------------------------------------
                            Immutable Pass
        ----------------------------------------------- */
        // ���� �� : ����
        if (!IsBlockInWorld(worldPos))
            return 0;

        // ���� 0�� ��ݾ�
        if (yPos == 0)
            return 1;

        /* -----------------------------------------------
                        Basic Terrain Pass
        ----------------------------------------------- */

        float noise = Noise.Get2DPerlin(new Vector2(worldPos.x, worldPos.z), 0f, biome.terrainScale);
        float terrainHeight = (int)(biome.terrainHeightRange * noise) + biome.solidGroindHeight;

        // teerainHeight �� �������� ����
        if (yPos > terrainHeight)
        {
            return 0;
        }

        // ����
        if (yPos == terrainHeight)
        {
            return 2;
        }
        // ����
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

        //���� ����
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

    //�ش� ��ġ�� ����� ���������� �˻�
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


    // �þ߹��� ���� ûũ ����
    private void UpdateChunksInViewRange()
    {
        var location = GetChunkCoordFromWorldPos(player.position);
        ChunkCoord centerCoord = new ChunkCoord(location.Item1, location.Item2);
        prevPlayerCoord = currentPlayerCoord; // ���� ��ǥ ����
        int viewDist = VoxelData.ViewDistanceInChunks;


        // Ȱ�� ��� : ���� -> �������� �̵�
        List<ChunkCoord> prevActiveChunkList = new List<ChunkCoord>(currentActiveChunkList);
        currentActiveChunkList.Clear();

        for (int x = centerCoord.x - viewDist; x < centerCoord.x + viewDist; x++)
        {
            for (int z = centerCoord.z - viewDist; z < centerCoord.z + viewDist; z++)
            {
                ChunkCoord coord = new ChunkCoord(x, z);


                // �þ� ���� ���� ûũ�� �������� ���� ������ ���� ���, ���� ����
                if (!chunks.ContainsKey(coord))
                {
                    chunks.Add(coord, null);
                    RequestChunkGeneration(coord); // �����忡�� ûũ ���� ��û
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


