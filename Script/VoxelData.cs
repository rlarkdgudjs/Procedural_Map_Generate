using UnityEngine;

public static class VoxelData
{
    // 청크 내의 X, Y, Z 성분 복셀 개수
    public static readonly int ChunkWidth = 32;
    public static readonly int ChunkHeight = 128;
    // 맵 로드 청크단위 시야거리
    public static readonly int ViewDistanceInChunks = 5;

    // 텍스쳐 아틀라스의 가로, 세로 텍스쳐 개수
    public static readonly int TextureAtlasWidth = 9;
    public static readonly int TextureAtlasHeight = 10;

    // 텍스쳐 아틀라스 내에서 각 행, 열마다 텍스쳐가 갖는 크기 비율
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

    //큐브의 8개 버텍스의 상대 위치
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

    //  큐브의 각 면을 이루는 삼각형들의 버텍스 인덱스 데이터
    public static readonly int[,] voxelTris = new int[6, 4]
    {

            {0, 3, 1, 2 }, // Back Face   (-Z)
            {5, 6, 4, 7 }, // Front Face  (+Z)
            {3, 7, 2, 6 }, // Top Face    (+Y)
            {1, 5, 0, 4 }, // Bottom Face (-Y)
            {4, 7, 0, 3 }, // Left Face   (-X)
            {1, 2, 5, 6 }, // RIght Face  (+X)
    };

    // voxelTris의 버텍스 인덱스 순서에 따라 정의된 UV 좌표 데이터
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
