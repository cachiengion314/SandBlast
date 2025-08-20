using System;
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
        }

        var slots = FindQueueSlotsPosParent(levelInformation.AmountSlot);
        for (int i = 0; i < slots.childCount; i++)
        {
            var pos = slots.GetChild(i).position;
            var blast = SpawnBlastBlockAt(pos, spawnedParent);
            if (blast.TryGetComponent<IColorBlock>(out var blastColor))
            {
                blastColor.SetIndex(-1);
                blastColor.SetColorValue(0);
            }
            if (blast.TryGetComponent<IGun>(out var blastGun))
            {
                blastGun.SetAmmunition(0);
            }
            _firingSlots.Add(blast.gameObject);
        }
    }
}
