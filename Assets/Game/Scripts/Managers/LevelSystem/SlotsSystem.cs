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

  bool IsPlacedShape(int shapeIdx)
  {
    return shapeIdx >= slotsParent.childCount;
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
    if (!_blockShapeDatas.ContainsKey(slotIndex)) return -1;
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
  }

  void OnInactive()
  {
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
    if (!_blockShapeDatas.ContainsKey(_currentGrabbingShapeIndex)) return;
    if (IsBlockShapeOutsideAt(_currentGrabbingShapeIndex))
    {
      var slotPos = GetAndSetSlotGridPositionAt(_currentGrabbingShapeIndex);
      OrderShapePositionsTo(slotPos, _currentGrabbingShapeIndex);
      ApplyDrawOrders();

      _currentGrabbingShapeIndex = -1;
      return;
    }

    // change current shape of slot to the shkape that belong to the board space
    var oldShapeData = _blockShapeDatas[_currentGrabbingShapeIndex];
    var newShapeIdx = GenerateUniqueShapeIdx();
    var newShapeData = new BlockShapeData
    {
      CenterPosition = oldShapeData.CenterPosition,
      StartSpawnedQuadIndex = oldShapeData.StartSpawnedQuadIndex,
      QuadsAmount = oldShapeData.QuadsAmount,
      ColorValue = oldShapeData.ColorValue,
      IsActive = true
    };
    _blockShapeDatas.Add(newShapeIdx, newShapeData);

    AssignQuadsToNewShape(newShapeIdx, _currentGrabbingShapeIndex);

    _currentGrabbingShapeIndex = -1;
  }
}