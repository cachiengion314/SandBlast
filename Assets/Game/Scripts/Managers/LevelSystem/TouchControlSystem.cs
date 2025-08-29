using Lean.Touch;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [Header("Touch Control System")]
  bool _isUserScreenTouching;
  public bool IsUserScreenTouching { get { return _isUserScreenTouching; } }
  float3 touchOffset = new(0, 2.5f, 0);
  float3 userTouchScreenPosition;

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
    if (!isLoadedLevel) return;
    _isUserScreenTouching = true;
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;

    userTouchScreenPosition = Camera.main.ScreenToWorldPoint(finger.ScreenPosition);

    if (Input.GetKey(KeyCode.B))
    {
      var pos = new float3(userTouchScreenPosition.x, userTouchScreenPosition.y, 0);
      var idxPos = quadGrid.ConvertWorldPosToIndex(pos);
      using var quadsMap = CollectLinkedQuadsAt(idxPos);
      using var quadDatas = quadsMap.GetKeyArray(Allocator.Temp);
      if (quadsMap.Count > 0)
        RemoveQuadsFrom(quadDatas);
    }

    if (isTriggerBooster3)
    {
      OnTriggerBooster3();
      return;
    }

    Collider2D[] colliders = Physics2D.OverlapPointAll(
      new float2(userTouchScreenPosition.x, userTouchScreenPosition.y)
    );
    var slot = FindSlotIn(colliders);
    if (slot == null) return;

    var slotIdx = FindSlotIndexOf(slot.transform);
    OnTouchSlot(slotIdx);
  }

  void OnFingerUpdate(LeanFinger finger)
  {
    if (!isLoadedLevel) return;
    _isUserScreenTouching = true;

    userTouchScreenPosition = Camera.main.ScreenToWorldPoint(finger.ScreenPosition);
  }

  private void OnFingerInactive(LeanFinger finger)
  {
    if (!isLoadedLevel) return;
    _isUserScreenTouching = false;

    OnInactive();
  }
}