using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
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
      if (quadData.GroupIndex != shapeIdx) continue;

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

  void CalculateQuadTransitionsInUpdate()
  {
    for (int x = 0; x < quadGrid.GridSize.x; ++x)
    {
      for (int y = 0; y < quadGrid.GridSize.y; ++y)
      {
        var currQuadGridPos = new int2(x, y);
        var currIdxPos = quadGrid.ConvertGridPosToIndex(currQuadGridPos);
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
          var _downQuadPos = quadGrid.ConvertIndexToWorldPos(downIdx);
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
        var _diagonalQuadPos = quadGrid.ConvertIndexToWorldPos(diagonalIdx);
        _quadIndexesDatas[diagonalIdx] = quadIndex;
        quadData.Position = _diagonalQuadPos;
        quadData.PlacedIndex = diagonalIdx;
        _quadDatas[quadIndex] = quadData;
        OrderQuadMeshAt(quadIndex, _diagonalQuadPos, quadData.ColorValue);
      }
    }
  }

  void CalculateQuadFallingsInUpdate()
  {
    if (_blockShapeDatas.Count == 0) return;

    for (int i = 0; i < _currentQuadAmount; ++i)
    {
      var quadData = _quadDatas[i];

      if (!_blockShapeDatas.ContainsKey(quadData.GroupIndex)) continue;
      if (!_blockShapeDatas[quadData.GroupIndex].IsActive) continue;
      if (!IsPlacedShape(quadData.GroupIndex)) continue;
      if (quadData.PlacedIndex != -1) continue;

      // TODO: heavyly calculations
      if (quadGrid.IsPosOutsideAt(quadData.Position)) continue;
      var currQuadPos = quadData.Position;
      var underEmptyPos = FindUnderEmptyQuadPosAt(currQuadPos);
      var underEmptyIdx = quadGrid.ConvertWorldPosToIndex(underEmptyPos);
      if (underEmptyPos.Equals(-1)) { continue; }

      _quadIndexesDatas[underEmptyIdx] = i;
      quadData.Position = underEmptyPos;
      quadData.PlacedIndex = underEmptyIdx;
      _quadDatas[i] = quadData;

      var shapeData = _blockShapeDatas[quadData.GroupIndex];
      shapeData.QuadsAmount--;
      _blockShapeDatas[quadData.GroupIndex] = shapeData;

      if (shapeData.QuadsAmount == 0)
      {
        _blockShapeDatas.Remove(quadData.GroupIndex);
      }

      OrderQuadMeshAt(i, underEmptyPos, quadData.ColorValue);
    }
  }
}