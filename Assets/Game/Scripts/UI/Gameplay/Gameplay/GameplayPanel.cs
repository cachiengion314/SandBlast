using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class GameplayPanel : MonoBehaviour
{
  public static GameplayPanel Instance { get; private set; }
  [Space(20)]
  [Header("Referents")]
  [SerializeField] Transform uiCanvas;
  [SerializeField] Transform notifyModal;
  [SerializeField] Transform SettingModal;
  [SerializeField] Transform LevelCompleteModal;
  [SerializeField] Transform LevelFailedModal;
  [SerializeField] Transform RePlayModal;
  [SerializeField] Transform OutOfSpaceModal;
  [SerializeField] Transform BuyBooster1Modal;
  [SerializeField] Transform BuyBooster2Modal;
  [SerializeField] Transform BuyBooster3Modal;
  [SerializeField] Transform UseBooster1Modal;
  [SerializeField] Transform UseBooster2Modal;
  [SerializeField] Transform UseBooster3Modal;

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
  }
  private void Start()
  {
    InitBooster();
    UpdateCoin();
    GameManager.Instance.OnCoinChange += UpdateCoin;
    // levelText.text = $"Level\n{GameManager.Instance.CurrentLevelIndex + 1}";
  }

  private void OnDestroy()
  {
    UnsubscribeBoosterEvent();
    GameManager.Instance.OnCoinChange -= UpdateCoin;
  }

  void UpdateCoin()
  {
    coinText.text = GameManager.Instance.CurrentCoin.ToString();
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
    var _goalCompletedNotify = Instantiate(notifyModal, uiCanvas);
    _goalCompletedNotify.GetComponentInChildren<TextMeshProUGUI>().text = content;
    OpenNotify(_goalCompletedNotify);
  }

  void OpenModal(Transform panel)
  {
    panel.gameObject.SetActive(true);
    panel.GetComponentInChildren<Animator>().Play("OpenModal");
    GameManager.Instance.SetGameState(GameState.GamepPause);
  }

  void CloseModal(Transform panel)
  {
    panel.GetComponentInChildren<Animator>().Play("CloseModal");
    var m_CurrentClipInfo = panel.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0);
    var m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
    LeanTween.delayedCall(gameObject, m_CurrentClipLength, () =>
    {
      panel.gameObject.SetActive(false);
      GameManager.Instance.SetGameState(GameState.Gameplay);
    });
  }


  public void ToggleSettingModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!SettingModal.gameObject.activeSelf)
    {
      OpenModal(SettingModal);
    }
    else
    {
      CloseModal(SettingModal);
    }
  }

  public void ToggleLevelCompleteModal()
  {
    if (!LevelCompleteModal.gameObject.activeSelf)
    {
      SoundManager.Instance.PlayWinLevelSfx();
      LevelCompleteModal.gameObject.SetActive(true);
    }
    else
    {
      LevelCompleteModal.gameObject.SetActive(false);
    }
  }

  public void ToggleLevelFailedModal()
  {
    if (!LevelFailedModal.gameObject.activeSelf)
    {
      SoundManager.Instance.PlayLoseLevelSfx();
      OpenModal(LevelFailedModal);
    }
    else
    {
      CloseModal(LevelFailedModal);
    }
  }

  public void ToggleReplayModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();
    if (!RePlayModal.gameObject.activeSelf)
    {
      OpenModal(RePlayModal);
    }
    else
    {
      CloseModal(RePlayModal);
    }
  }

  public void ToggleOutOfSpaceModal()
  {
    if (!OutOfSpaceModal.gameObject.activeSelf)
    {
      SoundManager.Instance.PlayLoseLevelSfx();
      OpenModal(OutOfSpaceModal);
    }
    else
    {
      CloseModal(OutOfSpaceModal);
    }
  }

  public void ToggleBooster1Modal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!BuyBooster1Modal.gameObject.activeSelf)
    {
      OpenModal(BuyBooster1Modal);
    }
    else
    {
      CloseModal(BuyBooster1Modal);
    }
  }

  public void ToggleBooster2Modal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!BuyBooster2Modal.gameObject.activeSelf)
    {
      OpenModal(BuyBooster2Modal);
    }
    else
    {
      CloseModal(BuyBooster2Modal);
    }
  }

  public void ToggleBooster3Modal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!BuyBooster3Modal.gameObject.activeSelf)
    {
      OpenModal(BuyBooster3Modal);
    }
    else
    {
      CloseModal(BuyBooster3Modal);
    }
  }
}
