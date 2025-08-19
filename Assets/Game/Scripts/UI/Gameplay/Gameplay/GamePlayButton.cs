using TMPro;
using UnityEngine;
using Sych.ShareAssets.Runtime;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Firebase.Analytics;
public partial class GameplayPanel
{
    [Space(20)]
    [Header("Logic")]
    [SerializeField] CoinsMoveSystem coinsMoveSystem;
    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TextMeshPro levelText;
    [SerializeField] string _urlGame;
    public void SendEmail()
    {
        string email = "cmzsoft.vn@gmail.com";
        string subject = EscapeURL("Yêu cầu hỗ trợ");
        string body = EscapeURL("Xin chào, tôi cần hỗ trợ về sản phẩm của bạn...");

        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    string EscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }

    public void ClickShare
    ()
    {
        // iOS chia sẻ
        // Tạo danh sách các mục cần chia sẻ
        string item = "Link Game: " + _urlGame;

        Share.Item(item, success =>
        {
            Debug.Log($"Chia sẻ {(success ? "thành công" : "thất bại")}");
        });

    }

    public void BackHome()
    {
        DOTween.KillAll();
        SoundManager.Instance.PlayPressBtnSfx();
        SceneManager.LoadScene(KeyString.NAME_SCENE_LOBBY);
    }

    public void TryAgain()
    {
        LevelPlayAds.Instance.ShowInterstitialAd(
        null,
        "try again",
        (LevelInformation) => OnTryAgain(),
        () => OnTryAgain());
    }

    void OnTryAgain()
    {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_LEVEL_RETRY,
        new Parameter[]
        {
            new ("level_id", (GameManager.Instance.CurrentLevelIndex + 1).ToString()),
        });
        DOTween.KillAll();
        SceneManager.LoadScene(KeyString.NAME_SCENE_GAMEPLAY);
    }

    public void PlayOn()
    {
        int coin = 600;
        if (GameManager.Instance.CurrentCoin < coin)
        {
            ShowNotifyWith("NOT ENOUGH COINS");
            return;
        }
        ToggleOutOfSpaceModal();
        GameManager.Instance.CurrentCoin -= coin;
        // PlayOn
    }

    public void ShowLevelFail()
    {
        OutOfSpaceModal.gameObject.SetActive(false);
        ToggleLevelFailedModal();
    }

    public void Continue()
    {
        LevelPlayAds.Instance.ShowInterstitialAd(
            null,
            "continue",
            (LevelInformation) => OnContinue(),
            () => OnContinue());
    }
    void OnContinue()
    {
        coinsMoveSystem.PlayCoinsEfx(startPos.position, endPos.position, 10, () =>
        {
            GameManager.Instance.CurrentLevelIndex++;
            GameManager.Instance.CurrentCoin += 20;
            DOTween.KillAll();
            SceneManager.LoadScene(KeyString.NAME_SCENE_GAMEPLAY);
        });
    }
}