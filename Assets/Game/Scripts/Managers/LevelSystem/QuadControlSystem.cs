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
    for (int i = 0; i < _quadDatas.Length; ++i)
    {
      var quadData = _quadDatas[i];
      if (quadData.GroupIndex != oldShapeIdx) continue;
      quadData.GroupIndex = newShapeIdx;
      _quadDatas[i] = quadData;
    }
    _shapeQuadDatas.Remove(oldShapeIdx);
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

  int FindInactiveBlockIdxForShape()
  {
    for (int i = 0; i < _blockDatas.Length; ++i)
    {
      var blockData = _blockDatas[i];
      if (blockData.ShapeIndex != -1) continue;
      return blockData.Index;
    }
    return -1;
  }

  NativeList<QuadData> FindInactiveQuadsForShape()
  {
    var list = new NativeList<QuadData>(64 * 4, Allocator.Temp); ;
    for (int i = 0; i < _quadDatas.Length; ++i)
    {
      var quadData = _quadDatas[i];
      if (quadData.IsActive) continue;
      list.Add(quadData);
      if (list.Length == list.Capacity) break;
    }
    if (list.Length < list.Capacity) return new NativeList<QuadData>(0, Allocator.Temp);
    return list;
  }

  void OrderUnitBlockAt(
    int startIndex,
    float2 blockSlotPos,
    int shapeIndex,
    int colorValue,
    NativeList<QuadData> inactiveQuads
  )
  {
    var shapeData = _shapeQuadDatas[shapeIndex];

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
        var pos = slotGrid.ConvertGridPosToWorldPos(gridPos);

        var quadData = inactiveQuads[i];
        quadData.GroupIndex = shapeIndex;
        quadData.Position = pos;
        quadData.PlacedIndex = -1;
        quadData.ColorValue = newColorIndex;
        quadData.IsActive = true;
        var _i = quadData.Index;

        _quadDatas[_i] = quadData;
        _shapeCenterOffsets[_i] = quadData.Position - shapeData.CenterPosition;
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
    var shapeIdx = slotIndex;
    if (_shapeQuadDatas.ContainsKey(shapeIdx))
    {
      print("Shape ID still exist, there is something wrong!");
      return;
    }

    var slotPos = GetAndSetSlotGridPositionAt(slotIndex);

    using var inactiveQuads = FindInactiveQuadsForShape();
    if (inactiveQuads.Length == 0)
    {
      print("Cannot find any spare quads");
      return;
    }

    var additionAmount = 4 * 64;
    var startSpawnedQuadIndex = 0;

    _shapeQuadDatas.Add(
      shapeIdx,
      new ShapeQuadData
      {
        CenterPosition = slotPos,
        QuadsAmount = additionAmount,
        ColorValue = colorValue,
      }
    );

    for (int i = 0; i < blockSlotPositions.Length; ++i)
    {
      var blockSlotPos = blockSlotPositions[i];
      OrderUnitBlockAt(
        startSpawnedQuadIndex + i * 64,
        blockSlotPos,
        shapeIdx,
        colorValue,
        inactiveQuads
      );

      var blockPos = ConvertSlotPosToWorldPos(blockSlotPos);
      var blockIdx = FindInactiveBlockIdxForShape();
      if (blockIdx == -1)
      {
        print("Cannot find any blockIdx ");
        return;
      }
      print("blockIdx " + blockIdx);
      var blockData = _blockDatas[blockIdx];
      blockData.ShapeIndex = shapeIdx;
      blockData.Position = blockPos;
      blockData.CenterOffset = blockPos - slotPos;

      _blockDatas[blockIdx] = blockData;
    }
  }
}