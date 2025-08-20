using Lean.Touch;
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
    LeanTouch.OnFingerUpdate += OnFingerUpdate;
    LeanTouch.OnFingerInactive += OnFingerInactive;
  }

  void UnsubscribeTouchEvent()
  {
    LeanTouch.OnFingerDown -= OnFingerDown;
    LeanTouch.OnFingerUpdate -= OnFingerUpdate;
    LeanTouch.OnFingerInactive -= OnFingerInactive;
  }

  private void OnFingerDown(LeanFinger finger)
  {
    _isUserScreenTouching = true;
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;

    userScreenPosition = Camera.main.ScreenToWorldPoint(finger.ScreenPosition);
  }

  void OnFingerUpdate(LeanFinger finger)
  {
    _isUserScreenTouching = true;

    userScreenPosition = Camera.main.ScreenToWorldPoint(finger.ScreenPosition);
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