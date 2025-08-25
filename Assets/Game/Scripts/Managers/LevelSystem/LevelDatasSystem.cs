using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct BlockData
{
  public int ShapeIndex;
  public float3 Position;
  public float3 CenterOffset;
}

public struct QuadData
{
  public int ShapeIndex;
  public float3 Position;
  public int ColorValue;
}

public struct BlockShapeData
{
  public float3 CenterPosition;
  public int StartSpawnedQuadIndex;
  public int QuadsAmount;
  public int ColorValue;
  public bool IsActive;
}

public partial class LevelSystem : MonoBehaviour
{
  NativeArray<QuadData> _quadDatas;
  NativeArray<float3> _shapeCenterOffsets;
  NativeHashMap<int, BlockShapeData> _blockShapeDatas;
  NativeArray<BlockData> _blockDatas;

  void InitDataBuffers(LevelInformation levelInformation)
  {
    _quadDatas = new NativeArray<QuadData>(quadMeshSystem.QuadCapacity, Allocator.Persistent);
    _shapeCenterOffsets = new NativeArray<float3>(quadMeshSystem.QuadCapacity, Allocator.Persistent);
    _blockShapeDatas = new NativeHashMap<int, BlockShapeData>(blockGrid.GridSize.x * blockGrid.GridSize.y / 4, Allocator.Persistent);
    _blockDatas = new NativeArray<BlockData>(blockGrid.GridSize.x * blockGrid.GridSize.y, Allocator.Persistent);
  }

  void DisposeDataBuffers()
  {
    _quadDatas.Dispose();
    _shapeCenterOffsets.Dispose();
    _blockShapeDatas.Dispose();
    _blockDatas.Dispose();
  }
}