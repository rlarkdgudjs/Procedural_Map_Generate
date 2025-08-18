using System;
using UnityEngine;

[Serializable]
public class BlockType
{
    public string blockName;
    public bool isSolid;

    [Header("Texture IDs")]
    public int topFaceTextureID;
    public int frontFaceTextureID;
    public int backFaceTextureID;
    public int leftFaceTextureID;
    public int rightFaceTextureID;
    public int bottomFaceTextureID;


    //Face Index(0~5)에 해당하는 텍스쳐 ID 리턴
    public int GetTextureID(int faceIndex)
    {
        switch (faceIndex)
        {
            case (int)VoxelData.VoxelFace.Top: return topFaceTextureID;
            case (int)VoxelData.VoxelFace.Front: return frontFaceTextureID;
            case (int)VoxelData.VoxelFace.Back: return backFaceTextureID;
            case (int)VoxelData.VoxelFace.Left: return leftFaceTextureID;
            case (int)VoxelData.VoxelFace.Right: return rightFaceTextureID;
            case (int)VoxelData.VoxelFace.Bottom: return bottomFaceTextureID;

            default:
                throw new IndexOutOfRangeException($"Face Index must be in 0 ~ 5, but input : {faceIndex}");
        }
    }
}
