using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace AngryBlock
{
  public partial struct GoalReactionSystem : ISystem
  {
    public void OnUpdate(ref SystemState state)
    {
      using var ecb = new EntityCommandBuffer(Allocator.Temp);

      foreach (var (buffer, bufferEntity) in
        SystemAPI
          .Query<DynamicBuffer<ReachingGoalEvent>>()
          .WithEntityAccess()
      )
      {
        if (buffer.Length > 0)
        {
          // Only do this once for all events
          foreach (var (transform, blockEntity) in
                   SystemAPI.Query<RefRW<LocalTransform>>()
                            .WithAll<Block, Moveable>()
                            .WithEntityAccess())
          {
            ecb.RemoveComponent<Moveable>(blockEntity);
          }

          buffer.Clear(); // Clear events after processing
        }
      }

      ecb.Playback(state.EntityManager);
    }
  }
}