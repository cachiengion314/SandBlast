using DG.Tweening;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem
{
    public bool isTriggerBooster3 = false;
    public void OnTriggerBooster1()
    {
        for (int i = 0; i < slotsParent.childCount; i++)
        {
            if (!IsSlotEmptyAt(i)) RemoveBlockAt(i);
            var colorValue = GetRamdomColor();
            using var blockSlotPositions = GetRandomShape();
            OrderBlockShapeAt(i, blockSlotPositions, colorValue);
        }
        VisualizeActiveQuads();
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

    public void OnTriggerBooster2()
    {
        if (isRemoveQuadTweening) return;
        isRemoveQuadTweening = true;
        using var quadsMap = CollectLinkedQuadsRowAt(0, 16);
        var quadDatas = quadsMap.GetKeyArray(Allocator.Persistent);
        Sequence seq = DOTween.Sequence();
        float atPosition = 0f;
        VisualizeRemoveQuads(quadDatas, ref seq, ref atPosition);
        seq.InsertCallback(atPosition, () =>
        {
            RemoveQuadsFrom(quadDatas);
            FillBlastBlockAt(quadDatas);
            quadDatas.Dispose();
            isRemoveQuadTweening = false;
        });
    }

    public void OnTriggerBooster3()
    {
        if (isRemoveQuadTweening) return;
        var pos = new float3(userTouchScreenPosition.x, userTouchScreenPosition.y, 0);
        if (quadGrid.IsPosOutsideAt(pos)) return;
        var idxPos = quadGrid.ConvertWorldPosToIndex(pos);
        var startQuadIdx = _quadIndexPositionDatas[idxPos];
        if (startQuadIdx == -1) return;
        isRemoveQuadTweening = true;
        var quadData = _quadDatas[startQuadIdx];
        var colorValue = _groupQuadDatas[quadData.GroupIndex].ColorValue;
        GameplayPanel.Instance.ToggleBooster3();
        GameManager.Instance.Booster3--;

        using var quadsMap = CollectQuadsMatch(colorValue);
        var quadDatas = quadsMap.GetKeyArray(Allocator.Persistent);
        Sequence seq = DOTween.Sequence();
        float atPosition = 0f;
        VisualizeRemoveQuads(quadDatas, ref seq, ref atPosition);
        seq.InsertCallback(atPosition, () =>
        {
            RemoveQuadsFrom(quadDatas);
            FillBlastBlockAt(quadDatas);
            quadDatas.Dispose();
            isRemoveQuadTweening = false;
        });
    }
}