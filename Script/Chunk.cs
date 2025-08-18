using System;
using UnityEngine;

//청크의 좌표를 나타내는 구조체 X,Z 로 각 청크를 구분
public struct ChunkCoord : IEquatable<ChunkCoord>
{
    public int x;
    public int z;

    public ChunkCoord(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }
    public override bool Equals(object obj)
    {
        if (obj is ChunkCoord other)
            return Equals(other);
        return false;
    }
    public bool Equals(ChunkCoord other)
    {
        return this.x == other.x && this.z == other.z;
    }
}

//하나의 청크를 나타내는 클래스 실제 청크 내부생성을 관리
public class Chunk 
{
    private GameObject chunkObject; // 청크가 생성될 게임오브젝트
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public ChunkCoord coord; // 청크의 좌표

    private byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth]; // 청크내의 블록 타입을 담는 배열
    private ChunkMeshData meshData;

    private bool _isActive; // 청크가 활성화 상태인지 여부
    private World world;

    public bool IsActive
    {
        get { return _isActive; }
        set
        {
            _isActive = value;
            if (chunkObject != null)
                chunkObject.SetActive(value);
        }
    }
    public Chunk(ChunkCoord coord, World world, byte[,,] voxelMap, ChunkMeshData meshData)
    {
        this.coord = coord;
        this.world = world;
        this.voxelMap = voxelMap;
        this.meshData = meshData;
    }

    public void Init()
    {
        chunkObject = new GameObject();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer.material = this.world.material;

        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position =
        new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = $"Chunk [{coord.x}, {coord.z}]";

        CreateMeshByMeshData();
    }

    //전달받은 meshData를 이용해 메시를 생성
    void CreateMeshByMeshData()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.vertices.ToArray();
        mesh.triangles = meshData.triangles.ToArray();
        mesh.uv = meshData.uvs.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

    }

}
