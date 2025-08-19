using Unity.Entities;
using UnityEngine;

public class EventBootstrap : MonoBehaviour
{
  void Start()
  {
    print("EventBootstrap ");
    var world = World.DefaultGameObjectInjectionWorld;
    var entityManager = world.EntityManager;

    // Create singleton event holder entity
    var entity = entityManager.CreateEntity();
    entityManager.AddBuffer<ReachingGoalEvent>(entity); // init a container of ReachingGoalEvent
  }
}
