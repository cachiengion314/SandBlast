using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct BlockData
{
  public int GroupIndex;
  public float3 Position;
  public float3 CenterOffset;
}

public struct QuadData
{
  public int GroupIndex;
  public float3 Position;
  public int ColorValue;
}

public struct GroupQuadsData
{
  public float3 CenterPosition;
  public int ColorValue;
  public bool IsActive;
}

public struct GroupQuadsListData
{
  public int GroupIndex;
  public int StartSpawnedQuadIndex;
  public int EndSpawnedQuadIndex;
}

public partial class LevelSystem : MonoBehaviour
{
  NativeArray<QuadData> _quadDatas;
  NativeArray<float3> _quadCenterOffsets;
  NativeHashMap<int, GroupQuadsData> _groupQuadDatas;
  NativeArray<BlockData> _blockDatas;
  NativeList<GroupQuadsListData> _groupQuadsListDatas;

  void InitDataBuffers(LevelInformation levelInformation)
  {
    _quadDatas = new NativeArray<QuadData>(quadMeshSystem.QuadCapacity, Allocator.Persistent);
    _quadCenterOffsets = new NativeArray<float3>(quadMeshSystem.QuadCapacity, Allocator.Persistent);
    _groupQuadDatas = new NativeHashMap<int, GroupQuadsData>(blockGrid.GridSize.x * blockGrid.GridSize.y / 4, Allocator.Persistent);
    _groupQuadsListDatas = new NativeList<GroupQuadsListData>(blockGrid.GridSize.x * blockGrid.GridSize.y / 4, Allocator.Persistent);
    _blockDatas = new NativeArray<BlockData>(blockGrid.GridSize.x * blockGrid.GridSize.y, Allocator.Persistent);
  }

  void DisposeDataBuffers()
  {
    _quadDatas.Dispose();
    _quadCenterOffsets.Dispose();
    _groupQuadDatas.Dispose();
    _groupQuadsListDatas.Dispose();
    _blockDatas.Dispose();
  }
}