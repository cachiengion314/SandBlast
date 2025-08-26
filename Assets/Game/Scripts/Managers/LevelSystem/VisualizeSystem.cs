using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  void VisualizeActiveQuads()
  {
    for (int i = 0; i < _quadDatas.Length; ++i)
    {
      var quadData = _quadDatas[i];
      if (!quadData.IsActive) continue;
      // if (!_shapeQuadDatas.ContainsKey(quadData.GroupIndex)) continue;

      var pos = quadData.Position;
      OrderQuadMeshAt(i, pos, quadData.ColorValue);
    }
  }
}