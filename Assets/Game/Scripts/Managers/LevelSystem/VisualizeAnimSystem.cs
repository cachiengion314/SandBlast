using System.Collections.Generic;
using DG.Tweening;
using Firebase.Analytics;
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

}