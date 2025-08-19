using Unity.Entities;
using Unity.Mathematics;

public struct Spawner : IComponentData
{
  public Entity Prefab;
  public int Count;
  public float3 Offset;
}