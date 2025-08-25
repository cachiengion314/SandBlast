using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  void VisualizeActiveQuads()
  {
    for (int i = 0; i < _currentQuadAmount; ++i)
    {
      var quadData = _quadDatas[i];
      if (!_groupQuadDatas.ContainsKey(quadData.GroupIndex)) continue;
      if (!_groupQuadDatas[quadData.GroupIndex].IsActive) continue;

      var pos = quadData.Position;
      OrderQuadMeshAt(i, pos, quadData.ColorValue);
    }
  }
}