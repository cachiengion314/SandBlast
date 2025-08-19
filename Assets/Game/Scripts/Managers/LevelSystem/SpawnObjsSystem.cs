using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [SerializeField] Transform spawnedParent;
  [SerializeField] Transform helicopterPref;
  [SerializeField] Transform slotBusPref;
  [SerializeField] Transform holeEfx;

  Transform SpawnHoleEfx(float3 pos, Transform parent)
  {
    var obj = Instantiate(holeEfx, parent);
    obj.position = pos;
    return obj;
  }

  Transform SpawnSlotBusAt(float3 pos, Transform parent)
  {
    var obj = Instantiate(slotBusPref, parent);
    obj.position = pos;
    return obj;
  }

  Transform SpawnHelicopter(float3 pos, Transform parent)
  {
    var obj = Instantiate(helicopterPref, parent);
    obj.position = pos;
    return obj;
  }
}