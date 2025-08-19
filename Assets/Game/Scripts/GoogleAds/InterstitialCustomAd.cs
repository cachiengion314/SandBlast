// using GoogleMobileAds.Api;
using UnityEngine;

public class InterstitialCustomAd : MonoBehaviour
{
//   Action userRewardEarnedCallback;
//   Action fallback;

//   public static InterstitialCustomAd Instance { get; private set; }

//   bool isMusicOn;
//   public string AndroidAdUnitId;
//   public string IOSAdUnitId;
//   // These ad units are configured to always serve test ads.
// #if UNITY_ANDROID
//   private string _adUnitId = "ca-app-pub-6234439167982040/6740730523";
// #elif UNITY_IPHONE
//   private string _adUnitId = "ca-app-pub-6234439167982040/4824395078";
// #else
//   private string _adUnitId = "unused";
// #endif

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

//     LoadInterstitialAd();
//   }

//   private InterstitialAd _interstitialAd;


//   public bool CanShowAd()
//   {
//     if (_interstitialAd != null && _interstitialAd.CanShowAd())
//     {
//       return true;
//     }
//     return false;
//   }

//   /// <summary>
//   /// Shows the interstitial ad.
//   /// </summary>
//   public void ShowInterstitialAd(Action userRewardEarnedCallback, string placement, Action fallback = null)
//   {
//     this.userRewardEarnedCallback = userRewardEarnedCallback;
//     this.fallback = fallback;

//     isMusicOn = GameManager.Instance.IsMusicOn;
//     GameManager.Instance.IsMusicOn = false;

//     if (_interstitialAd != null && _interstitialAd.CanShowAd())
//     {
//       Debug.Log("Showing interstitial ad.");

//       if (FirebaseSetup.Instance.FirebaseStatusCode)
//       {
//         FirebaseAnalytics.LogEvent(KeyString.KEY_INTER_SHOWSUCCESS);
//       }
//       _interstitialAd.Show();
//     }
//     else
//     {
//       GameManager.Instance.IsMusicOn = isMusicOn;
//       Debug.LogError("Interstitial ad is not ready yet.");
//       fallback?.Invoke();
//     }

//     if (FirebaseSetup.Instance.FirebaseStatusCode)
//     {
//       FirebaseAnalytics.LogEvent(
//         KeyString.FIREBASE_WATCH_VIDEO_GAME,
//         new Parameter[] {
//         new("actionWatch", "ShowInterstitialAd"),
//         // new("has_ads", IronSource.Agent.isRewardedVideoAvailable() == true ? 1: 0),
//         new("has_internet", CheckInternet.HasNetwork() == true ? 1: 0),
//         new("placement", placement),
//         }
//       );
//     }
//   }

//   /// <summary>
//   /// Loads the interstitial ad.
//   /// </summary>
//   public void LoadInterstitialAd(Action onWatchedSuccessClose = null)
//   {
//     // Clean up the old ad before loading a new one.
//     if (_interstitialAd != null)
//     {
//       _interstitialAd.Destroy();
//       _interstitialAd = null;
//     }

//     Debug.Log("Loading the interstitial ad.");

//     // create our request used to load the ad.
//     var adRequest = new AdRequest();

//     // send the request to load the ad.
//     InterstitialAd.Load(_adUnitId, adRequest,
//         (InterstitialAd ad, LoadAdError error) =>
//         {
//           // if error is not null, the load request failed.
//           if (error != null || ad == null)
//           {
//             Debug.LogError("interstitial ad failed to load an ad " +
//                                "with error : " + error);
//             return;
//           }

//           Debug.Log("Interstitial ad loaded with response : "
//                         + ad.GetResponseInfo());

//           _interstitialAd = ad;

//           _interstitialAd.OnAdClicked += OnAdClicked;
//           _interstitialAd.OnAdImpressionRecorded += OnAdImpressionRecorded;
//           _interstitialAd.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
//           _interstitialAd.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;

//           if (FirebaseSetup.Instance.FirebaseStatusCode)
//             FirebaseAnalytics.LogEvent(KeyString.FIREBASE_AD_INTER_LOAD);
//         });
//   }

//   public void OnAdClicked()
//   {
//     if (FirebaseSetup.Instance.FirebaseStatusCode)
//       FirebaseAnalytics.LogEvent(KeyString.FIREBASE_AD_INTER_CLICK);
//   }

//   public void OnAdImpressionRecorded()
//   {
//     Parameter[] AdParameters = {
//         new Parameter("ad_platform", "Google Admob"),
//         new Parameter("currency","USD"),
//       };

//     if (FirebaseSetup.Instance.FirebaseStatusCode)
//     {
//       FirebaseAnalytics.LogEvent(KeyString.FIREBASE_AD_IMPRESSION, AdParameters);
//       FirebaseAnalytics.LogEvent(KeyString.FIREBASE_AD_MAX);
//     }
//   }

//   public void OnAdFullScreenContentClosed()
//   {
//     GameManager.Instance.IsMusicOn = isMusicOn;

//     StartCoroutine(IEUserReward());
//     LoadInterstitialAd();
//   }

//   IEnumerator IEUserReward()
//   {
//     yield return null;
//     userRewardEarnedCallback?.Invoke();
//   }

//   public void OnAdFullScreenContentFailed(AdError adError)
//   {
//     GameManager.Instance.IsMusicOn = isMusicOn;
//     fallback?.Invoke();
//     LoadInterstitialAd();

//     if (FirebaseSetup.Instance.FirebaseStatusCode)
//       FirebaseAnalytics.LogEvent(KeyString.FIREBASE_AD_INTER_FAIL);
//   }
}
