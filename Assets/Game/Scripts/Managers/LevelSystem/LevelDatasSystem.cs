using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct BlockData
{
  public int GroupIndex;
  public float3 Position;
  public int ColorValue;
  public bool IsActive;
  public float3 CenterOffset;
}

public struct QuadData
{
  public int GroupIndex;
  public float3 Position;
  public int ColorValue;
  public bool IsActive;
}

public struct GroupQuadsData
{
  public float3 CenterPosition;
  public int ColorValue;
  public bool IsPlaced;
}

public partial class LevelSystem : MonoBehaviour
{
  NativeArray<QuadData> _quadDatas;
  NativeArray<float3> _quadCenterOffsets;
  NativeHashMap<int, GroupQuadsData> _groupQuadDatas;
  NativeArray<BlockData> _blockDatas;

  void InitDataBuffers(LevelInformation levelInformation)
  {
    _quadDatas = new NativeArray<QuadData>(quadMeshSystem.QuadCapacity, Allocator.Persistent);
    _quadCenterOffsets = new NativeArray<float3>(quadMeshSystem.QuadCapacity, Allocator.Persistent);
    _groupQuadDatas = new NativeHashMap<int, GroupQuadsData>(64, Allocator.Persistent);
    _blockDatas = new NativeArray<BlockData>(quadMeshSystem.QuadCapacity / 64, Allocator.Persistent);
  }

  void DisposeDataBuffers()
  {
    _quadDatas.Dispose();
    _quadCenterOffsets.Dispose();
    _groupQuadDatas.Dispose();
    _blockDatas.Dispose();
  }
}