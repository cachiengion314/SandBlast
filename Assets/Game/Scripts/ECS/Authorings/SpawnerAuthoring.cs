using Unity.Entities;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
  public GameObject prefab;
  public int count;
  public Vector3 offset;
}

/// <summary>
/// Create a Spawner entity in ECS world
/// This is actually a bridge that connect between ECS world and MonoBehavior world
/// </summary>
public class SpawnerBaker : Baker<SpawnerAuthoring>
{
  public override void Bake(SpawnerAuthoring authoring)
  {
    var entity = GetEntity(TransformUsageFlags.Dynamic);
    AddComponent(
      entity,
      new Spawner
      {
        Prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
        Count = authoring.count,
        Offset = authoring.offset
      }
    );
  }
}
