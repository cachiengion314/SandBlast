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
  public int IndexPosition;
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
  NativeArray<int> _quadIndexPositionDatas; // index is position, value is the index in _quadDatas array
  NativeArray<float3> _shapeCenterOffsets;
  NativeHashMap<int, ShapeQuadData> _shapeQuadDatas;
  NativeHashMap<int, GroupQuadData> _groupQuadDatas;
  NativeArray<BlockData> _blockDatas;
  NativeArray<int2> _fullDirections;

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

    _quadIndexPositionDatas = new NativeArray<int>(totalBoardQuadsAmount, Allocator.Persistent);
    for (int i = 0; i < _quadIndexPositionDatas.Length; ++i) _quadIndexPositionDatas[i] = -1;
    _shapeCenterOffsets = new NativeArray<float3>(totalBoardQuadsAmount, Allocator.Persistent);
    _shapeQuadDatas = new NativeHashMap<int, ShapeQuadData>(blockGrid.GridSize.x * blockGrid.GridSize.y / 4, Allocator.Persistent);
    var totalShapeAmount = blockGrid.GridSize.x * blockGrid.GridSize.y / 4;
    _groupQuadDatas = new NativeHashMap<int, GroupQuadData>(totalShapeAmount, Allocator.Persistent);
    _blockDatas = new NativeArray<BlockData>(blockGrid.GridSize.x * blockGrid.GridSize.y, Allocator.Persistent);
    for (int i = 0; i < _blockDatas.Length; ++i)
    {
      var blockData = _blockDatas[i];
      blockData.Index = i;
      blockData.ShapeIndex = -1;
      _blockDatas[i] = blockData;
    }

    _fullDirections = new NativeArray<int2>(8, Allocator.Persistent);
    _fullDirections[0] = new(-1, -1);
    _fullDirections[1] = new(1, -1);
    _fullDirections[2] = new(0, -1);
    _fullDirections[3] = new(-1, 0);
    _fullDirections[4] = new(-1, 1);
    _fullDirections[5] = new(0, 1);
    _fullDirections[6] = new(1, 1);
    _fullDirections[7] = new(1, 0);
  }

  void DisposeDataBuffers()
  {
    _quadDatas.Dispose();
    _quadIndexPositionDatas.Dispose();
    _shapeCenterOffsets.Dispose();
    _shapeQuadDatas.Dispose();
    _groupQuadDatas.Dispose();
    _blockDatas.Dispose();
    _fullDirections.Dispose();
  }
}