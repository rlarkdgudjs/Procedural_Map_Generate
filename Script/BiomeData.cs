using UnityEngine;

[CreateAssetMenu(fileName = "BiomeData", menuName = "Scriptable Objects/Voxel System/BiomeData")]
public class BiomeData : ScriptableObject
{
    public string biomeName; // 생물군계 이름
    public int solidGroindHeight; // 지면 높이
    public int terrainHeightRange; // 지면높이로 부터 증가할수있는 최대 높이값
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
