using Unity.Mathematics;
using UnityEngine;

public struct AnimatedData
{
  public Transform targetObj;
  public Transform needMovingObj;
  public int fromIndex;
  public float3 fromPos;
  public float3 targetPos;
  public int targetIndex;
  public Transform targetPosParent;
  public float startDuration;
  public float lengthDuration;
}

public partial class LevelSystem : MonoBehaviour
{
  void VisualizeActiveQuads()
  {
    for (int i = 0; i < _currentQuadAmount; ++i)
    {
      var data = _quadDatas[i];
      var isActive = data.IsActive;
      if (!isActive) continue;

      var pos = data.Position;
      quadMeshSystem.OrderQuadMeshAt(i, pos, -1, -1);
    }
    quadMeshSystem.ApplyDrawOrders();
  }
}