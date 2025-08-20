using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

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
}

public partial class LevelSystem : MonoBehaviour
{
  NativeArray<QuadData> _quadDatas;
  NativeArray<float3> _quadCenterOffsets;
  NativeHashMap<int, GroupQuadsData> _groupQuadDatas;

  void InitDataBuffers(LevelInformation levelInformation)
  {
    _quadDatas = new NativeArray<QuadData>(quadMeshSystem.QuadCapacity, Allocator.Persistent);
    _quadCenterOffsets = new NativeArray<float3>(quadMeshSystem.QuadCapacity, Allocator.Persistent);
    _groupQuadDatas = new NativeHashMap<int, GroupQuadsData>(32, Allocator.Persistent);
  }

  void DisposeDataBuffers()
  {
    _quadDatas.Dispose();
    _quadCenterOffsets.Dispose();
    _groupQuadDatas.Dispose();
  }
}