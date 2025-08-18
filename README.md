# Procedural_Map_Generate

청크(Chunk) 기반 복셀 월드를 실시간으로 생성하는 Unity 예제입니다.

플레이어 주변만 생성하고, 범위를 벗어나면 비활성화하여 성능을 확보합니다.

## 프로젝트 구조
```
Assets/
└─ Script/
   ├─ World.cs              // 월드/청크 관리 + 스레드 디스패치
   ├─ Chunk.cs              // 청크 GameObject + MeshRenderer/Filter 구성
   ├─ ChunkMeshBuilder.cs   // MeshData 생성
   ├─ VoxelData.cs          // 복셀/아틀라스 상수 및 헬퍼
   ├─ (BlockType.cs, BiomeData.cs, Noise.cs 등 사용됨)
```
## 동작 개요
1. 시야 범위(`viewDistance`) 내 청크 순회
   - 새 좌표 → 청크 생성 요청
   - 이미 존재하는 청크 → `SetActive(true)`
2. 워커 스레드: `voxelMap` 채우기 + `ChunkMeshBuilder.Build()` meshData 생성
3. 메인 스레드: `Chunk.Init()` meshData 기반 mesh 생성
4. 시야 밖 청크는 `SetActive(false)` 처리
