using System;
using UnityEngine;
using com.unity3d.mediation;
using Firebase.Analytics;
using System.Collections;
using HoangNam;
using UnityEngine.SceneManagement;

// Example for IronSource Unity.
public class LevelPlayAds : MonoBehaviour
{
  public static LevelPlayAds Instance { get; private set; }

  AdsDeviceData adsData;
  Action userRewardEarnedCallback;
  Action onShowRewardFail;
  Action onShowInterstitialFail;
  Action<Unity.Services.LevelPlay.LevelPlayAdInfo> onWatchedInterstitialClose;
  Action<Unity.Services.LevelPlay.LevelPlayAdInfo> onWatchedInterstitialSuccess;

  private Unity.Services.LevelPlay.LevelPlayBannerAd bannerAd;
  private Unity.Services.LevelPlay.LevelPlayInterstitialAd interstitialAd;

  string appKey = "";
  string bannerAdUnitId = "";
  string interstitialAdUnitId = "";

  bool isMusicOn;
  private bool _isSucceed;

  private string _placementRewardAds = "";

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }

    DontDestroyOnLoad(gameObject);
  }

  void Start()
  {
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
#if UNITY_IOS
    adsData = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.ads_data.ios;
#elif UNITY_ANDROID
    adsData = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.ads_data.android;
#endif

    if (adsData.is_banner_ads
      && GameManager.Instance.CurrentLevelIndex + 1 >= adsData.level_start_banner
      && !GameManager.Instance.IsRemoveAds)
    {
      ShowBanner();
    }
    else
    {
      HideBanner();
    }
  }

  public void Init()
  {
    // #if UNITY_IOS
    //     IronSourceEvents.onConsentViewDidAcceptEvent += onConsentViewDidAcceptEvent;
    //     IronSourceEvents.onConsentViewDidFailToLoadWithErrorEvent += onConsentViewDidFailToLoadWithErrorEvent;
    //     IronSourceEvents.onConsentViewDidLoadSuccessEvent += onConsentViewDidLoadSuccessEvent;
    //     IronSourceEvents.onConsentViewDidFailToShowWithErrorEvent += onConsentViewDidFailToShowWithErrorEvent;
    //     IronSourceEvents.onConsentViewDidShowSuccessEvent += onConsentViewDidShowSuccessEvent;
    // #endif

    InitAdUnit();
    EnableAds();

    Debug.Log("unity-script: IronSource.Agent.validateIntegration");
    IronSource.Agent.validateIntegration();

    Debug.Log("unity-script: unity version" + IronSource.unityVersion());

    // SDK init
    Debug.Log("unity-script: LevelPlay SDK initialization");
    Unity.Services.LevelPlay.LevelPlay.Init(appKey, adFormats: new[] { LevelPlayAdFormat.REWARDED });

    Unity.Services.LevelPlay.LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
    Unity.Services.LevelPlay.LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
  }

  private void InitAdUnit()
  {
    if (Debug.isDebugBuild)
    {
#if UNITY_ANDROID
      appKey = "85460dcd";
      bannerAdUnitId = "thnfvcsog13bhn08";
      interstitialAdUnitId = "aeyqi3vqlv6o8sh9";
#elif UNITY_IOS
      appKey = "8545d445";
      bannerAdUnitId = "iep3rxsyp9na3rw8";
      interstitialAdUnitId = "wmgt0712uuux8ju4";
#else
      appKey = "unexpected_platform";
      bannerAdUnitId = "unexpected_platform";
      interstitialAdUnitId = "unexpected_platform";
#endif
    }
    else
    {
#if UNITY_ANDROID
      appKey = "234ce6a05";
      bannerAdUnitId = "qh98u8j4oikjp2b7";
      interstitialAdUnitId = "ero3fi1728i7feim";
#elif UNITY_IOS
      appKey = "201621875";
      bannerAdUnitId = "mano8dz5ie7a8qxd";
      interstitialAdUnitId = "0ming2mth3486yhh";
#else
      appKey = "unexpected_platform";
      bannerAdUnitId = "unexpected_platform";
      interstitialAdUnitId = "unexpected_platform";
#endif
    }
  }

  [Obsolete]
  private void OnDestroy()
  {
    // #if UNITY_IOS
    //     IronSourceEvents.onConsentViewDidAcceptEvent -= onConsentViewDidAcceptEvent;
    //     IronSourceEvents.onConsentViewDidFailToLoadWithErrorEvent -= onConsentViewDidFailToLoadWithErrorEvent;
    //     IronSourceEvents.onConsentViewDidLoadSuccessEvent -= onConsentViewDidLoadSuccessEvent;
    //     IronSourceEvents.onConsentViewDidFailToShowWithErrorEvent -= onConsentViewDidFailToShowWithErrorEvent;
    //     IronSourceEvents.onConsentViewDidShowSuccessEvent -= onConsentViewDidShowSuccessEvent;
    // #endif

    Unity.Services.LevelPlay.LevelPlay.OnInitSuccess -= SdkInitializationCompletedEvent;
    Unity.Services.LevelPlay.LevelPlay.OnInitFailed -= SdkInitializationFailedEvent;
    SceneManager.sceneLoaded -= OnSceneLoaded;

    DisableAds();
  }

  // Consent View was loaded successfully
  private void onConsentViewDidShowSuccessEvent(string consentViewType)
  {
    Debug.Log("onConsentViewDidShowSuccessEvent");

  }
  // Consent view was failed to load
  private void onConsentViewDidFailToShowWithErrorEvent(string consentViewType, Unity.Services.LevelPlay.LevelPlayAdError error)
  {
    Debug.Log("onConsentViewDidFailToShowWithErrorEvent");

  }

  // Consent view was displayed successfully
  private void onConsentViewDidLoadSuccessEvent(string consentViewType)
  {
    Debug.Log("onConsentViewDidLoadSuccessEvent");
    IronSource.Agent.showConsentViewWithType("pre");
  }

  // Consent view was not displayed, due to error
  private void onConsentViewDidFailToLoadWithErrorEvent(string consentViewType, Unity.Services.LevelPlay.LevelPlayAdError error)
  {
    Debug.Log("onConsentViewDidFailToLoadWithErrorEvent");
    Debug.Log("Error" + error);
  }

  // The user pressed the Settings or Next buttons
  private void onConsentViewDidAcceptEvent(string consentViewType)
  {
    Debug.Log("onConsentViewDidAcceptEvent");
    // ATTrackingStatusBinding.RequestAuthorizationTracking();
  }

  [Obsolete]
  void EnableAds()
  {
    //Add ImpressionSuccess Event
    IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;

    //Add AdInfo Rewarded Video Events
    IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
    IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
    IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
    IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
    IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
    IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
    IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;

    bannerAd = new Unity.Services.LevelPlay.LevelPlayBannerAd(bannerAdUnitId, LevelPlayAdSize.CreateAdaptiveAdSize(), LevelPlayBannerPosition.BottomCenter);

    // Register to Banner events
    bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
    bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
    bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
    bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
    bannerAd.OnAdClicked += BannerOnAdClickedEvent;
    bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
    bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
    bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;

    // Create Interstitial object
    interstitialAd = new Unity.Services.LevelPlay.LevelPlayInterstitialAd(interstitialAdUnitId);

    // Register to Interstitial events
    interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
    interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
    interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
    interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
    interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
    interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
    interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
  }

  [Obsolete]
  void DisableAds()
  {
    //Add ImpressionSuccess Event
    IronSourceEvents.onImpressionDataReadyEvent -= ImpressionDataReadyEvent;

    //Add AdInfo Rewarded Video Events
    IronSourceRewardedVideoEvents.onAdOpenedEvent -= RewardedVideoOnAdOpenedEvent;
    IronSourceRewardedVideoEvents.onAdClosedEvent -= RewardedVideoOnAdClosedEvent;
    IronSourceRewardedVideoEvents.onAdAvailableEvent -= RewardedVideoOnAdAvailable;
    IronSourceRewardedVideoEvents.onAdUnavailableEvent -= RewardedVideoOnAdUnavailable;
    IronSourceRewardedVideoEvents.onAdShowFailedEvent -= RewardedVideoOnAdShowFailedEvent;
    IronSourceRewardedVideoEvents.onAdRewardedEvent -= RewardedVideoOnAdRewardedEvent;
    IronSourceRewardedVideoEvents.onAdClickedEvent -= RewardedVideoOnAdClickedEvent;

    // Register to Banner events
    if (bannerAd != null)
    {
      bannerAd.OnAdLoaded -= BannerOnAdLoadedEvent;
      bannerAd.OnAdLoadFailed -= BannerOnAdLoadFailedEvent;
      bannerAd.OnAdDisplayed -= BannerOnAdDisplayedEvent;
      bannerAd.OnAdDisplayFailed -= BannerOnAdDisplayFailedEvent;
      bannerAd.OnAdClicked -= BannerOnAdClickedEvent;
      bannerAd.OnAdCollapsed -= BannerOnAdCollapsedEvent;
      bannerAd.OnAdLeftApplication -= BannerOnAdLeftApplicationEvent;
      bannerAd.OnAdExpanded -= BannerOnAdExpandedEvent;
    }

    // Register to Interstitial events
    if (interstitialAd != null)
    {
      interstitialAd.OnAdLoaded -= InterstitialOnAdLoadedEvent;
      interstitialAd.OnAdLoadFailed -= InterstitialOnAdLoadFailedEvent;
      interstitialAd.OnAdDisplayed -= InterstitialOnAdDisplayedEvent;
      interstitialAd.OnAdDisplayFailed -= InterstitialOnAdDisplayFailedEvent;
      interstitialAd.OnAdClicked -= InterstitialOnAdClickedEvent;
      interstitialAd.OnAdClosed -= InterstitialOnAdClosedEvent;
      interstitialAd.OnAdInfoChanged -= InterstitialOnAdInfoChangedEvent;
    }
  }

  void OnApplicationPause(bool isPaused)
  {
    Debug.Log("unity-script: OnApplicationPause = " + isPaused);
    IronSource.Agent.onApplicationPause(isPaused);
  }

  #region Init callback handlers

  void SdkInitializationCompletedEvent(Unity.Services.LevelPlay.LevelPlayConfiguration config)
  {
    Debug.Log("unity-script: I got SdkInitializationCompletedEvent with config: " + config);

    bannerAd.LoadAd();
    interstitialAd.LoadAd();

    // #if UNITY_IOS
    //     if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
    //     {
    //       IronSource.Agent.loadConsentViewWithType("pre");
    //     }
    // #endif
  }

  void SdkInitializationFailedEvent(Unity.Services.LevelPlay.LevelPlayInitError error)
  {
    Debug.Log("unity-script: I got SdkInitializationFailedEvent with error: " + error);
  }

  #endregion

  #region AdInfo Rewarded Video
  void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
  {
    Debug.Log("unity-script: I got RewardedVideoOnAdOpenedEvent With AdInfo " + adInfo);
  }

  void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
  {
    Debug.Log("unity-script: I got RewardedVideoOnAdClosedEvent With AdInfo " + adInfo);

    BackToNormal();
    HoangNam.Utility.Print("RewardedVideoOnAdClosedEvent.info: " + adInfo);

    if (_isSucceed)
    {
      _isSucceed = false;
      userRewardEarnedCallback?.Invoke();

      if (FirebaseSetup.Instance.IsFirebaseReady)
      {
        FirebaseAnalytics.LogEvent(KeyString.KEY_REWARD_SHOWSUCCESS,
          new Parameter[]
          {
            new ("level", (GameManager.Instance.CurrentLevelIndex + 1).ToString()),
            new ("placement", _placementRewardAds)
          });
      }

      Utility.Print("Level_" + (GameManager.Instance.CurrentLevelIndex + 1) + "_Placement_" + _placementRewardAds);
    }
    else
    {
      onShowRewardFail?.Invoke();
    }
  }

  void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
  {
    Debug.Log("unity-script: I got RewardedVideoOnAdAvailable With AdInfo " + adInfo);
  }

  void RewardedVideoOnAdUnavailable()
  {
    Debug.Log("unity-script: I got RewardedVideoOnAdUnavailable");
  }

  void RewardedVideoOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
  {
    Debug.Log("unity-script: I got RewardedVideoOnAdShowFailedEvent With Error" + ironSourceError + "And AdInfo " + adInfo);

    if (FirebaseSetup.Instance.IsFirebaseReady)
      FirebaseAnalytics.LogEvent(KeyString.FIREBASE_ADS_REWARD_FAIL);

    onShowRewardFail?.Invoke();
  }

  void RewardedVideoOnAdRewardedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
  {
    Debug.Log("unity-script: I got RewardedVideoOnAdRewardedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);

    _isSucceed = true;
  }

  void RewardedVideoOnAdClickedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
  {
    Debug.Log("unity-script: I got RewardedVideoOnAdClickedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);

    if (FirebaseSetup.Instance.IsFirebaseReady)
      FirebaseAnalytics.LogEvent(KeyString.FIREBASE_ADS_REWARD_CLICK);

    _isSucceed = true;
  }

  #endregion
  #region AdInfo Interstitial

  void InterstitialOnAdLoadedEvent(Unity.Services.LevelPlay.LevelPlayAdInfo adInfo)
  {
    Debug.Log("unity-script: I got InterstitialOnAdLoadedEvent With AdInfo " + adInfo);

    if (FirebaseSetup.Instance.IsFirebaseReady)
      FirebaseAnalytics.LogEvent(KeyString.FIREBASE_AD_INTER_LOAD);
  }

  void InterstitialOnAdLoadFailedEvent(Unity.Services.LevelPlay.LevelPlayAdError error)
  {
    Debug.Log("unity-script: I got InterstitialOnAdLoadFailedEvent With Error " + error);

    if (FirebaseSetup.Instance.IsFirebaseReady)
      FirebaseAnalytics.LogEvent(KeyString.FIREBASE_AD_INTER_FAIL);
  }

  void InterstitialOnAdDisplayedEvent(Unity.Services.LevelPlay.LevelPlayAdInfo adInfo)
  {
    Debug.Log("unity-script: I got InterstitialOnAdDisplayedEvent With AdInfo " + adInfo);
  }

  [Obsolete]
  void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError infoError)
  {
    Debug.Log("unity-script: I got InterstitialOnAdDisplayFailedEvent With InfoError " + infoError);

    onShowInterstitialFail?.Invoke();
    StartCoroutine(TryLoadInterstitial());
  }

  void InterstitialOnAdClickedEvent(Unity.Services.LevelPlay.LevelPlayAdInfo adInfo)
  {
    Debug.Log("unity-script: I got InterstitialOnAdClickedEvent With AdInfo " + adInfo);

    if (FirebaseSetup.Instance.IsFirebaseReady)
      FirebaseAnalytics.LogEvent(KeyString.FIREBASE_AD_INTER_CLICK);
  }

  void InterstitialOnAdClosedEvent(Unity.Services.LevelPlay.LevelPlayAdInfo adInfo)
  {
    Debug.Log("unity-script: I got InterstitialOnAdClosedEvent With AdInfo " + adInfo);

    BackToNormal();

    onWatchedInterstitialClose?.Invoke(adInfo);
    StartCoroutine(TryLoadInterstitial());
    StartCoroutine(TryShowInterstitial());

    if (FirebaseSetup.Instance.IsFirebaseReady)
      FirebaseAnalytics.LogEvent(KeyString.KEY_INTER_SHOWSUCCESS);
  }

  void InterstitialOnAdInfoChangedEvent(Unity.Services.LevelPlay.LevelPlayAdInfo adInfo)
  {
    Debug.Log("unity-script: I got InterstitialOnAdInfoChangedEvent With AdInfo " + adInfo);
  }

  #endregion

  #region Banner AdInfo

  void BannerOnAdLoadedEvent(Unity.Services.LevelPlay.LevelPlayAdInfo adInfo)
  {
    Debug.Log("unity-script: I got BannerOnAdLoadedEvent With AdInfo " + adInfo);
  }

  void BannerOnAdLoadFailedEvent(Unity.Services.LevelPlay.LevelPlayAdError ironSourceError)
  {
    Debug.Log("unity-script: I got BannerOnAdLoadFailedEvent With Error " + ironSourceError);

    StartCoroutine(TryLoadBanner());
  }

  void BannerOnAdClickedEvent(Unity.Services.LevelPlay.LevelPlayAdInfo adInfo)
  {
    Debug.Log("unity-script: I got BannerOnAdClickedEvent With AdInfo " + adInfo);
  }

  void BannerOnAdDisplayedEvent(Unity.Services.LevelPlay.LevelPlayAdInfo adInfo)
  {
    Debug.Log("unity-script: I got BannerOnAdDisplayedEvent With AdInfo " + adInfo);
  }

  [Obsolete]
  void BannerOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError adInfoError)
  {
    Debug.Log("unity-script: I got BannerOnAdDisplayFailedEvent With AdInfoError " + adInfoError);

    StartCoroutine(TryLoadBanner());
  }

  void BannerOnAdCollapsedEvent(Unity.Services.LevelPlay.LevelPlayAdInfo adInfo)
  {
    Debug.Log("unity-script: I got BannerOnAdCollapsedEvent With AdInfo " + adInfo);
  }

  void BannerOnAdLeftApplicationEvent(Unity.Services.LevelPlay.LevelPlayAdInfo adInfo)
  {
    Debug.Log("unity-script: I got BannerOnAdLeftApplicationEvent With AdInfo " + adInfo);
  }

  void BannerOnAdExpandedEvent(Unity.Services.LevelPlay.LevelPlayAdInfo adInfo)
  {
    Debug.Log("unity-script: I got BannerOnAdExpandedEvent With AdInfo " + adInfo);
  }

  #endregion

  #region ImpressionSuccess callback handler

  void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
  {
    Debug.Log("unity - script: I got ImpressionDataReadyEvent ToString(): " + impressionData.ToString());
    Debug.Log("unity - script: I got ImpressionDataReadyEvent allData: " + impressionData.allData);

    if (impressionData != null)
    {
      Parameter[] AdParameters = {
        new("ad_platform", "ironSource"),
        new("ad_source", impressionData.adNetwork),
        new("ad_unit_name", impressionData.adFormat),
        new("ad_format", impressionData.instanceName),
        new("currency","USD"),
        new("value", impressionData.revenue.Value)
      };
      FirebaseAnalytics.LogEvent(KeyString.FIREBASE_AD_IMPRESSION, AdParameters);
      FirebaseAnalytics.LogEvent(KeyString.FIREBASE_AD_MAX);
    }
  }

  #endregion

  public void ShowRewardedAd(Action userRewardEarned, string placement, Action fallback = null)
  {
    if (!adsData.is_reward_ads)
    {
      userRewardEarned?.Invoke();
      return;
    }

#if UNITY_EDITOR
    userRewardEarned?.Invoke();
#else
    this.userRewardEarnedCallback = userRewardEarned;
    this.onShowRewardFail = fallback;
    _placementRewardAds = placement;

    PauseGame();

    if (IronSource.Agent.isRewardedVideoAvailable())
    {
      IronSource.Agent.showRewardedVideo();
      if (FirebaseSetup.Instance.IsFirebaseReady)
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_ADS_REWARD_SHOW);
    }
    else
    {
      BackToNormal();
      HoangNam.Utility.Print("Ironsource.Rewarded video is not ready");
      fallback?.Invoke();

      // RewardCustomAd.Instance.ShowRewardedAd(userRewardEarned, placement, fallback);
    }

    if (FirebaseSetup.Instance.IsFirebaseReady)
    {
      FirebaseAnalytics.LogEvent(KeyString.FIREBASE_ADS_REWARD_OFFER);
      FirebaseAnalytics.LogEvent(KeyString.FIREBASE_REWARD_PLACE_ + placement);
      FirebaseAnalytics.LogEvent(
        KeyString.FIREBASE_WATCH_VIDEO_GAME,
        new Parameter[] {
            new("actionWatch", "ShowRewardedAd"),
            new("has_ads", IronSource.Agent.isRewardedVideoAvailable() == true ? 1: 0),
            new("has_internet", CheckInternet.HasNetwork() == true ? 1: 0),
            new("placement", placement),
        }
      );
    }
#endif
  }

  public void ShowInterstitialAd(
    Action<Unity.Services.LevelPlay.LevelPlayAdInfo> onSuccess,
    string placement,
    Action<Unity.Services.LevelPlay.LevelPlayAdInfo> onClose = null,
    Action fallback = null)
  {
    if (!adsData.is_inter_ads)
    {
      onClose?.Invoke(null);
      return;
    }

    if (adsData.is_inter_ads
    && GameManager.Instance.CurrentLevelIndex + 1 < adsData.level_start_inter)
    {
      onClose?.Invoke(null);
      return;
    }

    if (interstitialAd == null)
    {
      onClose?.Invoke(null);
      return;
    }

    if (GameManager.Instance.IsRemoveAds)
    {
      onClose?.Invoke(null);
      return;
    }

    if (!_isShowInterAds)
    {
      onClose?.Invoke(null);
      return;
    }

#if UNITY_EDITOR
    onClose?.Invoke(null);
#else
    this.onWatchedInterstitialClose = onClose;
    this.onWatchedInterstitialSuccess = onSuccess;
    this.onShowInterstitialFail = fallback;

    PauseGame();

    if (interstitialAd.IsAdReady())
    {
      interstitialAd.ShowAd();

      FirebaseAnalytics.LogEvent(
        KeyString.FIREBASE_AD_INTER_SHOW,
        new Parameter[] {
          new ("level", (GameManager.Instance.CurrentLevelIndex + 1).ToString()),
        }
      );

      Utility.Print("Ads_inter_show" + GameManager.Instance.CurrentLevelIndex + 1);
    }
    else
    {
      BackToNormal();
      HoangNam.Utility.Print("Ironsource.Interstitial video is not ready");
      StartCoroutine(TryLoadInterstitial());
      fallback?.Invoke();
      // InterstitialCustomAd.Instance.ShowInterstitialAd(fallback, placement, fallback);
    }

    if (FirebaseSetup.Instance.IsFirebaseReady)
    {
      FirebaseAnalytics.LogEvent(
        KeyString.FIREBASE_WATCH_VIDEO_GAME,
        new Parameter[] {
        new("actionWatch", "ShowInterstitialAd"),
        new("has_ads", IronSource.Agent.isRewardedVideoAvailable() == true ? 1: 0),
        new("has_internet", CheckInternet.HasNetwork() == true ? 1: 0),
        new("placement", placement),
        }
      );
    }
#endif
  }

  public bool IsReadyInterstitialAds()
  {
    if (interstitialAd == null) return false;
    return interstitialAd.IsAdReady();
  }

  void PauseGame()
  {
    isMusicOn = GameManager.Instance.IsMusicOn;
    GameManager.Instance.IsMusicOn = false;
    // Time.timeScale = 1;
  }

  void BackToNormal()
  {
    GameManager.Instance.IsMusicOn = isMusicOn;
    // Time.timeScale = 1;
  }

  private void OnDisable()
  {
    bannerAd?.DestroyAd();
    interstitialAd?.DestroyAd();
  }

  public void ShowBanner()
  {
    bannerAd?.ShowAd();
  }

  public void HideBanner()
  {
    bannerAd?.HideAd();
  }

  bool _isLoadInterAds = false;
  IEnumerator TryLoadInterstitial()
  {
    if (_isLoadInterAds) yield break;

    _isLoadInterAds = true;
    yield return new WaitForSeconds(5);

    _isLoadInterAds = false;
    interstitialAd.LoadAd();
  }

  bool _isShowInterAds = true;
  IEnumerator TryShowInterstitial()
  {
    if (!_isShowInterAds) yield break;
    _isShowInterAds = false;
    yield return new WaitForSeconds(adsData.time_step_inter);
    _isShowInterAds = true;
  }

  IEnumerator TryLoadBanner()
  {
    yield return new WaitForSeconds(5);

    bannerAd.LoadAd();
  }
}
