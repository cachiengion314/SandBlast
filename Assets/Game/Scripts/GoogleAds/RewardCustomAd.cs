// using GoogleMobileAds.Api;
using UnityEngine;

public class RewardCustomAd : MonoBehaviour
{
//   Action onAdClicked;
//   Action userRewardEarnedCallback;
//   Action fallback;

//   public static RewardCustomAd Instance { get; private set; }
//   public string AndroidAdUnitId;
//   public string IOSAdUnitId;

//   // These ad units are configured to always serve test ads.
// #if UNITY_ANDROID
//   private string _adUnitId = "ca-app-pub-6234439167982040/9530545394";
// #elif UNITY_IPHONE
//   private string _adUnitId = "ca-app-pub-6234439167982040/9502006687";
// #else
//     private string _adUnitId = "unused";
// #endif

//   private RewardedAd _rewardedAd;
//   public RewardedAd RewardedAd { get { return _rewardedAd; } }
//   bool isMusicOn;

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

//     LoadRewardedAd();
//   }

//   public bool CanShowAd()
//   {
//     return _rewardedAd != null && _rewardedAd.CanShowAd();
//   }

//   bool _isSucceed;
//   public void ShowRewardedAd(Action userRewardEarnedCallback, string placement, Action fallback = null)
//   {
//     Debug.Log("Start show reward");
//     this.userRewardEarnedCallback = userRewardEarnedCallback;
//     this.fallback = fallback;

//     isMusicOn = GameManager.Instance.IsMusicOn;
//     GameManager.Instance.IsMusicOn = false;

//     if (_rewardedAd != null && _rewardedAd.CanShowAd())
//     {
//       Debug.Log("Show reward");

//       _rewardedAd.Show((Reward reward) =>
//       {
//         Debug.Log("Show reward succeed");
//         _isSucceed = true;
//       });
//     }
//     else
//     {
//       fallback?.Invoke();
//       Debug.Log("Fail reward");
//       GameManager.Instance.IsMusicOn = isMusicOn;
//       // Time.timeScale = 1;
//     }

//     if (FirebaseSetup.Instance.FirebaseStatusCode)
//     {
//       FirebaseAnalytics.LogEvent(KeyString.FIREBASE_ADS_REWARD_OFFER);
//       FirebaseAnalytics.LogEvent(KeyString.FIREBASE_REWARD_PLACE_ + placement);
//       FirebaseAnalytics.LogEvent(
//         KeyString.FIREBASE_WATCH_VIDEO_GAME,
//         new Parameter[] {
//             new("actionWatch", "ShowRewardedAd"),
//             // new("has_ads", IronSource.Agent.isRewardedVideoAvailable() == true ? 1: 0),
//             new("has_internet", CheckInternet.HasNetwork() == true ? 1: 0),
//             new("placement", placement),
//         }
//       );
//     }
//   }

//   /// <summary>
//   /// Loads the rewarded ad.
//   /// </summary>
//   public void LoadRewardedAd()
//   {
//     // Clean up the old ad before loading a new one.
//     if (_rewardedAd != null)
//     {
//       _rewardedAd.Destroy();
//       _rewardedAd = null;
//     }

//     Debug.Log("Loading the rewarded ad.");
//     // create our request used to load the ad.
//     var adRequest = new AdRequest();
//     // send the request to load the ad.
//     RewardedAd.Load(_adUnitId, adRequest,
//         (RewardedAd ad, LoadAdError error) =>
//         {
//           // if error is not null, the load request failed.
//           if (error != null || ad == null)
//           {
//             Debug.LogError("Rewarded ad failed to load an ad " +
//                                "with error : " + error);
//             return;
//           }
//           Debug.Log("Rewarded ad loaded with response : "
//                         + ad.GetResponseInfo());
//           _rewardedAd = ad;

//           _rewardedAd.OnAdClicked += OnAdClicked;
//           _rewardedAd.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
//           _rewardedAd.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
//           _rewardedAd.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
//           _rewardedAd.OnAdImpressionRecorded += OnAdImpressionRecorded;
//         });
//   }

//   public void OnAdClicked()
//   {
//     _isSucceed = true;
//     onAdClicked?.Invoke();

//     if (FirebaseSetup.Instance.FirebaseStatusCode)
//       FirebaseAnalytics.LogEvent(KeyString.FIREBASE_ADS_REWARD_CLICK);
//   }

//   public void OnAdFullScreenContentOpened()
//   {
//     if (FirebaseSetup.Instance.FirebaseStatusCode)
//       FirebaseAnalytics.LogEvent(KeyString.FIREBASE_ADS_REWARD_SHOW);
//   }

//   public void OnAdFullScreenContentClosed()
//   {
//     GameManager.Instance.IsMusicOn = isMusicOn;
//     if (_isSucceed)
//     {
//       _isSucceed = false;
//       StartCoroutine(IEOnUserReward());
//     }
//     else
//     {
//       StartCoroutine(IEFallback());
//     }

//     LoadRewardedAd();
//   }

//   IEnumerator IEOnUserReward()
//   {
//     yield return null;
//     userRewardEarnedCallback?.Invoke();
//   }

//   IEnumerator IEFallback()
//   {
//     yield return null;
//     fallback?.Invoke();
//   }

//   public void OnAdFullScreenContentFailed(AdError adError)
//   {
//     GameManager.Instance.IsMusicOn = isMusicOn;
//     fallback?.Invoke();
//     LoadRewardedAd();

//     if (FirebaseSetup.Instance.FirebaseStatusCode)
//       FirebaseAnalytics.LogEvent(KeyString.FIREBASE_ADS_REWARD_FAIL);
//   }

//   public void OnAdImpressionRecorded()
//   {
//     Parameter[] AdParameters = {
//         new Parameter("ad_platform", "googleAdmob"),
//         new Parameter("currency","USD"),
//       };

//     if (FirebaseSetup.Instance.FirebaseStatusCode)
//     {
//       FirebaseAnalytics.LogEvent(KeyString.FIREBASE_AD_IMPRESSION, AdParameters);
//       FirebaseAnalytics.LogEvent(KeyString.FIREBASE_AD_MAX);
//     }
//   }

//   public void ResetRewardAd()
//   {
//     if (_rewardedAd != null)
//     {
//       _rewardedAd.Destroy();
//       _rewardedAd = null;
//     }
//   }
}
