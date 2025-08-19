using com.binouze;
using Unity.Services.LevelPlay;
using UnityEngine;

public class ShowConsentUMP : MonoBehaviour
{
  public static ShowConsentUMP Instance { get; private set; }

  private void Awake()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
  }

  void Start()
  {
    Init();
  }

  public void Init()
  {
    // These settings must be set before Initialisation call
    if (Debug.isDebugBuild)
    {
      // this one is false by default
      GoogleUserMessagingPlatform.SetDebugLogging(true);

      // Set here your device ID for testing, 
      // if not set, the device ID to put here will be shown in the console
#if UNITY_IOS
      GoogleUserMessagingPlatform.SetDebugMode("C6A12D00-758A-4CEC-99B8-3323609893BB", true);
#elif UNITY_ANDROID
      GoogleUserMessagingPlatform.SetDebugMode("XXXXXXXXX", true);
#endif
    }

    if (PlayerPrefs.GetInt("IS_ACCEPT_UMP") == 2)
    {
      LevelPlay.SetConsent(false);
      LevelPlayAds.Instance.Init();
      Debug.Log("---Init1");
      return;
    }

    if (PlayerPrefs.GetInt("IS_ACCEPT_UMP") == 1)
    {
      LevelPlay.SetConsent(true);
      LevelPlayAds.Instance.Init();
      Debug.Log("---Init");
      return;
    }

    // Initialize GoogleUserMessagingPlatform
    GoogleUserMessagingPlatform.Initialize(status =>
    {
      // Maybe you want to show the form directly after the initialisation if status is REQUIRED
      GoogleUserMessagingPlatform.ShowFormIfRequired(
        form => CheckConsentStatus()
      );
    });
  }

  private void CheckConsentStatus()
  {
    // Get current consent status
    var consentStatus = GoogleUserMessagingPlatform.ConsentStatus;

    Debug.Log($"Consent Status: {consentStatus}");

    // Apply to IronSource based on status
    switch (consentStatus)
    {
      case ConsentStatus.OBTAINED:
        Debug.Log("User AGREED to consent");
        LevelPlay.SetConsent(true);
        PlayerPrefs.SetInt("IS_ACCEPT_UMP", 1);
        break;

      case ConsentStatus.NOT_REQUIRED:
        Debug.Log("Consent not required (outside EEA)");
        LevelPlay.SetConsent(true);
        PlayerPrefs.SetInt("IS_ACCEPT_UMP", 1);
        break;

      case ConsentStatus.REQUIRED:
        Debug.Log("User DISAGREED or dismissed dialog");
        LevelPlay.SetConsent(false);
        break;

      case ConsentStatus.UNKNOWN:
      default:
        Debug.Log("Consent status unknown");
        LevelPlay.SetConsent(false);
        break;
    }

    // Initialize IronSource after consent is set
    LevelPlayAds.Instance.Init();
  }
}
