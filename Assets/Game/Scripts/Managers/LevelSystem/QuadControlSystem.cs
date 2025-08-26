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
    var uvGridPos = quadMeshSystem.ConvertIndexToGridPos(colorValue);
    quadMeshSystem.OrderQuadMeshAt(index, pos, uvGridPos);
  }

  void ApplyDrawOrders()
  {
    quadMeshSystem.ApplyDrawOrders();
  }

  int FindEmptyDownIndexAt(int2 gridPos)
  {
    var downGridPDirection = new int2(0, -1);
    var downGridPos = gridPos + downGridPDirection;
    if (GridSystem.IsGridPosOutsideAt(downGridPos, quadGridSize)) return -1;

    var downIndex = GridSystem.ConvertGridPosToIndex(downGridPos, quadGridSize);
    if (_quadIndexesDatas[downIndex] == -1) return downIndex;
    return -1;
  }

  int FindEmptyDiagonalIndexAt(int2 gridPos)
  {
    for (int i = 0; i < _diagonalDirections.Length; ++i)
    {
      var _direction = _diagonalDirections[i];
      var _diagonalGridPos = gridPos + _direction;
      if (GridSystem.IsGridPosOutsideAt(_diagonalGridPos, quadGridSize)) continue;

      var _diagonalIdx = GridSystem.ConvertGridPosToIndex(_diagonalGridPos, quadGridSize);
      if (_quadIndexesDatas[_diagonalIdx] == -1) return _diagonalIdx;
    }
    return -1;
  }

  float3 FindUnderEmptyQuadPosAt(float3 quadPos)
  {
    var gridPos = FindUnderEmptyQuadGridPosAt(quadPos);
    if (gridPos.Equals(-1)) return -1;
    return GridSystem.ConvertGridPosToWorldPos(gridPos, quadGridSize, quadGridScale, quadGridPos);
  }

  int2 FindUnderEmptyQuadGridPosAt(float3 quadPos)
  {
    var gridPos = GridSystem.ConvertWorldPosToGridPos(quadPos, quadGridSize, quadGridScale, quadGridPos);
    var x = gridPos.x;
    for (int y = 0; y < gridPos.y; ++y)
    {
      var _currGridPos = new int2(x, y);
      var _currIdx = GridSystem.ConvertGridPosToIndex(_currGridPos, quadGridSize);
      if (_quadIndexesDatas[_currIdx] == -1) return _currGridPos;
    }
    return -1;
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
      if (GridSystem.IsPosOutsideAt(blockPos, blockGridSize, blockGridScale, blockGridPos)) return true;
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
    var slotPos = slotGridPos;
    var blockPos = slotPos + new float3(
      blockSlotPos.x * quadGridScale.x * 8,
      blockSlotPos.y * quadGridScale.y * 8,
      0
    );
    return blockPos;
  }

  int2 ConvertBlockPosToSlotGridPos(float2 blockSlotPos)
  {
    var centerSlot = new int2(
      (int)math.floor(slotGridSize.x / 2f),
      (int)math.floor(slotGridSize.y / 2f)
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
          Position = GridSystem.ConvertGridPosToWorldPos(gridPos, slotGridSize, slotGridScale, slotGridPos),
          PlacedIndex = -1,
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

  void GenerateEmptyShape(
    int givenShapeIdx,
    float3 slotPos,
    int colorValue,
    ref int startSpawnedQuadIndex,
    ref int additionAmount
  )
  {
    if (!_blockShapeDatas.ContainsKey(givenShapeIdx))
      _blockShapeDatas.Add(
        givenShapeIdx,
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

      var shapeData = _blockShapeDatas[givenShapeIdx];
      shapeData.StartSpawnedQuadIndex = startSpawnedQuadIndex;
      shapeData.QuadsAmount = _blockShapeDatas[foundShapeIdx].QuadsAmount;
      shapeData.IsActive = true;
      _blockShapeDatas[givenShapeIdx] = shapeData;
      _blockShapeDatas.Remove(foundShapeIdx);
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
    var additionAmount = blockSlotPositions.Length * 64;
    var startSpawnedQuadIndex = _currentQuadAmount;

    var slotPos = GetAndSetSlotGridPositionAt(slotIndex);

    var shapeIdx = slotIndex;
    GenerateEmptyShape(
      shapeIdx, slotPos, colorValue, ref startSpawnedQuadIndex, ref additionAmount
    );

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

  void CalculateTransitionForQuadsInUpdate()
  {
    for (int x = 0; x < quadGridSize.x; ++x)
    {
      for (int y = 0; y < quadGridSize.y; ++y)
      {
        var currQuadGridPos = new int2(x, y);
        var currIdxPos = GridSystem.ConvertGridPosToIndex(currQuadGridPos, quadGridSize);
        if (_quadIndexesDatas[currIdxPos] == -1)
        {
          // case: there is no quad in this grid so we skip this one
          continue;
        }
        // case: there is a quad in this grid
        var quadIndex = _quadIndexesDatas[currIdxPos];
        var quadData = _quadDatas[quadIndex];
        var downIdx = FindEmptyDownIndexAt(currQuadGridPos);
        if (downIdx != -1)
        {
          // case: there is a empty down here so we priority move the quad to here
          _quadIndexesDatas[currIdxPos] = -1;
          var _downQuadPos = GridSystem.ConvertIndexToWorldPos(downIdx, quadGridSize, quadGridScale, quadGridPos);
          _quadIndexesDatas[downIdx] = quadIndex;
          quadData.Position = _downQuadPos;
          quadData.PlacedIndex = downIdx;
          _quadDatas[quadIndex] = quadData;
          OrderQuadMeshAt(quadIndex, _downQuadPos, quadData.ColorValue);
          continue;
        }
        var diagonalIdx = FindEmptyDiagonalIndexAt(currQuadGridPos);
        if (diagonalIdx == -1) continue;
        // case: there is a empty diagonal here so we move the quad to here
        _quadIndexesDatas[currIdxPos] = -1;
        var _diagonalQuadPos = GridSystem.ConvertIndexToWorldPos(diagonalIdx, quadGridSize, quadGridScale, quadGridPos);
        _quadIndexesDatas[diagonalIdx] = quadIndex;
        quadData.Position = _diagonalQuadPos;
        quadData.PlacedIndex = diagonalIdx;
        _quadDatas[quadIndex] = quadData;
        OrderQuadMeshAt(quadIndex, _diagonalQuadPos, quadData.ColorValue);
      }
    }
  }

  void CalculateGravityForQuadsInUpdate()
  {
    for (int i = 0; i < _currentQuadAmount; ++i)
    {
      var quadData = _quadDatas[i];

      if (!_blockShapeDatas.ContainsKey(quadData.ShapeIndex)) continue;
      if (!_blockShapeDatas[quadData.ShapeIndex].IsActive) continue;
      if (!IsPlacedShape(quadData.ShapeIndex)) continue;
      if (quadData.PlacedIndex != -1) continue;

      // TODO: heavyly calculations
      if (GridSystem.IsPosOutsideAt(quadData.Position, quadGridSize, quadGridScale, quadGridPos)) continue;
      var currQuadPos = quadData.Position;
      var underEmptyPos = FindUnderEmptyQuadPosAt(currQuadPos);
      var underEmptyIdx = GridSystem.ConvertWorldPosToIndex(underEmptyPos, quadGridSize, quadGridScale, quadGridPos);
      if (underEmptyPos.Equals(-1)) { continue; }

      _quadIndexesDatas[underEmptyIdx] = i;
      quadData.Position = underEmptyPos;
      quadData.PlacedIndex = underEmptyIdx;
      _quadDatas[i] = quadData;

      var shapeData = _blockShapeDatas[quadData.ShapeIndex];
      shapeData.QuadsAmount--;
      _blockShapeDatas[quadData.ShapeIndex] = shapeData;

      if (shapeData.QuadsAmount == 0)
      {
        _blockShapeDatas.Remove(quadData.ShapeIndex);
      }

      OrderQuadMeshAt(i, underEmptyPos, quadData.ColorValue);
    }

    CalculateTransitionForQuadsInUpdate();

    ApplyDrawOrders();
  }
}