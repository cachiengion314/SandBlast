using DG.Tweening;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
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

  void VisualizeRemoveQuads(NativeHashMap<int, bool> quadsMap, ref Sequence seq, ref float atPosition)
  {
    if (quadsMap.Count == 0) return;
    var duration = Time.fixedDeltaTime;
    using var quadDatas = quadsMap.GetKeyArray(Allocator.Temp);
    for (int i = 0; i < 2; i++)
    {
      for (int y = 0; y < quadMeshSystem.GridResolution.y; y++)
      {
        for (int j = 0; j < quadDatas.Length; j++)
        {
          var quadIdx = quadDatas[j];
          var colorValue = quadMeshSystem.ConvertGirdPosToIndex(new int2(3, y));
          seq.InsertCallback(atPosition, () =>
          {
            var quadData = _quadDatas[quadIdx];
            quadData.ColorValue = colorValue;
            _quadDatas[quadIdx] = quadData;
            OrderQuadMeshAt(quadData.Index, quadData.Position, quadData.ColorValue);
          });
        }
        atPosition += duration;
      }
    }
  }
}