using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [SerializeField] Transform slotsParent;

  float3 SetAndGetSlotGridPositionAt(int slotIndex)
  {
    var slotPos = GetSlotPositionAt(slotIndex);
    slotGrid.transform.position = slotPos;
    return slotPos;
  }

  float3 GetSlotPositionAt(int slotIndex)
  {
    if (slotIndex < 0) return -1;
    if (slotIndex > slotsParent.childCount - 1) return -1;
    return slotsParent.GetChild(slotIndex).position;
  }

  Collider2D FindSlotIn(Collider2D[] cols)
  {
    for (int i = 0; i < cols.Length; ++i)
    {
      var col = cols[i];
      if (col == null) continue;
      if (col.gameObject.layer == LayerMask.NameToLayer("Slot"))
        return col;
    }
    return null;
  }

  int FindSlotIndexOf(Transform slot)
  {
    for (int i = 0; i < slotsParent.childCount; ++i)
      if (slotsParent.GetChild(i) == slot) return i;
    return -1;
  }

  int GetCurrentGroupIndexInSlot(int slotIndex)
  {
    if (!_groupQuadDatas.ContainsKey(slotIndex)) return -1;
    var groupData = _groupQuadDatas[slotIndex];
    if (groupData.IsPlaced) return -1;

    return slotIndex;
  }

  bool IsSlotEmptyAt(int slotIndex)
  {
    var currentGroup = GetCurrentGroupIndexInSlot(slotIndex);
    if (currentGroup == -1) return true;
    return false;
  }

  void OnTouchSlot(int slotIndex)
  {
    if (IsSlotEmptyAt(slotIndex)) return;

    _currentGrabbingGroupIndex = slotIndex;
  }

  void OnInactive()
  {
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
    if (!_groupQuadDatas.ContainsKey(_currentGrabbingGroupIndex)) return;
    if (IsBlockShapeOutsideAt(_currentGrabbingGroupIndex))
    {
      var slotPos = SetAndGetSlotGridPositionAt(_currentGrabbingGroupIndex);
      OrderBlockPositionsTo(slotPos, _currentGrabbingGroupIndex);
      ApplyDrawOrders();

      _currentGrabbingGroupIndex = -1;
      return;
    }

    var groupData = _groupQuadDatas[_currentGrabbingGroupIndex];
    groupData.IsPlaced = true;
    _groupQuadDatas[_currentGrabbingGroupIndex] = groupData;

    _currentGrabbingGroupIndex = -1;
  }
}