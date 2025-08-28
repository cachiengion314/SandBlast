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
    int2 leftGridPos,
    int2 rightGridPos,
    out NativeHashMap<int, bool> linkedQuads)
  {
    var leftIdxPos = quadGrid.ConvertGridPosToIndex(leftGridPos);
    linkedQuads = CollectLinkedQuadsAt(leftIdxPos);

    var rightQuadIdx = GetQuadIdxFrom(rightGridPos);
    if (linkedQuads.ContainsKey(rightQuadIdx)) return true;
    return false;
  }

  /// <summary>
  /// Return a map that contain delegate quad's datas that have separated by colors at x
  /// </summary>
  /// <param name="column"></param>
  NativeHashMap<int, QuadData> CollectDistinguishQuadDataAt(int column)
  {
    var map = new NativeHashMap<int, QuadData>(32, Allocator.Temp);

    var x = column;
    for (int y = 1; y < quadGrid.GridSize.y; ++y)
    {
      var currGridPos = new int2(x, y);
      var currQuadIdx = GetQuadIdxFrom(currGridPos);
      var preGridPos = new int2(x, y - 1);
      var preQuadIdx = GetQuadIdxFrom(preGridPos);

      if (currQuadIdx == -1) break;
      if (preQuadIdx == -1) continue;

      // var preColorValue = GetCO
      var currQuadData = _quadDatas[currQuadIdx];
      var currIdxPos = quadGrid.ConvertGridPosToIndex(currGridPos);
      var currColorValue = GetQuadGroupColorFrom(currQuadData);


    }

    return map;
  }

  NativeArray<int2> FindMatchingPairQuads()
  {
    var xLeft = 0;
    var xRight = quadGrid.GridSize.x - 1;

    var arr = new NativeArray<int2>(2, Allocator.Temp);






    return new NativeArray<int2>(0, Allocator.Temp);
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

  bool IsBlockShapeOutsideAt(int shapeIdx)
  {
    for (int i = 0; i < _blockDatas.Length; ++i)
    {
      var blockData = _blockDatas[i];
      var _shapeIdx = blockData.ShapeIndex;
      if (_shapeIdx != shapeIdx) continue;

      var blockPos = blockData.Position;
      if (blockGrid.IsPosOutsideAt(blockPos)) return true;
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

      var blockData = _blockDatas[blockIdx];
      blockData.ShapeIndex = shapeIdx;
      blockData.Position = blockPos;
      blockData.CenterOffset = blockPos - slotPos;

      _blockDatas[blockIdx] = blockData;
    }
  }
}