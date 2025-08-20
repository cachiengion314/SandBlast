using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct QuadData
{
  public int GroupIndex;
  public bool IsActive;
  public int ColorValue;
}

public struct GroupOfQuadsData
{
  public float3 CenterPosition;
  public int ColorValue;
}

public partial class LevelSystem : MonoBehaviour
{
  NativeArray<float3> _quadPositions;
  NativeArray<QuadData> _quadDatas;
  NativeHashMap<int, GroupOfQuadsData> _groupQuadDatas;

  void InitEntitiesDataBuffers(LevelInformation levelInformation)
  {
    _quadPositions = new NativeArray<float3>(quadMeshSystem.QuadCapacity, Allocator.Persistent);
    _quadDatas = new NativeArray<QuadData>(quadMeshSystem.QuadCapacity, Allocator.Persistent);
    _groupQuadDatas = new NativeHashMap<int, GroupOfQuadsData>(32, Allocator.Persistent);
  }

  void DisposeDataBuffers()
  {
    _quadPositions.Dispose();
    _quadDatas.Dispose();
    _groupQuadDatas.Dispose();
  }
}