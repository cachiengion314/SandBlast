using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using DG.Tweening;
using Unity.Mathematics;
using Unity.Collections;

public partial class LevelSystem : MonoBehaviour
{
  public static LevelSystem Instance { get; private set; }
  [SerializeField] GridWorld quadGrid;
  [SerializeField] GridWorld blockGrid;
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
      DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(100, 50);
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

    if (Input.GetKeyDown(KeyCode.Space))
    {
      m20LevelSystem.SetAmmunitionBlastColorAt(2);
    }

    GrabbingBlockControlInUpdate();
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

    blockGrid.GridScale = quadScale * 8;
    blockGrid.GridSize = new int2(10, 12);
    blockGrid.InitConvertedComponents();

    quadGrid.GridScale = quadScale;
    quadGrid.GridSize = new int2(blockGrid.GridSize.x * 8, blockGrid.GridSize.y * 8);
    quadGrid.InitConvertedComponents();

    slotGrid.GridScale = quadScale;
    slotGrid.GridSize = new int2(22, 22);
    slotGrid.InitConvertedComponents();

    quadMeshSystem.QuadScale = quadGrid.GridScale;
    quadMeshSystem.QuadCapacity = quadGrid.GridSize.x * quadGrid.GridSize.y;
    quadMeshSystem.InitComponents();
  }

  void SpawnAndBakingEntityDatas(LevelInformation levelInformation)
  {
    if (!IsSlotsEmpty()) return;

    var blockSlotPositions = new NativeArray<float2>(4, Allocator.Temp);
    blockSlotPositions[0] = new(.0f, .5f);
    blockSlotPositions[1] = new(1.0f, .5f);
    blockSlotPositions[2] = new(.0f, -.5f);
    blockSlotPositions[3] = new(-1.0f, -.5f);

    OrderBlockShapeAt(0, blockSlotPositions, 2);
    OrderBlockShapeAt(1, blockSlotPositions, 3);
    OrderBlockShapeAt(2, blockSlotPositions, 4);
    
    blockSlotPositions.Dispose();

    VisualizeActiveQuads();
    ApplyDrawOrders();
  }

  bool IsSlotsEmpty()
  {
    for (int i = 0; i < slotsParent.childCount; i++)
      if (!IsSlotEmptyAt(0)) return false;
    return true;
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
