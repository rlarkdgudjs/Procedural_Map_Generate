using UnityEngine;

public static class VoxelData
{
    // ûũ ���� X, Y, Z ���� ���� ����
    public static readonly int ChunkWidth = 32;
    public static readonly int ChunkHeight = 128;
    // �� �ε� ûũ���� �þ߰Ÿ�
    public static readonly int ViewDistanceInChunks = 5;

    // �ؽ��� ��Ʋ���� ����, ���� �ؽ��� ����
    public static readonly int TextureAtlasWidth = 9;
    public static readonly int TextureAtlasHeight = 10;

    // �ؽ��� ��Ʋ�� ������ �� ��, ������ �ؽ��İ� ���� ũ�� ����
    public static float NormalizedTextureAtlasWidth
        => 1f / TextureAtlasWidth;
    public static float NormalizedTextureAtlasHeight
        => 1f / TextureAtlasHeight;

    public enum VoxelFace
    {
        Back = 0,
        Front = 1,
        Top = 2,
        Bottom = 3,
        Left = 4,
        Right = 5
    }

    //ť���� 8�� ���ؽ��� ��� ��ġ
    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {
            // Front
            new Vector3(0.0f, 0.0f, 0.0f), // LB
            new Vector3(1.0f, 0.0f, 0.0f), // RB
            new Vector3(1.0f, 1.0f, 0.0f), // RT
            new Vector3(0.0f, 1.0f, 0.0f), // LT

            // Back
            new Vector3(0.0f, 0.0f, 1.0f), // LB
            new Vector3(1.0f, 0.0f, 1.0f), // RB
            new Vector3(1.0f, 1.0f, 1.0f), // RT
            new Vector3(0.0f, 1.0f, 1.0f), // LT
    };

    //  ť���� �� ���� �̷�� �ﰢ������ ���ؽ� �ε��� ������
    public static readonly int[,] voxelTris = new int[6, 4]
    {

            {0, 3, 1, 2 }, // Back Face   (-Z)
            {5, 6, 4, 7 }, // Front Face  (+Z)
            {3, 7, 2, 6 }, // Top Face    (+Y)
            {1, 5, 0, 4 }, // Bottom Face (-Y)
            {4, 7, 0, 3 }, // Left Face   (-X)
            {1, 2, 5, 6 }, // RIght Face  (+X)
    };

    // voxelTris�� ���ؽ� �ε��� ������ ���� ���ǵ� UV ��ǥ ������
    public static readonly Vector2[] voxelUvs = new Vector2[4]
    {
            new Vector2(0.0f, 0.0f), // LB
            new Vector2(0.0f, 1.0f), // LT
            new Vector2(1.0f, 0.0f), // RB
            new Vector2(1.0f, 1.0f), // RT
    };

    public static readonly Vector3[] faceChecks = new Vector3[6]
{
        new Vector3( 0.0f,  0.0f, -1.0f), // Back Face   (-Z)
        new Vector3( 0.0f,  0.0f, +1.0f), // Front Face  (+Z)
        new Vector3( 0.0f, +1.0f,  0.0f), // Top Face    (+Y)
        new Vector3( 0.0f, -1.0f,  0.0f), // Bottom Face (-Y)
        new Vector3(-1.0f,  0.0f,  0.0f), // Left Face   (-X)
        new Vector3(+1.0f,  0.0f,  0.0f), // RIght Face  (+X)
};
  
}
