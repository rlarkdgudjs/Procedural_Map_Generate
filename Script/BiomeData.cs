using UnityEngine;

[CreateAssetMenu(fileName = "BiomeData", menuName = "Scriptable Objects/Voxel System/BiomeData")]
public class BiomeData : ScriptableObject
{
    public string biomeName; // �������� �̸�
    public int solidGroindHeight; // ���� ����
    public int terrainHeightRange; // ������̷� ���� �����Ҽ��ִ� �ִ� ���̰�
    public float terrainScale;

    public Lode[] lodes;
}
[System.Serializable]
public class Lode
{
    public string loadName;
    public byte blockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold; 
    public float noiseOffset;
}
