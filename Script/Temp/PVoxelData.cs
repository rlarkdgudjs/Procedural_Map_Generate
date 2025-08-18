using UnityEngine;

public static class PVoxelData
{

    /* 
          7 �������� 6    
        / ��       / ��
      3 �������� 2   ��
      ��  ��     ��  ��
      ��  4����������5  
      ��/        ��/
      0 �������� 1
  */


    public static readonly Vector3[] voxelverts = new Vector3[8]{
        new Vector3(0.0f, 0.0f, 0.0f), // LB
        new Vector3(1.0f, 0.0f, 0.0f), // RB
        new Vector3(1.0f, 1.0f, 0.0f), // RT
        new Vector3(0.0f, 1.0f, 0.0f), // LT
        new Vector3(0.0f, 0.0f, 1.0f), // LB
        new Vector3(1.0f, 0.0f, 1.0f), // RB
        new Vector3(1.0f, 1.0f, 1.0f), // RT
        new Vector3(0.0f, 1.0f, 1.0f)  // LT

        };

    // �� ���� �̷�� �ﰢ���� 2��
    // ���ؽ� �ε����� �ð�������� ��ġ(�������� �׷�������)
    // �� ���� ���ؽ� ������ �ش� ���� �������� LB-LT-RB, RB-LT-RT
    /*
        LB-LT-RB   RB-LT-RT

        1          1 �� 2
        | ��         �� |
        0 �� 2          0
    */

    public static readonly int[,] voxeltris = new int[6, 4] {
        {0,3,1,2 },
        { 5, 6, 4, 7 },
        {3,7,2,6},
        {1,5,0,4 },
        {4,7,0,3 },
        {1,2,5,6 }
    };








}
