using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using DG.Tweening;
using Unity.Mathematics;
using Unity.Collections;

public partial class LevelSystem : MonoBehaviour
{
  public static LevelSystem Instance { get; private set; }
  [SerializeField] GridWorld slotGrid;
  int2 quadGridSize;
  float2 quadGridScale;
  float3 quadGridPos;
  int2 blockGridSize;
  float2 blockGridScale;
  float3 blockGridPos;
  int2 slotGridSize;
  float2 slotGridScale;
  float3 slotGridPos;
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
    CalculateGravityForQuadsInUpdate();
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
    var quadScale = new float2(.11f, .11f);

    // blockGrid.GridScale = quadScale * 8;
    // blockGrid.GridSize = new int2(10, 12);
    // blockGrid.InitConvertedComponents();

    // quadGrid.GridScale = quadScale;
    // quadGrid.GridSize = new int2(blockGrid.GridSize.x * 8, blockGrid.GridSize.y * 8);
    // quadGrid.InitConvertedComponents();

    // slotGrid.GridScale = quadScale;
    // slotGrid.GridSize = new int2(22, 22);
    // slotGrid.InitConvertedComponents();

    blockGridSize = new int2(10, 12);
    blockGridScale = quadScale * 8;
    blockGridPos = float3.zero;

    quadGridSize = new int2(blockGridSize.x * 8, blockGridSize.y * 8);
    quadGridScale = quadScale;
    quadGridPos = float3.zero;

    slotGridSize = new int2(22, 22);
    slotGridScale = quadScale;
    slotGridPos = float3.zero;

    quadMeshSystem.QuadScale = quadGridScale;
    quadMeshSystem.QuadCapacity = quadGridSize.x * quadGridSize.y;
    quadMeshSystem.InitComponents();
  }

  void SpawnAndBakingEntityDatas(LevelInformation levelInformation)
  {
    for (int i = 0; i < slotsParent.childCount; i++)
    {
      var colorValue = GetRamdomColor();
      using var blockSlotPositions = GetRandomShape();
      OrderBlockShapeAt(i, blockSlotPositions, colorValue);

      if (!_blockShapeDatas.ContainsKey(i)) return;
      var shapeData = _blockShapeDatas[i];
      _blockShapeDatas[i] = shapeData;
    }

    VisualizeActiveQuads();
    ApplyDrawOrders();
  }

  bool IsSlotsEmpty()
  {
    for (int i = 0; i < slotsParent.childCount; i++)
      if (!IsSlotEmptyAt(i)) return false;
    return true;
  }

  int GetRamdomColor()
  {
    float randomNumber = UnityEngine.Random.Range(0f, 100f);
    float threshold = 0;
    var availableColorDatas = _levelInformation.AvailableColorDatas;
    for (int i = 0; i < availableColorDatas.Length; i++)
    {
      var ratio = availableColorDatas[i].ratio;
      threshold += ratio;
      if (randomNumber < threshold)
        return availableColorDatas[i].ColorValue;
    }
    return availableColorDatas[0].ColorValue;
  }

  NativeArray<float2> GetRandomShape()
  {
    var randomIndex = UnityEngine.Random.Range(0, 8);
    return RendererSystem.Instance.GetShapeAt(randomIndex);
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
