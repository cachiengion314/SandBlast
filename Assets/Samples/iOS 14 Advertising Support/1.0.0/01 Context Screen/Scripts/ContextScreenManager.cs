using Unity.Advertisement.IosSupport.Components;
using UnityEngine;
using System.Collections;
using System;

namespace Unity.Advertisement.IosSupport.Samples
{
  /// <summary>
  /// This component will trigger the context screen to appear when the scene starts,
  /// if the user hasn't already responded to the iOS tracking dialog.
  /// </summary>
  public class ContextScreenManager : MonoBehaviour
  {
    /// <summary>
    /// The prefab that will be instantiated by this component.
    /// The prefab has to have an ContextScreenView component on its root GameObject.
    /// </summary>
    ///

    public static Action onCompletedAT;

    public ContextScreenView contextScreenPrefab;

    private float elapsedTime = 0;
    private float timeoutDuration = 5;

    void Start()
    {
      if (elapsedTime == timeoutDuration) { }

#if UNITY_IOS && !UNITY_EDITOR
      if (Application.platform == RuntimePlatform.IPhonePlayer && !SystemInfo.deviceModel.Contains("Mac"))
      {
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

        if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
          ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
      }
      // // check with iOS to see if the user has accepted or declined tracking
#endif

      StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
#if UNITY_EDITOR
      if (elapsedTime == timeoutDuration) { }
#endif

#if UNITY_IOS && !UNITY_EDITOR
      if (Application.platform == RuntimePlatform.IPhonePlayer && !SystemInfo.deviceModel.Contains("Mac"))
      {
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

        while (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
          status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

          elapsedTime += Time.deltaTime;

          // Kiểm tra nếu đã vượt quá thời gian timeout
          if (elapsedTime > timeoutDuration)
          {
            Debug.Log("Timeout reached, exiting the loop.");
            break; // Thoát vòng lặp nếu quá thời gian
          }
          yield return null;
        }
      }

      var statusATT = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

      if (statusATT == ATTrackingStatusBinding.AuthorizationTrackingStatus.DENIED || statusATT == ATTrackingStatusBinding.AuthorizationTrackingStatus.RESTRICTED)
      {
        PlayerPrefs.SetInt("IS_ACCEPT_UMP", 2);
      }
#endif

      ShowConsentUMP.Instance.Init();
      onCompletedAT?.Invoke();
      yield return null;
    }
  }
}