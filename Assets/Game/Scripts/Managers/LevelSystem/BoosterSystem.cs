using Unity.Collections;
using UnityEngine;

public partial class LevelSystem
{
    public void OnTriggerBooster1()
    {
        for (int i = 0; i < slotsParent.childCount; i++)
        {
            if(!IsSlotEmptyAt(i)) RemoveBlockAt(i);
            var colorValue = GetRamdomColor();
            using var blockSlotPositions = GetRandomShape();
            OrderBlockShapeAt(i, blockSlotPositions, colorValue);
        }

        VisualizeActiveQuads();
        ApplyDrawOrders();
    }

    void RemoveBlockAt(int shapeIdx)
    {
        _shapeQuadDatas.Remove(shapeIdx);
        for (int i = 0; i < _quadDatas.Length; ++i)
        {
            var quadData = _quadDatas[i];
            if (quadData.GroupIndex != shapeIdx) continue;
            quadData.GroupIndex = -1;
            quadData.IsActive = false;
            _quadDatas[i] = quadData;
        }
        for (int i = 0; i < _blockDatas.Length; i++)
        {
            var blockData = _blockDatas[i];
            if (blockData.ShapeIndex != shapeIdx) continue;
            blockData.ShapeIndex = -1;
            _blockDatas[i] = blockData;
        }
    }
}