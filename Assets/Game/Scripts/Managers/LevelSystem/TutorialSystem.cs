using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
public partial class LevelSystem
{
    [SerializeField] Tutorial tutorial;
    [SerializeField] Sprite booster1Sprite;
    [SerializeField] Sprite booster2Sprite;
    [SerializeField] Sprite booster3Sprite;
    [SerializeField] Canvas canvasBooster1;
    [SerializeField] Canvas canvasBooster2;
    [SerializeField] Canvas canvasBooster3;

    void StartTutorial1()
    {
        if (GameManager.Instance.CurrentLevelIndex == 0
            && PlayerPrefs.GetInt(KeyString.KEY_TUTORIAL_1, 0) == 0)
        {
            var sortingGroup = quadMeshSystem.gameObject.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "UI";
            sortingGroup.sortingOrder = 15;

            var startPos = GetSlotPositionAt(0);
            var endPos = quadMeshSystem.transform.position;
            tutorial.DoHandMoveAt(KeyString.KEY_TUTORIAL_1, (Vector3)startPos, endPos, -150,2f);
            tutorial.ShowTapPanelAt(KeyString.KEY_TUTORIAL_1, true, () =>
            {
                tutorial.HideObject();
                tutorial.HideTapPanel();
                Destroy(sortingGroup);
                PlayerPrefs.SetInt(KeyString.KEY_TUTORIAL_1, 1);
                tutorial.StopTutorial();
            });
        }
    }

    void StartTutorial2a()
    {
        if (GameManager.Instance.CurrentLevelIndex == 4
            && PlayerPrefs.GetInt(KeyString.KEY_TUTORIAL_2, 0) == 0)
        {
            GameManager.Instance.SetGameState(GameState.GamepPause);
            tutorial.ShowTapPanelAt(KeyString.KEY_TUTORIAL_2);
            tutorial.ShowReceivePanelAt(
                KeyString.KEY_TUTORIAL_2,
                booster1Sprite,
                "<b><color=#FFA500>SHUFFLE</color></b>\nShuffle board to spawn new block", () =>
                {
                    GameManager.Instance.Booster1++;
                    tutorial.HideReceivePanel();
                    canvasBooster1.overrideSorting = true;
                    var endPos = canvasBooster1.transform.position + new Vector3(0, 1.6f);
                    var startPos = endPos + new Vector3(0, 1.5f);
                    tutorial.DoHandMoveAt(KeyString.KEY_TUTORIAL_2, startPos, endPos);
                });
        }
    }

    public void StartTutorial2b()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_2)) return;
        tutorial.HideTapPanel();
        tutorial.HideObject();
        canvasBooster1.overrideSorting = false;
        GameManager.Instance.SetGameState(GameState.Gameplay);
        PlayerPrefs.SetInt(KeyString.KEY_TUTORIAL_2, 1);
        tutorial.StopTutorial();
    }

    void StartTutorial3a()
    {
        if (GameManager.Instance.CurrentLevelIndex == 9
            && PlayerPrefs.GetInt(KeyString.KEY_TUTORIAL_3, 0) == 0)
        {
            GameManager.Instance.SetGameState(GameState.GamepPause);
            SpawnQuadBlockAt();
            ApplyDrawOrders();
            tutorial.ShowTapPanelAt(KeyString.KEY_TUTORIAL_3);
            tutorial.ShowReceivePanelAt(
                KeyString.KEY_TUTORIAL_3,
                booster2Sprite,
                "<b><color=#FFA500>BROOM</color></b>\nRemoves an entire row from the board", () =>
                {
                    GameManager.Instance.Booster2++;
                    tutorial.HideReceivePanel();
                    canvasBooster2.overrideSorting = true;
                    var endPos = canvasBooster2.transform.position + new Vector3(0, 1.6f);
                    var startPos = endPos + new Vector3(0, 1.5f);
                    tutorial.DoHandMoveAt(KeyString.KEY_TUTORIAL_3, startPos, endPos);
                });
        }
    }

    public void StartTutorial3b()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_3)) return;
        tutorial.HideTapPanel();
        tutorial.HideObject();
        canvasBooster2.overrideSorting = false;
        GameManager.Instance.SetGameState(GameState.Gameplay);
        PlayerPrefs.SetInt(KeyString.KEY_TUTORIAL_3, 1);
        tutorial.StopTutorial();
    }

    void StartTutorial4a()
    {
        if (GameManager.Instance.CurrentLevelIndex == 14
            && PlayerPrefs.GetInt(KeyString.KEY_TUTORIAL_4, 0) == 0)
        {
            GameManager.Instance.SetGameState(GameState.GamepPause);
            SpawnQuadBlockAt();
            ApplyDrawOrders();
            tutorial.ShowTapPanelAt(KeyString.KEY_TUTORIAL_4);
            tutorial.ShowReceivePanelAt(
                KeyString.KEY_TUTORIAL_4,
                booster3Sprite,
                "<b><color=#FFA500>MAGNET</color></b>\nCollects a group of sands", () =>
                {
                    GameManager.Instance.Booster3++;
                    tutorial.HideReceivePanel();
                    canvasBooster3.overrideSorting = true;
                    var endPos = canvasBooster3.transform.position + new Vector3(0, 1.6f);
                    var startPos = endPos + new Vector3(0, 1.5f);
                    tutorial.DoHandMoveAt(KeyString.KEY_TUTORIAL_4, startPos, endPos);
                });
        }
    }

    public void StartTutorial4b()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_4)) return;
        tutorial.HideTapPanel();
        tutorial.HideObject();
        canvasBooster3.overrideSorting = false;
        GameManager.Instance.SetGameState(GameState.Gameplay);
        PlayerPrefs.SetInt(KeyString.KEY_TUTORIAL_4, 1);
        tutorial.StopTutorial();
    }

    void SpawnQuadBlockAt()
    {
        for (int y = 0; y < 3; y++)
        {
            var fromY = y * 8;
            var toY = fromY + 8;
            for (int x = 0; x < blockGrid.GridSize.x; x++)
            {
                var fromX = x * 8;
                var toX = fromX + 8;
                SpawnQuadBlockAt(fromY, toY, fromX, toX);
            }
        }
    }

    void SpawnQuadBlockAt(int fromY, int toY, int fromX, int toX)
    {
        var quadsAmount = (toX - fromX) * (toY - fromY);
        using var inactiveQuads = FindInactiveQuadsForShape(quadsAmount);
        if (inactiveQuads.Length == 0)
        {
            print("Cannot find any spare quads");
            return;
        }
        var colorValue = GetRamdomColor();
        int shapeIdx = GenerateUniqueShapeIdx();
        var newGroupData = new GroupQuadData
        {
            QuadsAmount = quadsAmount,
            ColorValue = colorValue,
            IsActive = true,
        };
        _groupQuadDatas.Add(shapeIdx, newGroupData);

        int i = 0;
        for (int y = fromY; y < toY; y++)
        {
            for (int x = fromX; x < toX; x++)
            {
                var newColorValue = colorValue;
                var ratio = UnityEngine.Random.Range(0f, 100f);
                if (ratio > 80f)
                {
                    var xColor = UnityEngine.Random.Range(0, quadMeshSystem.GridResolution.x - 1);
                    var newColorGrid = new int2(xColor, colorValue);
                    newColorValue = quadMeshSystem.ConvertGirdPosToIndex(newColorGrid);
                }
                var gridPos = new int2(x, y);
                var idx = quadGrid.ConvertGridPosToIndex(gridPos);
                var pos = quadGrid.ConvertGridPosToWorldPos(gridPos);
                var quadData = inactiveQuads[i++];
                quadData.GroupIndex = shapeIdx;
                quadData.Position = pos;
                quadData.IndexPosition = idx;
                quadData.ColorValue = newColorValue;
                quadData.IsActive = true;
                var index = quadData.Index;
                _quadDatas[index] = quadData;
                _quadIndexPositionDatas[idx] = index;
                OrderQuadMeshAt(index, pos, newColorValue);
            }
        }
    }
}