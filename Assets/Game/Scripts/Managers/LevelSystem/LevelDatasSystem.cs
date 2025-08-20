using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  NativeArray<float3> _quadPositions;
  NativeArray<bool> _quadActives;

  void InitEntitiesDataBuffers(LevelInformation levelInformation)
  {
    _quadPositions = new NativeArray<float3>(quadMeshSystem.QuadCapacity, Allocator.Persistent);
    _quadActives = new NativeArray<bool>(quadMeshSystem.QuadCapacity, Allocator.Persistent);
  }

  void DisposeDataBuffers()
  {
    _quadPositions.Dispose();
    _quadActives.Dispose();
  }
}