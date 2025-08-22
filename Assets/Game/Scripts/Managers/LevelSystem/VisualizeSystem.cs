using UnityEngine;

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
      OrderQuadMeshAt(i, pos, data.ColorValue);
    }
  }
}