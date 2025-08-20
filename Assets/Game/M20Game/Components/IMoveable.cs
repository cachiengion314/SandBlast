using Unity.Mathematics;
using UnityEngine;

public interface IMoveable
{
  public void SetInitPostion(float3 pos);
  public float3 GetInitPostion();
  public void SetLockedPosition(float3 lockedPosition);
  public float3 GetLockedPosition();
  public Transform GetLockedTarget();
  public void SetLockedTarget(Transform lockedTarget);
  public void SetPath(float3[] path);
  public float3[] GetPath();
}
