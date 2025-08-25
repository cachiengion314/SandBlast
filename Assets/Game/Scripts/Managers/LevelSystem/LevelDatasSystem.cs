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
  public int PlacedIndex;
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
  NativeArray<int> _quadIndexesDatas; // index is position, value is the index in _quadDatas array
  NativeArray<float3> _shapeCenterOffsets;
  NativeHashMap<int, BlockShapeData> _blockShapeDatas;
  NativeArray<BlockData> _blockDatas;

  void InitDataBuffers(LevelInformation levelInformation)
  {
    var totalBoardQuadsAmount = quadGrid.GridSize.x * quadGrid.GridSize.y;
    _quadDatas = new NativeArray<QuadData>(totalBoardQuadsAmount, Allocator.Persistent);
    _quadIndexesDatas = new NativeArray<int>(totalBoardQuadsAmount, Allocator.Persistent);
    for (int i = 0; i < _quadIndexesDatas.Length; ++i) _quadIndexesDatas[i] = -1;
    _shapeCenterOffsets = new NativeArray<float3>(totalBoardQuadsAmount, Allocator.Persistent);
    _blockShapeDatas = new NativeHashMap<int, BlockShapeData>(blockGrid.GridSize.x * blockGrid.GridSize.y / 4, Allocator.Persistent);
    _blockDatas = new NativeArray<BlockData>(blockGrid.GridSize.x * blockGrid.GridSize.y, Allocator.Persistent);
  }

  void DisposeDataBuffers()
  {
    _quadDatas.Dispose();
    _quadIndexesDatas.Dispose();
    _shapeCenterOffsets.Dispose();
    _blockShapeDatas.Dispose();
    _blockDatas.Dispose();
  }
}