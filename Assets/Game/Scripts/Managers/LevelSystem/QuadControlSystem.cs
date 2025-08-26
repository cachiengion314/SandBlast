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
    if (quadGrid.IsGridPosOutsideAt(downGridPos)) return -1;

    var downIndex = quadGrid.ConvertGridPosToIndex(downGridPos);
    if (_quadIndexesDatas[downIndex] == -1) return downIndex;
    return -1;
  }

  int FindEmptyDiagonalIndexAt(int2 gridPos)
  {
    for (int i = 0; i < _diagonalDirections.Length; ++i)
    {
      var _direction = _diagonalDirections[i];
      var _diagonalGridPos = gridPos + _direction;
      if (quadGrid.IsGridPosOutsideAt(_diagonalGridPos)) continue;

      var _diagonalIdx = quadGrid.ConvertGridPosToIndex(_diagonalGridPos);
      if (_quadIndexesDatas[_diagonalIdx] == -1) return _diagonalIdx;
    }
    return -1;
  }

  float3 FindUnderEmptyQuadPosAt(float3 quadPos)
  {
    var gridPos = FindUnderEmptyQuadGridPosAt(quadPos);
    if (gridPos.Equals(-1)) return -1;
    return quadGrid.ConvertGridPosToWorldPos(gridPos);
  }

  int2 FindUnderEmptyQuadGridPosAt(float3 quadPos)
  {
    var gridPos = quadGrid.ConvertWorldPosToGridPos(quadPos);
    var x = gridPos.x;
    for (int y = 0; y < gridPos.y; ++y)
    {
      var _currGridPos = new int2(x, y);
      var _currIdx = quadGrid.ConvertGridPosToIndex(_currGridPos);
      if (_quadIndexesDatas[_currIdx] == -1) return _currGridPos;
    }
    return -1;
  }

  void AssignQuadsToNewShape(int newShapeIdx, int oldShapeIdx)
  {
    for (int i = 0; i < _currentQuadAmount; ++i)
    {
      var quadData = _quadDatas[i];
      if (quadData.GroupIndex != oldShapeIdx) continue;
      quadData.GroupIndex = newShapeIdx;
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

  NativeList<QuadData> FindInactiveQuadsForShape()
  {
    var list = new NativeList<QuadData>(64 * 4, Allocator.Temp); ;
    for (int i = 0; i < _quadDatas.Length; ++i)
    {
      var quadData = _quadDatas[i];
      if (quadData.Index != -1) continue;
      list.Add(quadData);
    }

    return list;
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

  void OrderUnitBlockAt(
    int startIndex,
    float2 blockSlotPos,
    int shapeIndex,
    int colorValue
  // NativeList<QuadData> inactiveQuads
  )
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
          GroupIndex = shapeIndex,
          Index = i,
          Position = slotGrid.ConvertGridPosToWorldPos(gridPos),
          PlacedIndex = -1,
          ColorValue = newColorIndex,
        };
        _quadDatas[i] = data;
        _shapeCenterOffsets[i] = data.Position - shapeData.CenterPosition;
        i++;
      }
    }
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
}