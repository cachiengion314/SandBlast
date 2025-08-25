using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  int _currentQuadAmount;
  int _placedShapesAmount;
  int _currentGrabbingShapeIndex = -1;

  void OrderQuadMeshAt(int index, float3 pos, int colorValue)
  {
    // var uvGridPos = RendererSystem.Instance.GetUVGridPosFrom(colorValue);
    var uvGridPos = quadMeshSystem.ConvertIndexToGridPos(colorValue);
    quadMeshSystem.OrderQuadMeshAt(index, pos, uvGridPos);
  }

  void ApplyDrawOrders()
  {
    quadMeshSystem.ApplyDrawOrders();
  }

  void AssignQuadsToNewShape(int newShapeIdx, int oldShapeIdx)
  {
    for (int i = 0; i < _currentQuadAmount; ++i)
    {
      var quadData = _quadDatas[i];
      if (quadData.ShapeIndex != oldShapeIdx) continue;
      quadData.ShapeIndex = newShapeIdx;
      _quadDatas[i] = quadData;
    }
    _blockShapeDatas.Remove(oldShapeIdx);
  }

  int GetCurrentBlockAmount()
  {
    return _currentQuadAmount / 64;
  }

  bool IsBlockShapeOutsideAt(int shapeIdx)
  {
    var _currentBlockAmount = GetCurrentBlockAmount();
    for (int i = 0; i < _currentBlockAmount; ++i)
    {
      var data = _blockDatas[i];
      var _shapeIdx = data.ShapeIndex;
      if (_shapeIdx != shapeIdx) continue;

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

  void OrderUnitBlockAt(int startIndex, float2 blockSlotPos, int shapeIndex, int colorValue)
  {
    var shapeData = _blockShapeDatas[shapeIndex];

    var startSlotGridPos = ConvertBlockPosToSlotGridPos(blockSlotPos);
    var startX = startSlotGridPos.x;
    var startY = startSlotGridPos.y;

    var i = startIndex;
    var colorGird = quadMeshSystem.ConvertIndexToGridPos(colorValue);
    for (int x = startX; x < startX + 8; ++x)
    {
      for (int y = startY; y < startY + 8; ++y)
      {
        var newColorIndex = colorValue;
        var ratio = UnityEngine.Random.Range(0f, 100f);
        if (ratio > 80f)
        {
          var xColor = UnityEngine.Random.Range(0, quadMeshSystem.GridResolution.x - 1);
          var newColorGrid = new int2(xColor, colorGird.y);
          newColorIndex = quadMeshSystem.ConvertGirdPosToIndex(newColorGrid);
        }

        var gridPos = new int2(x, y);
        var data = new QuadData
        {
          ShapeIndex = shapeIndex,
          Position = slotGrid.ConvertGridPosToWorldPos(gridPos),
          ColorValue = newColorIndex,
        };
        _quadDatas[i] = data;
        _shapeCenterOffsets[i] = data.Position - shapeData.CenterPosition;
        i++;
      }
    }
  }

  int FindFirstInactivedShapeIdx()
  {
    using var kvArray = _blockShapeDatas.GetKeyValueArrays(Allocator.Temp);
    for (int i = slotsParent.childCount; i < kvArray.Length; ++i)
    {
      var key = kvArray.Keys[i];
      var val = kvArray.Values[i];
      if (val.IsActive) continue;
      return key;
    }
    return -1;
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
    var additionAmount = blockSlotPositions.Length * 64;
    var startSpawnedQuadIndex = _currentQuadAmount;

    var slotPos = GetAndSetSlotGridPositionAt(slotIndex);

    var shapeIdx = slotIndex;
    if (!_blockShapeDatas.ContainsKey(shapeIdx))
      _blockShapeDatas.Add(
        shapeIdx,
        new BlockShapeData
        {
          CenterPosition = slotPos,
          StartSpawnedQuadIndex = startSpawnedQuadIndex,
          QuadsAmount = additionAmount,
          ColorValue = colorValue,
          IsActive = true
        }
      );

    var foundShapeIdx = FindFirstInactivedShapeIdx();
    if (foundShapeIdx != -1)
    {
      startSpawnedQuadIndex = _blockShapeDatas[foundShapeIdx].StartSpawnedQuadIndex;
      additionAmount = 0;

      var shapeData = _blockShapeDatas[shapeIdx];
      shapeData.StartSpawnedQuadIndex = startSpawnedQuadIndex;
      shapeData.QuadsAmount = _blockShapeDatas[foundShapeIdx].QuadsAmount;
      shapeData.IsActive = true;
      _blockShapeDatas[shapeIdx] = shapeData;
      _blockShapeDatas.Remove(foundShapeIdx);
    }

    for (int i = 0; i < blockSlotPositions.Length; ++i)
    {
      var blockSlotPos = blockSlotPositions[i];
      OrderUnitBlockAt(startSpawnedQuadIndex + i * 64, blockSlotPos, shapeIdx, colorValue);

      var blockPos = ConvertSlotPosToWorldPos(blockSlotPos);
      var blockData = new BlockData
      {
        ShapeIndex = shapeIdx,
        Position = blockPos,
        CenterOffset = blockPos - slotPos,
      };
      _blockDatas[_currentBlockAmount + i] = blockData;
    }

    var availableAmount = GetAvailableQuadAmount(additionAmount);
    _currentQuadAmount = availableAmount;
  }

  void OrderShapePositionsTo(float3 targetPos, int shapeIdx)
  {
    var shapeData = _blockShapeDatas[shapeIdx];
    shapeData.CenterPosition = targetPos;
    _blockShapeDatas[shapeIdx] = shapeData;

    var _currentBlockAmount = GetCurrentBlockAmount();
    for (int i = 0; i < _currentBlockAmount; ++i)
    {
      var blockData = _blockDatas[i];
      if (blockData.ShapeIndex != shapeIdx) continue;

      var offset = blockData.CenterOffset;
      var nextBlockPos = shapeData.CenterPosition + offset;
      blockData.Position = nextBlockPos;

      _blockDatas[i] = blockData;
    }

    for (int i = 0; i < _currentQuadAmount; ++i)
    {
      var quadData = _quadDatas[i];
      if (quadData.ShapeIndex != shapeIdx) continue;

      var offset = _shapeCenterOffsets[i];
      var nextQuadPos = shapeData.CenterPosition + offset;
      quadData.Position = nextQuadPos;

      _quadDatas[i] = quadData;
      OrderQuadMeshAt(i, nextQuadPos, quadData.ColorValue);
    }
  }

  void GrabbingBlockControlInUpdate()
  {
    if (_currentGrabbingShapeIndex == -1) return;
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
    if (!_isUserScreenTouching) return;
    if (!_blockShapeDatas.ContainsKey(_currentGrabbingShapeIndex)) return;

    var _userTouchScreenPos = new float3(
      userTouchScreenPosition.x, userTouchScreenPosition.y, 0
    );
    var targetPos = _userTouchScreenPos + touchOffset;
    OrderShapePositionsTo(targetPos, _currentGrabbingShapeIndex);
    ApplyDrawOrders();
  }

  void CalculateGravityForQuadsInUpdate()
  {
    var uniformVelocity = 4.5f * Time.deltaTime * new float3(0, -1, 0);

    for (int i = 0; i < _currentQuadAmount; ++i)
    {
      var quadData = _quadDatas[i];

      if (!_blockShapeDatas.ContainsKey(quadData.ShapeIndex)) continue;
      if (!_blockShapeDatas[quadData.ShapeIndex].IsActive) continue;
      if (IsSlotIndex(quadData.ShapeIndex)) continue;

      var currQuadPos = quadData.Position;
      var nextQuadPos = currQuadPos + uniformVelocity;
      if (quadGrid.IsPosOutsideAt(nextQuadPos))
      {
        var shapeData = _blockShapeDatas[quadData.ShapeIndex];
        shapeData.IsActive = false;
        _blockShapeDatas[quadData.ShapeIndex] = shapeData;
        continue;
      }

      quadData.Position = nextQuadPos;
      _quadDatas[i] = quadData;
      OrderQuadMeshAt(i, nextQuadPos, quadData.ColorValue);
    }

    ApplyDrawOrders();
  }
}