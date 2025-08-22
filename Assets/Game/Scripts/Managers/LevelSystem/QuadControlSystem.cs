using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  int _currentQuadAmount;
  int _currentGrabbingGroupIndex = -1;

  int GetCurrentBlockAmount()
  {
    return _currentQuadAmount / 64;
  }

  bool IsBlockShapeOutsideAt(int groupIdx)
  {
    var _currentBlockAmount = GetCurrentBlockAmount();
    for (int i = 0; i < _currentBlockAmount; ++i)
    {
      var data = _blockDatas[i];
      var isActive = data.IsActive;
      if (!isActive) continue;
      var _groupIdx = data.GroupIndex;
      if (_groupIdx != groupIdx) continue;

      var blockPos = data.Position;
      if (blockGrid.IsPosOutsideAt(blockPos)) return true;
    }
    return false;
  }

  int GetAvailableQuadAmount(int _additionAmount = 0)
  {
    var availableAmount
      = math.min(_currentQuadAmount + _additionAmount, quadMeshSystem.QuadCapacity);
    return availableAmount;
  }

  float3 ConvertSlotPosToWorldPos(float2 blockSlotPos)
  {
    var slotPos = (float3)slotGrid.transform.position;
    var blockPos = slotPos + new float3(
      blockSlotPos.x * quadGrid.GridScale.x * 8,
      blockSlotPos.y * quadGrid.GridScale.y * 8,
      0
    );
    return blockPos;
  }

  int2 ConvertBlockPosToSlotGridPos(float2 blockSlotPos)
  {
    var centerSlot = new int2(
      (int)math.floor(slotGrid.GridSize.x / 2f),
      (int)math.floor(slotGrid.GridSize.y / 2f)
    );
    var r = new int2(
      (int)math.floor(centerSlot.x + blockSlotPos.x * 8),
      (int)math.floor(centerSlot.y + blockSlotPos.y * 8)
    );
    var slotGridPos = r - 4;
    return slotGridPos;
  }

  void OrderUnitBlockAt(int startIndex, float2 blockSlotPos, int groupIndex, int colorValue)
  {
    var groupData = _groupQuadDatas[groupIndex];

    var startSlotGridPos = ConvertBlockPosToSlotGridPos(blockSlotPos);
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
    NativeArray<float2> blockSlotPositions,
    int colorValue
  )
  {
    var _currentBlockAmount = GetCurrentBlockAmount();
    var slotPos = SetAndGetSlotGridPositionAt(slotIndex);

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

    for (int i = 0; i < blockSlotPositions.Length; ++i)
    {
      var blockSlotPos = blockSlotPositions[i];
      OrderUnitBlockAt(_currentQuadAmount + i * 64, blockSlotPos, groupIdx, colorValue);

      var blockPos = ConvertSlotPosToWorldPos(blockSlotPos);
      var blockData = new BlockData
      {
        GroupIndex = groupIdx,
        Position = blockPos,
        ColorValue = colorValue,
        IsActive = true,
        CenterOffset = blockPos - slotPos,
      };
      _blockDatas[_currentBlockAmount + i] = blockData;
    }

    var additionAmount = blockSlotPositions.Length * 64;
    var availableAmount = GetAvailableQuadAmount(additionAmount);
    _currentQuadAmount = availableAmount;
  }

  void OrderBlockPositionsTo(float3 targetPos, int groupIdx)
  {
    var groupData = _groupQuadDatas[groupIdx];
    groupData.CenterPosition = targetPos;
    _groupQuadDatas[groupIdx] = groupData;

    var _currentBlockAmount = GetCurrentBlockAmount();
    for (int i = 0; i < _currentBlockAmount; ++i)
    {
      var blockData = _blockDatas[i];
      if (!blockData.IsActive) continue;
      if (blockData.GroupIndex != groupIdx) continue;

      var blockGroupId = blockData.GroupIndex;
      if (_groupQuadDatas[blockGroupId].IsPlaced) continue;

      var offset = blockData.CenterOffset;
      var nextBlockPos = groupData.CenterPosition + offset;
      blockData.Position = nextBlockPos;

      _blockDatas[i] = blockData;
    }

    for (int i = 0; i < _currentQuadAmount; ++i)
    {
      var quadData = _quadDatas[i];
      if (!quadData.IsActive) continue;
      if (quadData.GroupIndex != groupIdx) continue;
      var quadGroupId = quadData.GroupIndex;
      if (_groupQuadDatas[quadGroupId].IsPlaced) continue;

      var offset = _quadCenterOffsets[i];
      var nextQuadPos = groupData.CenterPosition + offset;
      quadData.Position = nextQuadPos;

      _quadDatas[i] = quadData;
      OrderQuadMeshAt(i, nextQuadPos, quadData.ColorValue);
    }
  }

  void GrabbingBlockControlInUpdate()
  {
    if (_currentGrabbingGroupIndex == -1) return;
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
    if (!_isUserScreenTouching) return;
    if (!_groupQuadDatas.ContainsKey(_currentGrabbingGroupIndex)) return;

    var _userTouchScreenPos = new float3(
      userTouchScreenPosition.x, userTouchScreenPosition.y, 0
    );
    var targetPos = _userTouchScreenPos + touchOffset;
    OrderBlockPositionsTo(targetPos, _currentGrabbingGroupIndex);
    ApplyDrawOrders();
  }

  void OrderQuadMeshAt(int index, float3 pos, int colorValue)
  {
    var uvGridPos = RendererSystem.Instance.GetUVGridPosFrom(colorValue);
    quadMeshSystem.OrderQuadMeshAt(index, pos, uvGridPos);
  }

  void ApplyDrawOrders()
  {
    quadMeshSystem.ApplyDrawOrders();
  }
}