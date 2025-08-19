using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AngryBlock
{
  public partial struct MovingBlockSystem : ISystem
  {
    public void OnCreate(ref SystemState state)
    {
      state.RequireForUpdate<Spawner>();
      Debug.Log("MovingBlockSystem invoke");
    }

    public void OnUpdate(ref SystemState state)
    {
      foreach (var (transform, entity) in
               SystemAPI.Query<RefRW<LocalTransform>>()
               .WithAll<Block, Moveable>()
               .WithEntityAccess())
      {
        var nextPosition
          = transform.ValueRO.Position
            + SystemAPI.Time.DeltaTime * new float3(1, 0, 0);
        transform.ValueRW.Position = nextPosition;
      }
    }
  }
}