using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using DG.Tweening;
using Unity.Mathematics;
using Unity.Collections;

public partial class LevelSystem : MonoBehaviour
{
  public static LevelSystem Instance { get; private set; }
  [SerializeField] GridWorld boardGrid;
  [SerializeField] GridWorld slotGrid;
  [SerializeField] QuadMeshSystem quadMeshSystem;
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

    SubscribeTouchEvent();
    m20LevelSystem.InitPool();
    yield return new WaitForSeconds(0.1f);

    m20LevelSystem.SetupCurrentLevel(_levelInformation);
    SetupCurrentLevel(_levelInformation);

    isLoadedLevel = true;
    GameManager.Instance.SetGameState(GameState.Gameplay);
  }

  void OnDestroy()
  {
    UnsubscribeTouchEvent();
    DisposeDataBuffers();
    quadMeshSystem.DisposeComponents();
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
    InitDataBuffers(levelInformation);
    SpawnAndBakingEntityDatas(levelInformation);
    SetSizeCamera();
  }

  void BakingGrids(LevelInformation levelInformation)
  {
    var quadScale = new float2(.15f, .15f);

    boardGrid.GridScale = quadScale;
    boardGrid.GridSize = new int2(10 * 8, 20 * 8);
    boardGrid.InitConvertedComponents();

    slotGrid.GridScale = quadScale;
    slotGrid.GridSize = new int2(22, 22);
    slotGrid.InitConvertedComponents();

    quadMeshSystem.ScaleSize = boardGrid.GridScale;
    quadMeshSystem.QuadCapacity = boardGrid.GridSize.x * boardGrid.GridSize.y;
    quadMeshSystem.InitComponents();
  }

  void SpawnAndBakingEntityDatas(LevelInformation levelInformation)
  {
    var blockPositions = new NativeArray<float2>(4, Allocator.Temp);
    blockPositions[0] = new(0, -1);
    blockPositions[1] = new(0, 0);
    blockPositions[2] = new(0, 1);
    blockPositions[3] = new(1, 1);
    OrderBlockShapeAt(0, blockPositions, 0);
    OrderBlockShapeAt(1, blockPositions, 0);
    OrderBlockShapeAt(2, blockPositions, 0);
    blockPositions.Dispose();

    VisualizeActiveQuads();
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
