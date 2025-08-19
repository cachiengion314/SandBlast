using System;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
  Menu,
  Gameplay,
  GamepPause,
  Gameover,
  Gamewin,
  Tutorial,
}

public partial class GameManager : MonoBehaviour
{
  public event Action OnBooster1Change;
  public event Action OnBooster2Change;
  public event Action OnBooster3Change;
  public event Action OnSoundChange;
  public event Action OnMusicChange;
  public event Action OnHapticChange;
  public event Action OnCoinChange;
  public event Action OnRemoveAdsChange;


  public static GameManager Instance { get; private set; }

  [Header("Events")]
  public Action<GameState> onGameStateChanged;

  [Header("User Settings")]
  [Range(19, 30)]
  [SerializeField] int levelIndexCapacity;
  public int LevelIndexCapacity
  {
    get { return levelIndexCapacity; }
  }

  GameState _gameState;

  bool _IsRemoveAds;
  public bool IsRemoveAds
  {
    get => _IsRemoveAds;
    set
    {
      _IsRemoveAds = value;
      PlayerPrefs.SetInt(KeyString.KEY_IS_REMOVE_ADS, value ? 1 : 0);
      OnRemoveAdsChange?.Invoke();
    }
  }

  int _Booster1;
  public int Booster1
  {
    get => _Booster1;
    set
    {
      _Booster1 = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_BOOSTER_1, value);
      OnBooster1Change?.Invoke();
    }
  }

  int _Booster2;
  public int Booster2
  {
    get => _Booster2;
    set
    {
      _Booster2 = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_BOOSTER_2, value);
      OnBooster2Change?.Invoke();
    }
  }
  int _Booster3;
  public int Booster3
  {
    get => _Booster3;
    set
    {
      _Booster3 = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_BOOSTER_3, value);
      OnBooster3Change?.Invoke();
    }
  }

  bool _IsSoundOn;
  public bool IsSoundOn
  {
    get => _IsSoundOn;
    set
    {
      _IsSoundOn = value;
      if (!value) FirebaseAnalytics.LogEvent(KeyString.FIREBASE_SOUND_DISABLED);
      PlayerPrefs.SetInt(KeyString.KEY_IS_SOUND_ON, value ? 1 : 0);
      OnSoundChange?.Invoke();
    }
  }

  bool _IsMusicOn;
  public bool IsMusicOn
  {
    get => _IsMusicOn;
    set
    {
      _IsMusicOn = value;
      if (!value) FirebaseAnalytics.LogEvent(KeyString.FIREBASE_MUSIC_DISABLED);
      PlayerPrefs.SetInt(KeyString.KEY_IS_MUSIC_ON, value ? 1 : 0);
      OnMusicChange?.Invoke();
    }
  }

  bool _IsHapticOn;
  public bool IsHapticOn
  {
    get => _IsHapticOn;
    set
    {
      _IsHapticOn = value;
      if (!value) FirebaseAnalytics.LogEvent(KeyString.FIREBASE_HAPTIC_DISABLED);
      PlayerPrefs.SetInt(KeyString.KEY_IS_HAPTIC_ON, value ? 1 : 0);
      OnHapticChange?.Invoke();
    }
  }

  int _CurrentCoin;
  public int CurrentCoin
  {
    get => _CurrentCoin;
    set
    {
      _CurrentCoin = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_COIN, value);
      OnCoinChange?.Invoke();
    }
  }

  int _CurrentLevelIndex;
  public int CurrentLevelIndex
  {
    get
    {
      _CurrentLevelIndex = _CurrentLevelIndex > levelIndexCapacity ? levelIndexCapacity : _CurrentLevelIndex;
      return _CurrentLevelIndex;
    }
    set
    {
      _CurrentLevelIndex = value > levelIndexCapacity ? levelIndexCapacity : value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_LEVEL_INDEX, _CurrentLevelIndex);
    }
  }
  /// <summary>
  /// Event section
  /// </summary>
  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      GetInitUserData();
    }
    else
    {
      Destroy(gameObject);
    }
    DontDestroyOnLoad(gameObject);
  }

  /// <summary>
  /// Game State Section
  /// </summary>
  /// <returns></returns>
  public GameState GetGameState()
  {
    return _gameState;
  }

  public void SetGameState(GameState state)
  {
    _gameState = state;
    onGameStateChanged?.Invoke(state);
  }

  public void LoadSceneFrom(GameState state)
  {
    _gameState = state;
    onGameStateChanged?.Invoke(state);
  }

  public void LoadSceneFrom(string sceneName)
  {
    SceneManager.LoadScene(sceneName);
  }

  void GetInitUserData()
  {
    _IsRemoveAds = PlayerPrefs.GetInt(KeyString.KEY_IS_REMOVE_ADS, 1) == 0;
    _Booster1 = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_BOOSTER_1, 3);
    _Booster2 = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_BOOSTER_2, 3);
    _Booster3 = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_BOOSTER_3, 3);
    _IsSoundOn = PlayerPrefs.GetInt(KeyString.KEY_IS_SOUND_ON, 1) == 1;
    _IsMusicOn = PlayerPrefs.GetInt(KeyString.KEY_IS_MUSIC_ON, 1) == 1;
    _IsHapticOn = PlayerPrefs.GetInt(KeyString.KEY_IS_HAPTIC_ON, 1) == 1;
    _CurrentCoin = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_COIN, 200);
    _CurrentLevelIndex = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_LEVEL_INDEX, 0);
  }
}
