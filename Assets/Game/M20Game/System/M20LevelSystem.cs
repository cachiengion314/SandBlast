using System;
using DG.Tweening;
using Firebase.Analytics;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct ColorBlockPartitionData
{
    [ViewOnly] public int Index;
    public int ColorValue;
    [Range(1, 5)]
    public int Health;
}
[Serializable]
public struct AvailableColorData
{
    public int ColorValue;
    public float ratio;
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
    int2 GridSize;
    float2 GridScale;
    float3 GridPos;
    int _amountColorBlock = 0;
    public void SetupCurrentLevel(LevelInformation levelInformation)
    {
        GridSize = levelInformation.TopGridSize;
        GridScale = new int2(1, 1);
        GridPos = levelInformation.TopGridPosition;

        var colorBlockPartitionDatas = levelInformation.ColorBlockPartitionDatas;
        var length = GridSize.x * GridSize.y;
        _colorBlocks = new ColorBlockControl[length];

        for (int i = 0; i < colorBlockPartitionDatas.Length; ++i)
        {
            var partition = colorBlockPartitionDatas[i];
            var index = partition.Index;
            var gridPos = GridSystem.ConvertIndexToGridPos(index, GridSize);
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
        VisualizeStartColorBlock();

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

    void VisualizeStartColorBlock()
    {
        Sequence seq = DOTween.Sequence();
        var spaceY = 0.1f;
        var spaceX = 0.03f;
        var duration = 0.3f;

        for (int i = 0; i < _colorBlocks.Length; i++)
        {
            var block = _colorBlocks[i];
            if (block == null) continue;
            var blockPos = GridSystem.ConvertIndexToWorldPos(i, GridSize, GridScale, GridPos);
            var blockGrid = GridSystem.ConvertIndexToGridPos(i, GridSize);

            block.transform.position = blockPos + new float3(0f, 7f, 0f);

            seq.Insert(spaceX * blockGrid.x + spaceY * blockGrid.y,
            block.transform.DOMove(blockPos, duration)
            .SetEase(Ease.OutQuart));
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
        LevelSystem.Instance.loseType = 2;
        GameManager.Instance.SetGameState(GameState.Gameover);
        DOVirtual.DelayedCall(1f, () =>
        {
            if (_currentSlot == maxSlot)
                GameplayPanel.Instance.ToggleLevelFailedModal();
            else
                GameplayPanel.Instance.ToggleOutOfSpaceModal();
        });

        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_END_LEVEL,
           new Parameter[]
           {
        new ("level_id", (GameManager.Instance.CurrentLevelIndex + 1).ToString()),
        new ("result", 0),
           });
    }

    public void PlayOn()
    {
        if (_currentSlot == maxSlot) return;

        var duration = 0.3f;
        var slots = FindQueueSlotsPosParent(++_currentSlot);
        var pos = slots.GetChild(slots.childCount - 1).position;
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
        var currentScale = blast.transform.localScale;
        blast.transform.localScale = float3.zero;
        blast.transform.DOScale(currentScale, duration).SetEase(Ease.Linear);
        for (int i = 0; i < _firingSlots.Count; i++)
        {
            var blast1 = _firingSlots[i];
            var pos1 = slots.GetChild(i).position;
            blast1.transform.DOMove(pos1, duration).SetEase(Ease.Linear);
        }
    }
}
