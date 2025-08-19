using Lean.Touch;
using Unity.Entities;
using UnityEngine;

namespace AngryBlock
{
  public partial class TouchControlSystem : SystemBase
  {
    protected override void OnCreate()
    {
      RequireForUpdate<Spawner>();
      // LeanTouch.OnFingerDown += OnFingerDown;
    }

    protected override void OnDestroy()
    {
      // LeanTouch.OnFingerDown -= OnFingerDown;
    }

    protected override void OnUpdate()
    {
      // normal ECS update
    }

    private void OnFingerDown(LeanFinger finger)
    {
      Debug.Log("OnFingerDown ");
      GoalEventTrigger.Instance.NotifyGoalReached();
    }
  }
}