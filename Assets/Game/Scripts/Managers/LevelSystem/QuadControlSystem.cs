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
        new GroupOfQuadsData
        {
          CenterPosition = 0,
          ColorValue = colorValue
        }
      );
    for (int i = _currentQuadAmount; i < availableAmount; ++i)
    {
      var data = new QuadData
      {
        Position = boardGrid.ConvertIndexToWorldPos(i),
        GroupIndex = groupIdx,
        IsActive = true,
        ColorValue = colorValue,
      };
      _quadDatas[i] = data;
    }
    _currentQuadAmount = availableAmount;
  }

  void ControlQuadsInUpdate()
  {
    if (!_isUserScreenTouching) return;

    // var availableAmount = GetAvailableQuadAmount();

    // for (int i = 0; i < availableAmount; ++i)
    // {
    //   var isActive = _quadDatas[i];
    //   if (!isActive) continue;
    //   var pos = _quadPositions[i];

    //   var _userScreenPosition = new float3(userScreenPosition.x, userScreenPosition.y, 0);
    //   var _pos = pos - _userScreenPosition;

    //   quadMeshSystem.OrderQuadMeshAt(i, pos, -1, -1);
    //   _quadPositions[i] = pos;
    // }

    // if (Input.GetKey(KeyCode.Space))
    // {
    //   if (currentQuadAmount < quadMeshSystem.QuadCapacity)
    //     currentQuadAmount++;
    //   _quadActives[currentQuadAmount - 1] = true;
    // }

    // quadMeshSystem.ApplyDrawOrders();
  }
}