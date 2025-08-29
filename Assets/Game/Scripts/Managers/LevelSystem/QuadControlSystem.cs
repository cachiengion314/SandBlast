using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
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

  int GetQuadGroupColorFrom(QuadData quadData)
  {
    return _groupQuadDatas[quadData.GroupIndex].ColorValue;
  }

  int GetQuadIdxFrom(int2 gridPos)
  {
    var idxPos = quadGrid.ConvertGridPosToIndex(gridPos);
    return _quadIndexPositionDatas[idxPos];
  }

  bool IsPairQuadLinked(
    int leftIdxPos,
    int rightIdxPos,
    out NativeHashMap<int, bool> linkedQuads)
  {
    linkedQuads = CollectLinkedQuadsAt(leftIdxPos);
    var rightQuadIdx = _quadIndexPositionDatas[rightIdxPos];
    if (linkedQuads.ContainsKey(rightQuadIdx)) return true;
    return false;
  }

  NativeArray<int> CreateDistinguishColorsMap()
  {
    var distinguishColors = new NativeArray<int>(_quadIndexPositionDatas.Length, Allocator.Temp);
    var directions = new NativeArray<int2>(3, Allocator.Temp);
    directions[0] = new(0, 1);
    directions[1] = new(1, 1);
    directions[2] = new(1, 0);

    distinguishColors[0] = 1;
    for (int x = 0; x < quadGrid.GridSize.x; ++x)
    {
      for (int y = 0; y < quadGrid.GridSize.y; ++y)
      {
        var currGridPos = new int2(x, y);
        var currQuadIdx = GetQuadIdxFrom(currGridPos);
        if (currQuadIdx == -1) break;

        var currQuadData = _quadDatas[currQuadIdx];
        var currIdxPos = currQuadData.IndexPosition;
        var currColorValue = GetQuadGroupColorFrom(currQuadData);

        for (int i = 0; i < directions.Length; ++i)
        {
          var nextGridPos = currGridPos + directions[i];
          if (quadGrid.IsGridPosOutsideAt(nextGridPos)) continue;

          var nextQuadIdx = GetQuadIdxFrom(nextGridPos);
          if (nextQuadIdx == -1) continue;

          var nextQuadData = _quadDatas[nextQuadIdx];
          var nextIdxPos = nextQuadData.IndexPosition;
          var nextColorValue = GetQuadGroupColorFrom(nextQuadData);
          if (currColorValue == nextColorValue)
          {
            distinguishColors[nextIdxPos] = distinguishColors[currIdxPos];
            continue;
          }
          distinguishColors[nextIdxPos] = distinguishColors[currIdxPos] + 1;
        }
      }
    }
    directions.Dispose();
    return distinguishColors;
  }

  /// <summary>
  /// Return a list that contain delegate quad's datas that have separated 
  /// by colors at column x
  /// </summary>
  /// <param name="column"></param>
  NativeList<int> CollectDistinguishQuadIdxesAt(int column)
  {
    var list = new NativeList<int>(32, Allocator.Temp);
    var x = column;

    for (int y = 0; y < quadGrid.GridSize.y; ++y)
    {
      var currGridPos = new int2(x, y);
      var currQuadIdx = GetQuadIdxFrom(currGridPos);
      if (currQuadIdx == -1) break;

      var nextGridPos = new int2(x, y + 1);
      var nextQuadIdx = GetQuadIdxFrom(nextGridPos);
      if (nextQuadIdx == -1)
      {
        list.Add(currQuadIdx);
        break;
      }

      var currQuadData = _quadDatas[currQuadIdx];
      var currColorValue = GetQuadGroupColorFrom(currQuadData);
      var nextQuadData = _quadDatas[nextQuadIdx];
      var nexxtColorValue = GetQuadGroupColorFrom(nextQuadData);
      if (currColorValue != nexxtColorValue)
      {
        list.Add(currQuadIdx);
      }
    }
    return list;
  }

  NativeHashMap<int, bool> CollectLeftAndRightLinkedQuads()
  {
    var xLeft = 0;
    var xRight = quadGrid.GridSize.x - 1;
    var leftList = CollectDistinguishQuadIdxesAt(xLeft);
    var rightList = CollectDistinguishQuadIdxesAt(xRight);
    if (leftList.Length == 0 || rightList.Length == 0)
      return new NativeHashMap<int, bool>(0, Allocator.Temp);

    for (int i = 0; i < leftList.Length; ++i)
    {
      var leftQuadIdx = leftList[i];
      var leftQuadData = _quadDatas[leftQuadIdx];
      var leftColorValue = GetQuadGroupColorFrom(leftQuadData);
      for (int j = 0; j < rightList.Length; ++j)
      {
        var rightQuadIdx = rightList[j];
        var rightQuadData = _quadDatas[rightQuadIdx];
        var rightColorValue = GetQuadGroupColorFrom(rightQuadData);
        if (leftColorValue != rightColorValue) continue;

        if (
          !IsPairQuadLinked(
            leftQuadData.IndexPosition, rightQuadData.IndexPosition, out var linkedQuads
          )
        )
        {
          linkedQuads.Dispose();
          continue;
        }
        return linkedQuads;
      }
    }
    return new NativeHashMap<int, bool>(0, Allocator.Temp);
  }

  NativeList<int> FindNeighborQuadIdxesAround(QuadData quadData)
  {
    var list = new NativeList<int>(8, Allocator.Temp);
    var currColorValue = _groupQuadDatas[quadData.GroupIndex].ColorValue;
    var currGridPos = quadGrid.ConvertIndexToGridPos(quadData.IndexPosition);
    for (int j = 0; j < _fullDirections.Length; ++j)
    {
      var direction = _fullDirections[j];
      var nextGridPos = currGridPos + direction;
      if (quadGrid.IsGridPosOutsideAt(nextGridPos)) continue;

      var nextIndexPos = quadGrid.ConvertGridPosToIndex(nextGridPos);
      var nextQuadIdx = _quadIndexPositionDatas[nextIndexPos];
      if (nextQuadIdx == -1) continue; // nextGridPos is an empty grid position so we skip this

      var nextQuadData = _quadDatas[nextQuadIdx];
      var nextColorValue = _groupQuadDatas[nextQuadData.GroupIndex].ColorValue;
      if (nextColorValue != currColorValue) continue;
      list.Add(nextQuadIdx);
    }
    return list;
  }

  NativeHashMap<int, bool> CollectLinkedQuadsAt(int startIdxPos)
  {
    var visitedQuads = new NativeHashMap<int, bool>(
      quadMeshSystem.QuadCapacity, Allocator.Temp
    );
    var availableQuads = new NativeHashMap<int, bool>(
      quadMeshSystem.QuadCapacity, Allocator.Temp
    );

    var startQuadIdx = _quadIndexPositionDatas[startIdxPos];
    availableQuads.Add(startQuadIdx, true);

    while (availableQuads.Count > 0)
    {
      using var availableQuadsArray = availableQuads.GetKeyValueArrays(Allocator.Temp);
      var currentQuadIdx = availableQuadsArray.Keys[^1];
      var currentQuadData = _quadDatas[currentQuadIdx];

      availableQuads.Remove(currentQuadIdx);
      visitedQuads.Add(currentQuadIdx, true);

      using var neighbors = FindNeighborQuadIdxesAround(currentQuadData);
      for (int j = 0; j < neighbors.Length; ++j)
      {
        var neighborQuadIdx = neighbors[j];
        if (availableQuads.ContainsKey(neighborQuadIdx)) continue;
        if (visitedQuads.ContainsKey(neighborQuadIdx)) continue;
        availableQuads.Add(neighborQuadIdx, true);
      }
    }
    return visitedQuads;
  }

  NativeHashMap<int, bool> CollectLinkedQuadsMatch(int colorValue)
  {
    var quadHashMap = new NativeHashMap<int, bool>(
      quadMeshSystem.QuadCapacity, Allocator.Temp
    );
    for (int x = 0; x < quadGrid.GridSize.x; ++x)
    {
      for (int y = 0; y < quadGrid.GridSize.y; ++y)
      {
        var currGridPos = new int2(x, y);
        var currIdxPos = quadGrid.ConvertGridPosToIndex(currGridPos);
        var currQuadIdx = _quadIndexPositionDatas[currIdxPos];
        if (currQuadIdx == -1) continue;

        var quadData = _quadDatas[currQuadIdx];
        if (_groupQuadDatas[quadData.GroupIndex].ColorValue != colorValue) continue;
        if (quadHashMap.ContainsKey(currQuadIdx)) continue;

        quadHashMap.Add(currQuadIdx, true);
        using var list = FindNeighborQuadIdxesAround(quadData);
        for (int j = 0; j < list.Length; ++j)
        {
          var _quadIdx = list[j];
          if (quadHashMap.ContainsKey(_quadIdx)) continue;
          quadHashMap.Add(_quadIdx, true);
        }
      }
    }
    return quadHashMap;
  }

  NativeHashMap<int, bool> CollectLinkedQuadsRowAt(int from, int to)
  {
    var quadHashMap = new NativeHashMap<int, bool>(
      quadMeshSystem.QuadCapacity, Allocator.Temp
    );
    for (int x = 0; x < quadGrid.GridSize.x; ++x)
    {
      for (int y = from; y < to; ++y)
      {
        var currGridPos = new int2(x, y);
        var currIdxPos = quadGrid.ConvertGridPosToIndex(currGridPos);
        var currQuadIdx = _quadIndexPositionDatas[currIdxPos];
        if (currQuadIdx == -1) continue;

        if (quadHashMap.ContainsKey(currQuadIdx)) continue;
        quadHashMap.Add(currQuadIdx, true);
      }
    }
    return quadHashMap;
  }

  void RemoveQuadsFrom(NativeHashMap<int, bool> quadsMap)
  {
    using var quadsMapArray = quadsMap.GetKeyValueArrays(Allocator.Temp);
    for (int i = 0; i < quadsMapArray.Length; ++i)
    {
      var quadIdx = quadsMapArray.Keys[i];
      var quadData = _quadDatas[quadIdx];

      quadData.IsActive = false;
      _quadIndexPositionDatas[quadData.IndexPosition] = -1;
      _quadDatas[quadIdx] = quadData;

      OrderQuadMeshAt(quadIdx, -11, quadData.ColorValue);
    }
  }

  void RemoveAvailableBlock()
  {
    for (int i = _quadDatas.Length -1; i >= _quadDatas.Length - 64*4; i--)
    {
      var quadDataTemp = _quadDatas[i];
      quadDataTemp.IsActive = false;
      _quadDatas[i] = quadDataTemp;
      OrderQuadMeshAt(i, -11, 0);
    }
  }

  int FindEmptyIndexAt(int2 gridPos, int2 direction, int _searchingDeepAmount = 1)
  {
    var downGridPos = gridPos;
    var downIndex = -1;

    var searchingDeepsCount = 0;
    var chooseIndex = downIndex;
    while (searchingDeepsCount < _searchingDeepAmount)
    {
      downGridPos += direction;
      if (quadGrid.IsGridPosOutsideAt(downGridPos)) return chooseIndex;
      downIndex = quadGrid.ConvertGridPosToIndex(downGridPos);
      if (_quadIndexPositionDatas[downIndex] != -1) return chooseIndex;

      chooseIndex = downIndex;
      searchingDeepsCount++;
    }
    return chooseIndex;
  }

  int FindEmptyDownIndexAt(int2 gridPos)
  {
    var downGridPDirection = new int2(0, -1);
    return FindEmptyIndexAt(gridPos, downGridPDirection, searchingDeepAmount);
  }

  int FindEmptyDiagonalIndexAt(int2 gridPos)
  {
    for (int i = 0; i < 2; ++i)
    {
      var _direction = _fullDirections[i];
      var emptyIdx = FindEmptyIndexAt(gridPos, _direction, 1);
      if (emptyIdx != -1) return emptyIdx;
    }
    return -1;
  }

  void AssignQuadsToNewGroup(int newGroupIdx, int oldShapeIdx)
  {
    // change current shape of slot space to the group that belong to the board space
    var oldShapeData = _shapeQuadDatas[oldShapeIdx];

    var newGroupData = new GroupQuadData
    {
      QuadsAmount = oldShapeData.QuadsAmount,
      ColorValue = oldShapeData.ColorValue,
      IsActive = true,
    };
    _groupQuadDatas.Add(newGroupIdx, newGroupData);

    // remove blocks from shape since we don't need to read block_positions anymore
    for (int i = 0; i < _blockDatas.Length; ++i)
    {
      var blockData = _blockDatas[i];
      if (blockData.ShapeIndex != _currentGrabbingShapeIndex) continue;
      blockData.ShapeIndex = -1;
      _blockDatas[i] = blockData;
    }

    // re-assign group id for quads
    for (int i = 0; i < _quadDatas.Length; ++i)
    {
      var quadData = _quadDatas[i];
      if (quadData.GroupIndex != oldShapeIdx) continue;
      quadData.GroupIndex = newGroupIdx;
      _quadDatas[i] = quadData;
    }

    _shapeQuadDatas.Remove(oldShapeIdx);
  }

  bool IsQuadsInplaceableAt(int shapeIdx)
  {
    for (int i = 0; i < _quadDatas.Length; i++)
    {
      var quadData = _quadDatas[i];
      if (quadData.GroupIndex != shapeIdx) continue;
      if (quadGrid.IsPosOutsideAt(quadData.Position)) return true;
      var idx = quadGrid.ConvertWorldPosToIndex(quadData.Position);
      if (_quadIndexPositionDatas[idx] != -1) return true;
    }
    return false;
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

  NativeList<QuadData> FindInactiveQuadsForShape(int capacity)
  {
    var list = new NativeList<QuadData>(capacity, Allocator.Temp); ;
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
    for (int x = startX; x < startX + 8; ++x)
    {
      for (int y = startY; y < startY + 8; ++y)
      {
        var newColorIndex = colorValue;
        var ratio = UnityEngine.Random.Range(0f, 100f);
        if (ratio > 80f)
        {
          var xColor = UnityEngine.Random.Range(0, quadMeshSystem.GridResolution.x - 1);
          var newColorGrid = new int2(xColor, colorValue);
          newColorIndex = quadMeshSystem.ConvertGirdPosToIndex(newColorGrid);
        }

        var gridPos = new int2(x, y);
        var pos = slotGrid.ConvertGridPosToWorldPos(gridPos);

        var quadData = inactiveQuads[i];
        quadData.GroupIndex = shapeIndex;
        quadData.Position = pos;
        quadData.IndexPosition = -1;
        quadData.ColorValue = newColorIndex;
        quadData.IsActive = true;
        var index = quadData.Index;

        _quadDatas[index] = quadData;
        _shapeCenterOffsets[index] = quadData.Position - shapeData.CenterPosition;
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
      print("The shape with this ID still exist! Cannot create shape with this ID");
      return;
    }

    var slotPos = GetAndSetSlotGridPositionAt(slotIndex);

    using var inactiveQuads = FindInactiveQuadsForShape(64 * 4);
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

      var blockData = _blockDatas[blockIdx];
      blockData.ShapeIndex = shapeIdx;
      blockData.Position = blockPos;
      blockData.CenterOffset = blockPos - slotPos;

      _blockDatas[blockIdx] = blockData;
    }
  }
}