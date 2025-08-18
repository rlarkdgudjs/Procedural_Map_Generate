using System;
using UnityEngine;

//ûũ�� ��ǥ�� ��Ÿ���� ����ü X,Z �� �� ûũ�� ����
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

//�ϳ��� ûũ�� ��Ÿ���� Ŭ���� ���� ûũ ���λ����� ����
public class Chunk 
{
    private GameObject chunkObject; // ûũ�� ������ ���ӿ�����Ʈ
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public ChunkCoord coord; // ûũ�� ��ǥ

    private byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth]; // ûũ���� ��� Ÿ���� ��� �迭
    private ChunkMeshData meshData;

    private bool _isActive; // ûũ�� Ȱ��ȭ �������� ����
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

    //���޹��� meshData�� �̿��� �޽ø� ����
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
