using Unity.Collections;
using UnityEngine;

public partial class LevelSystem
{
    public void OnTriggerBooster1()
    {
        var quadAmount = _currentQuadAmount;
        for (int i = 0; i < slotsParent.childCount; i++)
        {
            if (IsSlotEmptyAt(i)) continue;
            var data = _shapeQuadDatas[i];
            // _currentQuadAmount = data.StartSpawnedQuadIndex;
            _shapeQuadDatas.Remove(i);
            var colorValue = GetRamdomColor();
            using var blockSlotPositions = GetRandomShape();
            OrderBlockShapeAt(i, blockSlotPositions, colorValue);
        }
        _currentQuadAmount = quadAmount;
        for (int i = 0; i < slotsParent.childCount; i++)
        {
            if (!IsSlotEmptyAt(i)) continue;
            var colorValue = GetRamdomColor();
            using var blockSlotPositions = GetRandomShape();
            OrderBlockShapeAt(i, blockSlotPositions, colorValue);
        }

        VisualizeActiveQuads();
        ApplyDrawOrders();
    }
}