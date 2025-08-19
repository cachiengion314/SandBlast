// using GoogleMobileAds.Api;
using UnityEngine;

// https://gist.github.com/prakashpun/d19e34c5710f8b7f40f828c7df3e887c
public class BannerAd : MonoBehaviour
{
//   public static BannerAd Instance { get; private set; }
//   public string AndroidAdUnitId;
//   public string IOSAdUnitId;

//   // These ad units are configured to always serve test ads.
// #if UNITY_ANDROID
//     private string _adUnitId = "ca-app-pub-6234439167982040/4604903222";
// #elif UNITY_IPHONE
//   private string _adUnitId = "ca-app-pub-6234439167982040/3104737845";
// #else
//   private string _adUnitId = "unused";
// #endif

//   BannerView _bannerView;

//   private void OnEnable()
//   {
//     GameManager.onChangeRemoveAds += DestroyBannerView;
//   }

//   private void OnDisable()
//   {
//     GameManager.onChangeRemoveAds += DestroyBannerView;
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
//     DontDestroyOnLoad(gameObject);

// #if UNITY_ANDROID
//         _adUnitId = AndroidAdUnitId;
// #elif UNITY_IPHONE
//     _adUnitId = IOSAdUnitId;
// #endif

//     LoadAd();
//   }

//   /// <summary>
//   /// Creates a 320x50 banner view at top of the screen.
//   /// </summary>
//   public void CreateBannerView()
//   {
//     Debug.Log("Creating banner view");

//     // If we already have a banner, destroy the old one.
//     if (_bannerView != null)
//     {
//       DestroyBannerView();
//     }

//     // Create a 320x50 banner at top of the screen
//     AdSize adaptiveSize =
//         AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
//     _bannerView = new BannerView(_adUnitId, adaptiveSize, AdPosition.Bottom);
//   }

//   /// <summary>
//   /// Creates the banner view and loads a banner ad.
//   /// </summary>
//   public void LoadAd()
//   {
//     if (GameManager.Instance.IsRemoveAds) return;

//     // create an instance of a banner view first.
//     if (_bannerView == null)
//     {
//       CreateBannerView();
//     }

//     // create our request used to load the ad.
//     var adRequest = new AdRequest();

//     // send the request to load the ad.
//     Debug.Log("Loading banner ad.");
//     _bannerView.LoadAd(adRequest);
//   }

//   /// <summary>
//   /// Destroys the banner view.
//   /// </summary>
//   public void DestroyBannerView()
//   {
//     if (_bannerView != null)
//     {
//       Debug.Log("Destroying banner view.");
//       _bannerView.Destroy();
//       _bannerView = null;
//     }
//   }
}
