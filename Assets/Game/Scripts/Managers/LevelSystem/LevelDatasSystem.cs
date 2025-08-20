using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct QuadData
{
  public float3 Position;
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
  NativeArray<QuadData> _quadDatas;
  NativeHashMap<int, GroupOfQuadsData> _groupQuadDatas;

  void InitEntitiesDataBuffers(LevelInformation levelInformation)
  {
    _quadDatas = new NativeArray<QuadData>(quadMeshSystem.QuadCapacity, Allocator.Persistent);
    _groupQuadDatas = new NativeHashMap<int, GroupOfQuadsData>(32, Allocator.Persistent);
  }

  void DisposeDataBuffers()
  {
    _quadDatas.Dispose();
    _groupQuadDatas.Dispose();
  }
}