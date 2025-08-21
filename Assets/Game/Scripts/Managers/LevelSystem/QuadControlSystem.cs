using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  int _currentQuadAmount;
  int _currentGrabbingGroup = -1;

  int GetAvailableQuadAmount(int _additionAmount = 0)
  {
    var availableAmount
      = math.min(_currentQuadAmount + _additionAmount, quadMeshSystem.QuadCapacity);
    return availableAmount;
  }

  int2 ConvertBlockPosToSlotGridPos(float2 blockPos)
  {
    var centerSlot = new int2(
      (int)math.floor(slotGrid.GridSize.x / 2f),
      (int)math.floor(slotGrid.GridSize.y / 2f)
    );
    var r = new int2(
      (int)math.floor(centerSlot.x + blockPos.x * 8),
      (int)math.floor(centerSlot.y + blockPos.y * 8)
    );
    var slotGridPos = r - new int2(4, 4);
    return slotGridPos;
  }

  void OrderUnitBlockAt(int startIndex, float2 blockPos, int groupIndex, int colorValue)
  {
    var groupData = _groupQuadDatas[groupIndex];

    var startSlotGridPos = ConvertBlockPosToSlotGridPos(blockPos);
    var startX = startSlotGridPos.x;
    var startY = startSlotGridPos.y;

    var i = startIndex;
    for (int x = startX; x < startX + 8; ++x)
    {
      for (int y = startY; y < startY + 8; ++y)
      {
        var gridPos = new int2(x, y);
        var data = new QuadData
        {
          GroupIndex = groupIndex,
          Position = slotGrid.ConvertGridPosToWorldPos(gridPos),
          ColorValue = colorValue,
          IsActive = true,
        };
        _quadDatas[i] = data;
        _quadCenterOffsets[i] = data.Position - groupData.CenterPosition;
        i++;
      }
    }
  }

  /// <summary>
  /// block position is position from block space with (0,0) at the center of the slot.
  /// One unit block equal to an 8x8 quads size
  /// </summary>
  void OrderBlockShapeAt(
    int slotIndex,
    NativeArray<float2> blockPositions,
    int colorValue
  )
  {
    var slotPos = SetSlotGridPositionAt(slotIndex);

    var groupIdx = slotIndex;
    if (!_groupQuadDatas.ContainsKey(groupIdx))
      _groupQuadDatas.Add(
        groupIdx,
        new GroupQuadsData
        {
          CenterPosition = slotPos,
          ColorValue = colorValue,
          IsPlaced = false
        }
      );

    for (int i = 0; i < blockPositions.Length; ++i)
    {
      var blockPos = blockPositions[i];
      OrderUnitBlockAt(_currentQuadAmount + i * 64, blockPos, groupIdx, colorValue);
    }

    var additionAmount = blockPositions.Length * 64;
    var availableAmount = GetAvailableQuadAmount(additionAmount);
    _currentQuadAmount = availableAmount;
  }

  void ControlQuadsInUpdate()
  {
    if (_currentGrabbingGroup == -1) return;
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
    if (!_isUserScreenTouching) return;
    if (!_groupQuadDatas.ContainsKey(_currentGrabbingGroup)) return;

    var _userTouchScreenPos = new float3(
      userTouchScreenPosition.x, userTouchScreenPosition.y, 0
    );
    var _groupData = _groupQuadDatas[_currentGrabbingGroup];
    _groupData.CenterPosition = _userTouchScreenPos + touchOffset;
    _groupQuadDatas[_currentGrabbingGroup] = _groupData;

    for (int i = 0; i < _currentQuadAmount; ++i)
    {
      var quadData = _quadDatas[i];
      var isActive = quadData.IsActive;
      if (!isActive) continue;

      var groupIdx = quadData.GroupIndex;
      if (groupIdx != _currentGrabbingGroup) continue;

      var groupData = _groupQuadDatas[groupIdx];
      var offset = _quadCenterOffsets[i];
      var nextQuadPos = groupData.CenterPosition + offset;
      quadData.Position = nextQuadPos;

      _quadDatas[i] = quadData;
      quadMeshSystem.OrderQuadMeshAt(i, nextQuadPos, -1, -1);
    }

    quadMeshSystem.ApplyDrawOrders();
  }
}