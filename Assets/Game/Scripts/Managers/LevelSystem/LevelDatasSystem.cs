using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct BlockData
{
  public int Index;
  public int ShapeIndex;
  public float3 Position;
  public float3 CenterOffset;
}

public struct QuadData
{
  public int Index;
  public int GroupIndex;
  public float3 Position;
  public int PlacedIndex;
  public int ColorValue;
  public bool IsActive;
}

public struct ShapeQuadData
{
  public float3 CenterPosition;
  public int QuadsAmount;
  public int ColorValue;
}

public struct GroupQuadData
{
  public int QuadsAmount;
  public int ColorValue;
  public bool IsActive;
}

public partial class LevelSystem : MonoBehaviour
{
  NativeArray<QuadData> _quadDatas;
  NativeArray<int> _quadIndexesDatas; // index is position, value is the index in _quadDatas array
  NativeArray<float3> _shapeCenterOffsets;
  NativeHashMap<int, ShapeQuadData> _shapeQuadDatas;
  NativeHashMap<int, GroupQuadData> _groupQuadDatas;
  NativeArray<BlockData> _blockDatas;
  NativeArray<int2> _diagonalDirections;

  void InitDataBuffers(LevelInformation levelInformation)
  {
    var totalBoardQuadsAmount = quadGrid.GridSize.x * quadGrid.GridSize.y;
    _quadDatas = new NativeArray<QuadData>(totalBoardQuadsAmount, Allocator.Persistent);
    for (int i = 0; i < _quadDatas.Length; ++i)
    {
      var quadData = _quadDatas[i];
      quadData.Index = i;
      quadData.GroupIndex = -1;
      _quadDatas[i] = quadData;
    }

    _quadIndexesDatas = new NativeArray<int>(totalBoardQuadsAmount, Allocator.Persistent);
    for (int i = 0; i < _quadIndexesDatas.Length; ++i) _quadIndexesDatas[i] = -1;
    _shapeCenterOffsets = new NativeArray<float3>(totalBoardQuadsAmount, Allocator.Persistent);
    _shapeQuadDatas = new NativeHashMap<int, ShapeQuadData>(blockGrid.GridSize.x * blockGrid.GridSize.y / 4, Allocator.Persistent);
    _groupQuadDatas = new NativeHashMap<int, GroupQuadData>(16, Allocator.Persistent);
    _blockDatas = new NativeArray<BlockData>(blockGrid.GridSize.x * blockGrid.GridSize.y, Allocator.Persistent);
    for (int i = 0; i < _blockDatas.Length; ++i)
    {
      var blockData = _blockDatas[i];
      blockData.Index = i;
      blockData.ShapeIndex = -1;
      _blockDatas[i] = blockData;
    }

    _diagonalDirections = new NativeArray<int2>(2, Allocator.Persistent);
    _diagonalDirections[0] = new(-1, -1);
    _diagonalDirections[1] = new(1, -1);
  }

  void DisposeDataBuffers()
  {
    _quadDatas.Dispose();
    _quadIndexesDatas.Dispose();
    _shapeCenterOffsets.Dispose();
    _shapeQuadDatas.Dispose();
    _groupQuadDatas.Dispose();
    _blockDatas.Dispose();
    _diagonalDirections.Dispose();
  }
}