using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Chunk2 : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    private bool[,,] voxelMap =
  new bool[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    private void Start()
    {
        
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();

    }
    private void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    voxelMap[x, y, z] = true;
                }
            }
        }
    }
    private void CreateMeshData()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
    }
    void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

    }

    private void AddVoxelDataToChunk(Vector3 pos)
    {
        // 6������ �� �׸���
        // p : -Z, +Z, +Y, -Y, -X, +X ������ �̷����, ť���� �� �鿡 ���� �ε���
        for (int p = 0; p < 6; p++)
        {
            if (!voxelMap[(int)pos.x, (int)pos.y, (int)pos.z])
                continue;

            // �� ��(�ﰢ�� 2��) �׸���

            // 1. Vertex, UV 4�� �߰�
            for (int i = 0; i <= 3; i++)
            {
                vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[p, i]] + pos);
                uvs.Add(VoxelData.voxelUvs[i]);
            }

            // 2. Triangle�� ���ؽ� �ε��� 6�� �߰�
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);

            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 3);

            vertexIndex += 4;
        }
    }
}
