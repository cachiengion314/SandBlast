using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  int _currentQuadAmount;

  int GetAvailableQuadAmount(int _additionAmount = 0)
  {
    var availableAmount
      = math.min(_currentQuadAmount + _additionAmount, quadMeshSystem.QuadCapacity);
    return availableAmount;
  }

  void SpawnAdditionQuads(int additionAmount, int colorValue)
  {
    var availableAmount = GetAvailableQuadAmount(additionAmount);
    var groupIdx = 0;
    if (!_groupQuadDatas.ContainsKey(groupIdx))
      _groupQuadDatas.Add(
        groupIdx,
        new GroupQuadsData
        {
          CenterPosition = 0,
          ColorValue = colorValue
        }
      );
    var groupData = _groupQuadDatas[groupIdx];

    for (int i = _currentQuadAmount; i < availableAmount; ++i)
    {
      var data = new QuadData
      {
        GroupIndex = groupIdx,
        Position = boardGrid.ConvertIndexToWorldPos(i),
        ColorValue = colorValue,
        IsActive = true,
      };
      _quadDatas[i] = data;
      _quadCenterOffsets[i] = data.Position - groupData.CenterPosition;
    }
    _currentQuadAmount = availableAmount;
  }

  void ControlQuadsInUpdate()
  {
    if (!_isUserScreenTouching) return;

    foreach (var keyval in _groupQuadDatas)
    {
      // print("keyval.Key " + keyval.Key);
      // continue;
      // var _userTouchScreenPosition = new float3(
      //   userTouchScreenPosition.x, userTouchScreenPosition.y, 0
      // );
      // var groupData = keyval.Value;
      // var groupIdx = keyval.Key;
      // groupData.CenterPosition = _userTouchScreenPosition;
      // _groupQuadDatas[groupIdx] = groupData;
    }

    var _userTouchScreenPosition = new float3(
        userTouchScreenPosition.x, userTouchScreenPosition.y, 0
      );
    var _groupData = _groupQuadDatas[0];
    var _groupIdx = 0;
    _groupData.CenterPosition = _userTouchScreenPosition;
    _groupQuadDatas[_groupIdx] = _groupData;

    for (int i = 0; i < _currentQuadAmount; ++i)
    {
      var quadData = _quadDatas[i];
      var isActive = quadData.IsActive;
      if (!isActive) continue;

      var offset = _quadCenterOffsets[i];
      var groupIdx = quadData.GroupIndex;

      var groupData = _groupQuadDatas[groupIdx];

      var nextQuadPos = groupData.CenterPosition + offset;
      quadData.Position = nextQuadPos;

      _quadDatas[i] = quadData;
      quadMeshSystem.OrderQuadMeshAt(i, nextQuadPos, -1, -1);
    }

    quadMeshSystem.ApplyDrawOrders();
  }
}