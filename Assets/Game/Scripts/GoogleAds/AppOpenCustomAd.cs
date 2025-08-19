using UnityEngine;
// using GoogleMobileAds.Api;
// using GoogleMobileAds.Common;

/// <summary>
/// Demonstrates how to use the Google Mobile Ads app open ad format.
/// </summary>
[AddComponentMenu("GoogleMobileAds/Samples/AppOpenAdController")]
public class AppOpenCustomAd : MonoBehaviour
{
//   public static AppOpenCustomAd Instance { get; private set; }

//   public string AndroidAdUnitId;
//   public string IOSAdUnitId;

//   // These ad units are configured to always serve test ads.
// #if UNITY_ANDROID
//   string _adUnitId = "ca-app-pub-3940256099942544/9257395921";
// #elif UNITY_IPHONE
//   string _adUnitId = "ca-app-pub-3940256099942544/5575463023";
// #else
//     private string _adUnitId = "unused";
// #endif

//   DateTime _expireTime;
//   private AppOpenAd _appOpenAd;
//   public bool IsAdAvailable
//   {
//     get
//     {
//       return _appOpenAd != null
//              && DateTime.Now < _expireTime;
//     }
//   }

//   private void Start()
//   {
//     if (Instance == null)
//     {
//       Instance = this;
//     }
//     else
//     {
//       Destroy(gameObject);
//     }

// #if UNITY_ANDROID
//     _adUnitId = AndroidAdUnitId;
// #elif UNITY_IPHONE
//     _adUnitId = IOSAdUnitId;
// #endif
//     if (GameManager.Instance.IsRemoveAds) return;

//     if (PlayerPrefs.GetInt(KeyString.KEY_FIRST_TIME_APPOPEN) == 0)
//     {
//       PlayerPrefs.SetInt(KeyString.KEY_FIRST_TIME_APPOPEN, 1);
//       LoadAppOpenAd();
//     }
//     else
//     {
//       LoadAppOpenAd(ShowAppOpenAd);
//     }

//     // Use the AppStateEventNotifier to listen to application open/close events.
//     // This is used to launch the loaded ad when we open the app.
//     AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
//   }

//   private void OnDestroy()
//   {
//     // Always unlisten to events when complete.
//     AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
//   }


//   private void OnAppStateChanged(AppState state)
//   {
//     Debug.Log("App State changed to : " + state);
//     // if the app is Foregrounded and the ad is available, show it.
//     if (state == AppState.Foreground)
//     {
//       if (IsAdAvailable)
//       {
//         ShowAppOpenAd();
//       }
//     }
//   }

//   /// <summary>
//   /// Loads the app open ad.
//   /// </summary>
//   public void LoadAppOpenAd(Action _onCompleted = null)
//   {
//     // Clean up the old ad before loading a new one.
//     if (_appOpenAd != null)
//     {
//       _appOpenAd.Destroy();
//       _appOpenAd = null;
//     }

//     Debug.Log("Loading the app open ad.");

//     // Create our request used to load the ad.
//     var adRequest = new AdRequest();

//     // send the request to load the ad.
//     AppOpenAd.Load(_adUnitId, adRequest,
//         (AppOpenAd ad, LoadAdError error) =>
//         {
//           // if error is not null, the load request failed.
//           if (error != null || ad == null)
//           {
//             Debug.LogError("app open ad failed to load an ad " +
//                              "with error : " + error);
//             return;
//           }

//           Debug.Log("App open ad loaded with response : "
//                       + ad.GetResponseInfo());

//           // App open ads can be preloaded for up to 4 hours.
//           _expireTime = DateTime.Now + TimeSpan.FromHours(4);

//           _appOpenAd = ad;

//           _onCompleted?.Invoke();
//           RegisterReloadHandler(ad);
//         });
//   }

//   /// <summary>
//   /// Shows the app open ad.
//   /// </summary>
//   public void ShowAppOpenAd()
//   {
//     if (GameManager.Instance.IsRemoveAds) return;

//     var isShowAd = true;

// #if UNITY_ANDROID
//     isShowAd = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.Admob.android.admob_AOA.isShow;
// #elif UNITY_IOS
//     isShowAd = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.Admob.ios.admob_AOA.isShow;
// #endif

//     if (!isShowAd) return;

//     if (_appOpenAd != null && _appOpenAd.CanShowAd())
//     {
//       Debug.Log("Showing app open ad.");
//       _appOpenAd.Show();

//       if (FirebaseSetup.Instance.FirebaseStatusCode)
//       {
//         FirebaseAnalytics.LogEvent(KeyString.KEY_AD_APPOPEN_SHOWSUCCESS);
//       }
//     }
//     else
//     {
//       Debug.LogError("App open ad is not ready yet.");
//     }
//   }

//   private void RegisterReloadHandler(AppOpenAd ad)
//   {
//     // Raised when the ad closed full screen content.
//     ad.OnAdFullScreenContentClosed += () =>
//     {
//       Debug.Log("App open ad full screen content closed.");

//       // Reload the ad so that we can show another as soon as possible.
//       LoadAppOpenAd();
//     };
//     // Raised when the ad failed to open full screen content.
//     ad.OnAdFullScreenContentFailed += (AdError error) =>
//     {
//       Debug.LogError("App open ad failed to open full screen content " +
//                      "with error : " + error);

//       // Reload the ad so that we can show another as soon as possible.
//       LoadAppOpenAd();
//     };
//   }

//   private void RegisterEventHandlers(AppOpenAd ad)
//   {
//     // Raised when the ad is estimated to have earned money.
//     ad.OnAdPaid += (AdValue adValue) =>
//     {
//       Debug.Log(String.Format("App open ad paid {0} {1}.",
//           adValue.Value,
//           adValue.CurrencyCode));
//     };
//     // Raised when an impression is recorded for an ad.
//     ad.OnAdImpressionRecorded += () =>
//     {
//       Debug.Log("App open ad recorded an impression.");
//     };
//     // Raised when a click is recorded for an ad.
//     ad.OnAdClicked += () =>
//     {
//       Debug.Log("App open ad was clicked.");
//     };
//     // Raised when an ad opened full screen content.
//     ad.OnAdFullScreenContentOpened += () =>
//     {
//       Debug.Log("App open ad full screen content opened.");
//     };
//     // Raised when the ad closed full screen content.
//     ad.OnAdFullScreenContentClosed += () =>
//     {
//       Debug.Log("App open ad full screen content closed.");
//     };
//     // Raised when the ad failed to open full screen content.
//     ad.OnAdFullScreenContentFailed += (AdError error) =>
//     {
//       Debug.LogError("App open ad failed to open full screen content " +
//                      "with error : " + error);
//     };
//   }
}