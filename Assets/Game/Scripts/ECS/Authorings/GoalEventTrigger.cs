using Unity.Entities;
using UnityEngine;

public class GoalEventTrigger : MonoBehaviour
{
  public static GoalEventTrigger Instance { get; private set; }

  void Start()
  {
    if (Instance == null)
      Instance = this;
    else Destroy(gameObject);
  }

  public void NotifyGoalReached()
  {
    var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

    using var query = entityManager
      .CreateEntityQuery(ComponentType.ReadWrite<ReachingGoalEvent>());
    if (query.IsEmpty) return;

    var eventEntity = query.GetSingletonEntity();
    var buffer = entityManager.GetBuffer<ReachingGoalEvent>(eventEntity);

    buffer.Add(new ReachingGoalEvent
    {
      Position = transform.position
    });
  }
}
