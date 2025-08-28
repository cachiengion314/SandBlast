using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [SerializeField] Transform spawnedParent;
  [SerializeField] Transform redLine;

  Transform SpawnRedLine(float3 pos, float3 scale, Transform parent)
  {
    var obj = Instantiate(redLine, parent);
    obj.position = pos;
    obj.localScale = scale;
    return obj;
  }
}