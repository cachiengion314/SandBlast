using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using DG.Tweening;
using Unity.Mathematics;

public partial class LevelSystem : MonoBehaviour
{
  public static LevelSystem Instance { get; private set; }
  [SerializeField] GridWorld boardGrid;
  [SerializeField] M20LevelSystem m20LevelSystem;
  [SerializeField] CinemachineCamera cinemachineCamera;
  LevelInformation _levelInformation;
  [SerializeField][Range(1, 30)] int levelSelected = 1;
  public bool IsSelectedLevel;
  bool isLoadedLevel = false;

  IEnumerator Start()
  {
    if (Instance == null)
    {
      DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(1500, 100);
      Instance = this;
    }
    else Destroy(gameObject);

    if (IsSelectedLevel)
    {
      GameManager.Instance.CurrentLevelIndex = levelSelected - 1;
      LoadLevelFrom(levelSelected);
    }
    else LoadLevelFrom(GameManager.Instance.CurrentLevelIndex + 1);

    GameManager.Instance.SetGameState(GameState.Gameplay);
    SubscribeTouchEvent();
    m20LevelSystem.InitPool();
    yield return new WaitForSeconds(0.1f);

    m20LevelSystem.SetupCurrentLevel(_levelInformation);
    SetupCurrentLevel(_levelInformation);
    
    isLoadedLevel = true;
  }

  void OnDestroy()
  {
    UnsubscribeTouchEvent();
    DisposeDataBuffers();
    DisposeQuadMeshSystem();
  }

  void Update()
  {
    if (!isLoadedLevel) return;
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
    m20LevelSystem.FindNeedArrangeCollumnInUpdate();
    m20LevelSystem.ArrangeColorBlocksUpdate();
    m20LevelSystem.LockAndFireTargetUpddate();
    m20LevelSystem.BulletPositionsUpdate();
    m20LevelSystem.UpdateLoseLevel();
    m20LevelSystem.UpdateWinLevel();

    ControlQuadsInUpdate();
    if (Input.GetKeyDown(KeyCode.Space))
    {
      m20LevelSystem.SetAmmunitionBlastColorAt(0);
    }
  }

  void SetupCurrentLevel(LevelInformation levelInformation)
  {
    BakingGrids(levelInformation);
    InitEntitiesDataBuffers(levelInformation);
    SpawnAndBakingEntityDatas(levelInformation);
    SetSizeCamera();
  }

  void BakingGrids(LevelInformation levelInformation)
  {
    boardGrid.GridScale = new float2(1, 1);
    boardGrid.GridSize = new int2(10, 10);
    boardGrid.InitConvertedComponents();
  }

  void SpawnAndBakingEntityDatas(LevelInformation levelInformation)
  {
    SpawnQuadMeshes(100);
  }

  public void LoadLevelFrom(int level)
  {
    var _rawLevelInfo = Resources.Load<TextAsset>("Levels/" + KeyString.NAME_LEVEL_FILE + level).text;
    var levelInfo = JsonUtility.FromJson<LevelInformation>(_rawLevelInfo);

    if (levelInfo == null) { print("This level is not existed!"); return; }
    _levelInformation = levelInfo;
    print("Load level " + level + " successfully ");
  }

  void SetSizeCamera()
  {

  }
}
