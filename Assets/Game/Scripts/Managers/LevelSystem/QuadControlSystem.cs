using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [SerializeField] QuadMeshSystem quadMeshSystem;
  [Range(1, 2000)]
  [SerializeField] int currentQuadAmount;
  Vector3 userScreenPosition;

  int GetAvailableQuadAmount(int _additionAmount = 0)
  {
    var availableAmount
      = math.min(currentQuadAmount + _additionAmount, quadMeshSystem.QuadCapacity);
    return availableAmount;
  }

  void SpawnQuadMeshes(int quadAmount)
  {
    // quadMeshSystem.InitComponents();

    // var availableAmount = GetAvailableQuadAmount(quadAmount);
    // for (int i = currentQuadAmount; i < availableAmount; ++i)
    //   _quadActives[i] = true;

    // for (int i = 0; i < amount; ++i)
    // {
    //   var isActive = _quadActives[i];
    //   if (!isActive) continue;

    //   var pos = _quadPositions[i];
    //   pos = boardGrid.ConvertIndexToWorldPos(i);
    //   _quadPositions[i] = pos;

    //   var groupIdx = 0;
    //   if (!_groupQuadDatas.ContainsKey(groupIdx))
    //     _groupQuadDatas.Add(0, new GroupQuadData
    //     {
    //       GroupIndex = groupIdx,
    //       Color = 1,
    //       CenterPosition = 0f
    //     });
    //   _quadGroups[i] = groupIdx;

    //   quadMeshSystem.OrderQuadMeshAt(i, pos, -1, -1);
    // }

    // quadMeshSystem.ApplyDrawOrders();
  }

  void DisposeQuadMeshSystem()
  {
    quadMeshSystem.DisposeComponents();
  }

  void ControlQuadsInUpdate()
  {
    // if (!_isUserScreenTouching) return;
    // var amount = GetAvailableQuadAmount();
    // for (int i = 0; i < amount; ++i)
    // {
    //   var isActive = _quadActives[i];
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