using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [SerializeField] Transform spawnedParent;
  [SerializeField] Transform holeEfx;

  Transform SpawnHoleEfx(float3 pos, Transform parent)
  {
    var obj = Instantiate(holeEfx, parent);
    obj.position = pos;
    return obj;
  }
}