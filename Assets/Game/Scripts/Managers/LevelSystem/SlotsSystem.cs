using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [SerializeField] Transform slotsParent;

  int GenerateUniqueShapeIdx()
  {
    var newShapeIdx = slotsParent.childCount + _placedShapesAmount;
    _placedShapesAmount++;
    return newShapeIdx;
  }

  bool IsSlotsEmpty()
  {
    for (int i = 0; i < slotsParent.childCount; i++)
      if (!IsSlotEmptyAt(i)) return false;
    return true;
  }

  void OrderShapesForSlots()
  {
    for (int i = 0; i < slotsParent.childCount; i++)
    {
      var colorValue = GetRamdomColor();
      using var blockSlotPositions = GetRandomShape();
      OrderBlockShapeAt(i, blockSlotPositions, colorValue);

      if (!_shapeQuadDatas.ContainsKey(i)) return;
      var shapeData = _shapeQuadDatas[i];
      _shapeQuadDatas[i] = shapeData;
    }
  }

  float3 GetAndSetSlotGridPositionAt(int slotIndex)
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
    if (!_shapeQuadDatas.ContainsKey(slotIndex)) return -1;
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
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
    if (IsSlotEmptyAt(slotIndex)) return;

    _currentGrabbingShapeIndex = slotIndex;
    SoundManager.Instance.PlayDragBlockSfx();
  }

  void OnInactive()
  {
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
    if (!_shapeQuadDatas.ContainsKey(_currentGrabbingShapeIndex)) return;

    if (IsQuadsInplaceableAt(_currentGrabbingShapeIndex))
    {
      var slotPos = GetAndSetSlotGridPositionAt(_currentGrabbingShapeIndex);
      OrderShapePositionsTo(slotPos, _currentGrabbingShapeIndex);

      _currentGrabbingShapeIndex = -1;
      SoundManager.Instance.PlayDropBlockSfx();
      return;
    }

    var newGroupIdx = GenerateUniqueShapeIdx();
    AssignQuadsToNewGroup(newGroupIdx, _currentGrabbingShapeIndex);

    if (IsSlotsEmpty())
    {
      OrderShapesForSlots();
      VisualizeActiveQuads();
    }

    _currentGrabbingShapeIndex = -1;
  }
}