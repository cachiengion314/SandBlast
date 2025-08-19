using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class LobbyPanel : MonoBehaviour
{
  public static LobbyPanel Instance { get; private set; }
  [SerializeField] RectTransform settingModal;
  [SerializeField] Transform goalCompletedNotify;
  [SerializeField] Transform uiCanvas;
  [SerializeField] TextMeshProUGUI levelText;
  [SerializeField] TextMeshProUGUI coinText;

  private void Start()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);

    levelText.text = $"LEVEL {GameManager.Instance.CurrentLevelIndex + 1}";
    UpdateCoin();
    GameManager.Instance.OnCoinChange += UpdateCoin;
  }

  private void OnDestroy()
  {
    DOTween.KillAll();
    GameManager.Instance.OnCoinChange -= UpdateCoin;
  }

  void UpdateCoin()
  {
    coinText.text = GameManager.Instance.CurrentCoin.ToString();
  }

  void OpenModal(Transform panel)
  {
    panel.gameObject.SetActive(true);
    panel.GetComponentInChildren<Animator>().Play("OpenModal");
  }

  void CloseModal(Transform panel)
  {
    panel.GetComponentInChildren<Animator>().Play("CloseModal");
    var m_CurrentClipInfo = panel.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0);
    var m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
    LeanTween.delayedCall(gameObject, m_CurrentClipLength, () =>
    {
      panel.gameObject.SetActive(false);
    });
  }

  bool IsShowingNotify = false;

  void OpenNotify(Transform panel)
  {
    if (IsShowingNotify) return;
    IsShowingNotify = true;
    panel.gameObject.SetActive(true);
    panel.GetComponentInChildren<Animator>().Play("OpenNotify");

    var m_CurrentClipInfo = panel.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0);
    var m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
    LeanTween.delayedCall(gameObject, m_CurrentClipLength, () =>
    {
      IsShowingNotify = false;
      Destroy(panel.gameObject);
    });
  }

  public void ShowNotifyWith(string content)
  {
    var _goalCompletedNotify = Instantiate(goalCompletedNotify, uiCanvas);
    _goalCompletedNotify.GetComponentInChildren<TextMeshProUGUI>().text = content;
    OpenNotify(_goalCompletedNotify);
  }

  public void LoadSceneWithDelay(string sceneName)
  {
    SoundManager.Instance.PlayPressBtnSfx();
    SceneManager.LoadScene(sceneName);
  }

  public void LoadSceneAt(int i)
  {
    SceneManager.LoadScene(i);
  }

  public void LoadSceneWith(string nameScene)
  {
    SceneManager.LoadScene(nameScene);
  }

  public void ToggleSettingPanel()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!settingModal.gameObject.activeSelf)
    {
      OpenModal(settingModal);
    }
    else
    {
      CloseModal(settingModal);
    }
  }
}
