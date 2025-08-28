using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [Range(1, 16)]
  [SerializeField] int searchingDeepAmount = 1;
  [SerializeField] int redLineRow = 79;
  bool isQuadFalling = false;

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
      if (quadData.IndexPosition != -1) continue;

      if (quadGrid.IsPosOutsideAt(quadData.Position)) continue;
      var currQuadPos = quadData.Position;
      var currIdxPos = quadGrid.ConvertWorldPosToIndex(currQuadPos);

      _quadIndexPositionDatas[currIdxPos] = i;
      quadData.IndexPosition = currIdxPos;
      _quadDatas[i] = quadData;

      OrderQuadMeshAt(i, currQuadPos, quadData.ColorValue);
    }
  }

  void CalculateQuadFallingInUpdate()
  {
    isQuadFalling = false;
    for (int x = 0; x < quadGrid.GridSize.x; ++x)
    {
      for (int y = 0; y < quadGrid.GridSize.y; ++y)
      {
        var currQuadGridPos = new int2(x, y);
        var currIdxPos = quadGrid.ConvertGridPosToIndex(currQuadGridPos);
        if (_quadIndexPositionDatas[currIdxPos] == -1)
        {
          // case: there is no quad in this grid so we skip this one
          continue;
        }
        // case: there is a quad in this grid
        var quadIndex = _quadIndexPositionDatas[currIdxPos];
        var quadData = _quadDatas[quadIndex];
        var downIdxPos = FindEmptyDownIndexAt(currQuadGridPos);
        if (downIdxPos != -1)
        {
          // case: there is an empty down there so we priority move the quad to there
          _quadIndexPositionDatas[currIdxPos] = -1;
          var _downQuadPos = quadGrid.ConvertIndexToWorldPos(downIdxPos);
          _quadIndexPositionDatas[downIdxPos] = quadIndex;
          quadData.Position = _downQuadPos;
          quadData.IndexPosition = downIdxPos;
          _quadDatas[quadIndex] = quadData;
          isQuadFalling = true;

          OrderQuadMeshAt(quadIndex, _downQuadPos, quadData.ColorValue);
          continue;
        }
        var diagonalIdxPos = FindEmptyDiagonalIndexAt(currQuadGridPos);
        if (diagonalIdxPos == -1) continue;
        // case: there is an empty diagonal there so we move the quad to there
        _quadIndexPositionDatas[currIdxPos] = -1;
        var _diagonalQuadPos = quadGrid.ConvertIndexToWorldPos(diagonalIdxPos);
        _quadIndexPositionDatas[diagonalIdxPos] = quadIndex;
        quadData.Position = _diagonalQuadPos;
        quadData.IndexPosition = diagonalIdxPos;
        _quadDatas[quadIndex] = quadData;
        isQuadFalling = true;

        OrderQuadMeshAt(quadIndex, _diagonalQuadPos, quadData.ColorValue);
      }
    }
  }

  void AutoClearLinkedQuadsInUpdate()
  {
    // if (isQuadFalling) return;

    // using var linkedQuads = CollectLeftAndRightLinkedQuads();
    // if (linkedQuads.Count == 0) return;
    // RemoveQuadsFrom(linkedQuads);
  }

  void CheckLoseInUpdate()
  {
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
    if (isQuadFalling) return;
    for (int i = 0; i < quadGrid.GridSize.x; i++)
    {
      var idx = quadGrid.ConvertGridPosToIndex(new int2(i, redLineRow));
      if (_quadIndexPositionDatas[idx] == -1) continue;
      GameplayPanel.Instance.ToggleLevelFailedModal();
      GameManager.Instance.SetGameState(GameState.Gameover);
      break;
    }
  }
}