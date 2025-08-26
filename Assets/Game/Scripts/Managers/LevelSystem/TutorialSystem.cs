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
            tutorial.DoHandMoveAt(KeyString.KEY_TUTORIAL_1, (Vector3)startPos, endPos);
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
                "Shuffle", () =>
                {
                    GameManager.Instance.Booster1++;
                    tutorial.HideReceivePanel();
                    canvasBooster1.overrideSorting = true;
                    var endPos = canvasBooster1.transform.position + new Vector3(0, 2);
                    var startPos = endPos + new Vector3(0, 1.5f);
                    tutorial.DoHandMoveAt(KeyString.KEY_TUTORIAL_2, startPos, endPos, 180);
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
            tutorial.ShowTapPanelAt(KeyString.KEY_TUTORIAL_3);
            tutorial.ShowReceivePanelAt(
                KeyString.KEY_TUTORIAL_3,
                booster2Sprite,
                "Broom", () =>
                {
                    GameManager.Instance.Booster2++;
                    tutorial.HideReceivePanel();
                    canvasBooster2.overrideSorting = true;
                    var endPos = canvasBooster2.transform.position + new Vector3(0, 2);
                    var startPos = endPos + new Vector3(0, 1.5f);
                    tutorial.DoHandMoveAt(KeyString.KEY_TUTORIAL_3, startPos, endPos, 180);
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
            tutorial.ShowTapPanelAt(KeyString.KEY_TUTORIAL_4);
            tutorial.ShowReceivePanelAt(
                KeyString.KEY_TUTORIAL_4,
                booster3Sprite,
                "Magnet", () =>
                {
                    GameManager.Instance.Booster3++;
                    tutorial.HideReceivePanel();
                    canvasBooster3.overrideSorting = true;
                    var endPos = canvasBooster3.transform.position + new Vector3(0, 2);
                    var startPos = endPos + new Vector3(0, 1.5f);
                    tutorial.DoHandMoveAt(KeyString.KEY_TUTORIAL_4, startPos, endPos, 180);
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
}