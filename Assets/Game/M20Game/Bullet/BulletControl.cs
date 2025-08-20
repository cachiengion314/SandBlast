using Unity.Mathematics;
using UnityEngine;

public class BulletControl : MonoBehaviour
  , IBullet
  , IMoveable
  , IColorBlock
{
  [Header("Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [Header("Datas")]
  int _colorValue;
  int _damage;
  float3 _velocity;
  float3 _lockedPosition;
  float _lifeDuration;
  Transform _lockedTarget;
  float3 _initPosition;
  float3[] _path;

  public void SetVelocity(float3 velocity)
  {
    _velocity = velocity;
  }

  public float3 GetVelocity()
  {
    return _velocity;
  }

  public int GetDamage()
  {
    return _damage;
  }

  public void SetDamage(int damage)
  {
    _damage = damage;
  }

  public void SetLockedPosition(float3 lockedPosition)
  {
    _lockedPosition = lockedPosition;
  }

  public float3 GetLockedPosition()
  {
    return _lockedPosition;
  }

  public float GetLifeTimer()
  {
    return _lifeDuration;
  }

  public void SetLifeTimer(float duration)
  {
    _lifeDuration = duration;
  }

  public Transform GetLockedTarget()
  {
    return _lockedTarget;
  }

  public void SetLockedTarget(Transform lockedTarget)
  {
    _lockedTarget = lockedTarget;
  }

  public void SetInitPostion(float3 pos)
  {
    _initPosition = pos;
  }

  public float3 GetInitPostion()
  {
    return _initPosition;
  }

  public void SetPath(float3[] path)
  {
    _path = path;
  }

  public float3[] GetPath()
  {
    return _path;
  }

  public int GetColorValue()
  {
    return _colorValue;
  }

  public void SetColorValue(int colorValue)
  {
    _colorValue = colorValue;
    bodyRenderer.color = RendererSystem.Instance.GetColorBy(colorValue);
  }

  public void SetIndex(int index)
  {
    throw new System.NotImplementedException();
  }

  public int GetIndex()
  {
    return -1;
  }
}