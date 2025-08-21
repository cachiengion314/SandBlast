using System;
using DG.Tweening;
using Firebase.Analytics;
using UnityEngine;

[Serializable]
public class ColorBlockPartitionData
{
    [ViewOnly] public int Index;
    public int ColorValue;
    [Range(1, 5)]
    public int Health = 1;
}

public partial class M20LevelSystem : MonoBehaviour
{
    [Header("Level Manager")]
    [Range(0f, 2f)]
    [SerializeField] float updateSpeed;
    [Header("Dependencies")]
    [SerializeField] Transform spawnedParent;
    public Transform SpawnedParent => spawnedParent;
    [Header("Grids")]
    [SerializeField] GridWorld topGrid;
    int _amountColorBlock = 0;
    public void SetupCurrentLevel(LevelInformation levelInformation)
    {
        topGrid.transform.position = levelInformation.TopGridPosition;
        topGrid.GridSize = levelInformation.TopGridSize;
        topGrid.InitConvertedComponents();

        var colorBlockPartitionDatas = levelInformation.ColorBlockPartitionDatas;

        var length = topGrid.GridSize.x * topGrid.GridSize.y;
        _colorBlocks = new ColorBlockControl[length];

        for (int i = 0; i < colorBlockPartitionDatas.Length; ++i)
        {
            var partition = colorBlockPartitionDatas[i];
            var index = partition.Index;
            var gridPos = topGrid.ConvertIndexToGridPos(index);
            var colorBlock = SpawnColorBlockAt(index, spawnedParent);
            colorBlock.SetIndex(index);
            colorBlock.SetColorValue(partition.ColorValue);
            colorBlock.SetInitHealth(partition.Health);
            colorBlock.SetSortingOrder(-gridPos.y);

            if (!IsAtVisibleBound(colorBlock.gameObject))
                colorBlock.gameObject.SetActive(false);

            _colorBlocks[index] = colorBlock;
            _amountColorBlock++;
        }

        var slots = FindQueueSlotsPosParent(levelInformation.AmountSlot);
        for (int i = 0; i < slots.childCount; i++)
        {
            var pos = slots.GetChild(i).position;
            var blast = SpawnBlastBlockAt(pos, spawnedParent);
            if (blast.TryGetComponent<IColorBlock>(out var blastColor))
            {
                blastColor.SetIndex(-1);
                blastColor.SetColorValue(-1);
            }
            if (blast.TryGetComponent<IGun>(out var blastGun))
            {
                blastGun.SetAmmunition(0);
            }
            _firingSlots.Add(blast.gameObject);
        }
    }

    public void UpdateWinLevel()
    {
        if (_amountColorBlock > 0) return;
        GameManager.Instance.SetGameState(GameState.Gamewin);
        DOVirtual.DelayedCall(1f, GameplayPanel.Instance.ToggleLevelCompleteModal);

        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_END_LEVEL,
          new Parameter[]
          {
        new ("level_id", (GameManager.Instance.CurrentLevelIndex + 1).ToString()),
        new ("result", 1),
          });
    }

    public void UpdateLoseLevel()
    {
        var emptyIndex = FindIndexEmptyBlast();
        if (emptyIndex != -1) return;
        if (_needMovingColorBlocks.Count > 0) return;

        for (int i = 0; i < _firingSlots.Count; i++)
        {
            var blast = _firingSlots[i];
            if (blast == null) continue;
            if (!blast.TryGetComponent(out IColorBlock colorBlast)) continue;
            if (IsThereAtLeastOneBlockOfTheSameColor(colorBlast.GetColorValue())) return;
        }

        GameManager.Instance.SetGameState(GameState.Gameover);
        DOVirtual.DelayedCall(1f, () =>
        {
            GameplayPanel.Instance.ToggleLevelFailedModal();
        });

        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_END_LEVEL,
           new Parameter[]
           {
        new ("level_id", (GameManager.Instance.CurrentLevelIndex + 1).ToString()),
        new ("result", 0),
           });
    }
}
