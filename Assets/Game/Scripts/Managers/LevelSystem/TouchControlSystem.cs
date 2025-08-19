using System.Collections.Generic;
using Lean.Touch;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [Header("Touch Control System")]
  bool _isUserScreenTouching;
  public bool IsUserScreenTouching { get { return _isUserScreenTouching; } }
  readonly RaycastHit[] results = new RaycastHit[10];

  void SubscribeTouchEvent()
  {
    LeanTouch.OnFingerDown += OnFingerDown;
    LeanTouch.OnGesture += OnGesture;
    LeanTouch.OnFingerInactive += OnFingerInactive;
  }

  void UnsubscribeTouchEvent()
  {
    LeanTouch.OnFingerDown -= OnFingerDown;
    LeanTouch.OnGesture -= OnGesture;
    LeanTouch.OnFingerInactive -= OnFingerInactive;
  }

  private void OnFingerDown(LeanFinger finger)
  {
    _isUserScreenTouching = true;

    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
    Vector3 startTouchPos = Camera.main.ScreenToWorldPoint(finger.ScreenPosition);

    var ray = new Ray(startTouchPos, Camera.main.transform.forward);
    var hitCount = Physics.RaycastNonAlloc(ray, results);
    if (hitCount == 0) return;
  
  }

  void OnGesture(List<LeanFinger> list)
  {
    _isUserScreenTouching = true;
  }

  private void OnFingerInactive(LeanFinger finger)
  {
    _isUserScreenTouching = false;
  }

  public Collider FindObjIn<T>(RaycastHit[] cols, int hitCount)
  {
    for (int i = 0; i < hitCount; ++i)
    {
      if (cols[i].collider == null) continue;
      if (cols[i].collider.TryGetComponent<T>(out var comp))
      {
        return cols[i].collider;
      }
    }
    return default;
  }
}