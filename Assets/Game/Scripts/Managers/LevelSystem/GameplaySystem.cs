using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  void OrderShapePositionsTo(float3 targetPos, int shapeIdx)
  {
    var shapeData = _shapeQuadDatas[shapeIdx];
    shapeData.CenterPosition = targetPos;
    _shapeQuadDatas[shapeIdx] = shapeData;

    for (int i = 0; i < _blockDatas.Length; ++i)
    {
      var blockData = _blockDatas[i];
      if (blockData.ShapeIndex != shapeIdx) continue;

      var offset = blockData.CenterOffset;
      var nextBlockPos = shapeData.CenterPosition + offset;
      blockData.Position = nextBlockPos;

      _blockDatas[i] = blockData;
    }

    for (int i = 0; i < _quadDatas.Length; ++i)
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
    if (!_shapeQuadDatas.ContainsKey(_currentGrabbingShapeIndex)) return;

    var _userTouchScreenPos = new float3(
      userTouchScreenPosition.x, userTouchScreenPosition.y, 0
    );
    var targetPos = _userTouchScreenPos + touchOffset;
    OrderShapePositionsTo(targetPos, _currentGrabbingShapeIndex);
  }

  void SnapQuadToGridInUpdate()
  {
    for (int i = 0; i < _quadDatas.Length; ++i)
    {
      var quadData = _quadDatas[i];
      if (!quadData.IsActive) continue;
      if (!_groupQuadDatas.ContainsKey(quadData.GroupIndex)) continue;
      if (quadData.PlacedIndex != -1) continue;

      if (quadGrid.IsPosOutsideAt(quadData.Position)) continue;
      var currQuadPos = quadData.Position;
      var currIdx = quadGrid.ConvertWorldPosToIndex(currQuadPos);

      _quadIndexesDatas[currIdx] = i;
      quadData.PlacedIndex = currIdx;
      _quadDatas[i] = quadData;

      OrderQuadMeshAt(i, currQuadPos, quadData.ColorValue);
    }
  }

  void CalculateQuadFallingInUpdate()
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
          // case: there is an empty down here so we priority move the quad to here
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
        // case: there is an empty diagonal here so we move the quad to here
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
}