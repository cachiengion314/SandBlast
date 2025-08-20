using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public partial class M20LevelSystem
{
    [Header("ColorBlock Manager")]
    /// <summary>
    /// Manager all color blocks
    /// </summary>
    readonly Dictionary<int, List<GameObject>> _needMovingColorBlocks = new();
    ColorBlockControl[] _colorBlocks;
    public ColorBlockControl[] ColorBlocks { get { return _colorBlocks; } }
    [Range(.01f, 10f)]
    [SerializeField] float arrangeSpeed = 2.5f;
    [Range(.01f, 10f)]
    [SerializeField] float arrangeDampSpeed = 3.4f;

    bool ShouldAdd(GameObject colorBlockObj, GameObject blastBlock)
    {
        if (!colorBlockObj.TryGetComponent<IColorBlock>(out var colorBlock)) return false;
        if (!colorBlockObj.TryGetComponent<IDamageable>(out var damageable)) return false;
        if (damageable.IsDead()) return false;
        if (damageable.GetWhoPicked() != null && damageable.GetWhoPicked() != blastBlock)
            return false;
        if (damageable.GetWhoLocked() != null) return false; // the damageable block is waiting its dead when locked
        if (!blastBlock.TryGetComponent<IColorBlock>(out var dirColor)) return false;
        if (colorBlock.GetColorValue() != dirColor.GetColorValue()) return false;

        return true;
    }

    ColorBlockControl FindFirstBlockMatchedFor(GameObject blastBlock)
    {
        for (int x = 0; x < topGrid.GridSize.x; ++x)
        {
            var idx = topGrid.ConvertGridPosToIndex(new int2(x, 0));
            var obj = _colorBlocks[idx];
            if (obj == null) continue;
            if (!ShouldAdd(obj.gameObject, blastBlock)) continue;
            return obj;
        }
        return null;
    }

    int[] FindColorMatchedFor()
    {
        HashSet<int> availableColor = new();
        for (int x = 0; x < topGrid.GridSize.x; ++x)
        {
            var idx = topGrid.ConvertGridPosToIndex(new int2(x, 0));
            var obj = _colorBlocks[idx];
            if (obj == null) continue;
            if (!obj.TryGetComponent(out IColorBlock colorBlock)) continue;
            availableColor.Add(colorBlock.GetColorValue());
        }
        return availableColor.ToArray();
    }

    List<GameObject> FindColorBlocksMatchedFor(GameObject blastBlock)
    {
        var list = new List<GameObject>();

        var startX = 0;
        var firstHit = Physics2D.Raycast(
          (Vector2)blastBlock.transform.position,
          (Vector2)blastBlock.transform.up
        );
        if (
          firstHit.collider != null
          && firstHit.collider.TryGetComponent<IColorBlock>(out var color)
          && color.GetIndex() != -1
        )
            startX = topGrid.ConvertIndexToGridPos(color.GetIndex()).x;

        for (int x = startX; x < topGrid.GridSize.x; ++x)
        {
            var idx = topGrid.ConvertGridPosToIndex(new int2(x, 0));
            var obj = _colorBlocks[idx];
            if (obj == null) continue;
            if (!ShouldAdd(obj.gameObject, blastBlock)) continue;

            list.Add(obj.gameObject);
        }

        if (list.Count == 0)
        {
            for (int x = startX - 1; x >= 0; --x)
            {
                var idx = topGrid.ConvertGridPosToIndex(new int2(x, 0));
                var obj = _colorBlocks[idx];
                if (obj == null) continue;
                if (!ShouldAdd(obj.gameObject, blastBlock)) continue;

                list.Add(obj.gameObject);
            }
        }

        return list;
    }

    bool IsAtVisibleBound(GameObject colorBlock)
    {
        if (colorBlock.transform.position.y < 15.5f) return true;
        return false;
    }

    bool IsCollmunEmptyAt(int x)
    {
        for (int y = 0; y < topGrid.GridSize.y; ++y)
        {
            var grid = new int2(x, y);
            var idx = topGrid.ConvertGridPosToIndex(grid);
            var obj = _colorBlocks[idx];
            if (obj != null) return false;
        }
        return true;
    }

    List<int> FindNeedArrangeCollumns()
    {
        var list = new List<int>();
        for (int y = 0; y < 1; ++y)
        {
            for (int x = 0; x < topGrid.GridSize.x; ++x)
            {
                if (IsCollmunEmptyAt(x)) continue;
                var grid = new int2(x, y);
                var index = topGrid.ConvertGridPosToIndex(grid);
                var obj = _colorBlocks[index];
                if (obj != null) continue;

                list.Add(x);
            }
        }
        return list;
    }

    public void ArrangeColorBlocksUpdate()
    {
        var columnsToRemove = new List<int>();

        foreach (var kvp in _needMovingColorBlocks)
        {
            var needArrangeCollumn = kvp.Key;
            var needMovingObjs = kvp.Value;
            for (int i = needMovingObjs.Count - 1; i >= 0; --i)
            {
                var obj = needMovingObjs[i];
                if (!obj.TryGetComponent<IColorBlock>(out var colorBlock)) continue;
                if (!obj.TryGetComponent<IMoveable>(out var colorMoveable)) continue;

                var targetGridPos = topGrid.ConvertWorldPosToGridPos(colorMoveable.GetLockedPosition());
                var y = targetGridPos.y;
                HoangNam.Utility.InterpolateMoveUpdate(
                  obj.transform.position,
                  colorMoveable.GetInitPostion(),
                  colorMoveable.GetLockedPosition(),
                  updateSpeed * (arrangeSpeed + math.pow(math.E, -arrangeDampSpeed * y)),
                  out var t,
                  out var nextPos
                );

                if (IsAtVisibleBound(obj) && obj.activeSelf == false)
                    obj.SetActive(true);

                obj.transform.position = nextPos;
                if (t < 1) continue;

                var targetIndex = topGrid.ConvertWorldPosToIndex(colorMoveable.GetLockedPosition());
                _colorBlocks[targetIndex] = obj.GetComponent<ColorBlockControl>();
                colorBlock.SetIndex(targetIndex);

                needMovingObjs.Remove(obj);
            }

            if (needMovingObjs.Count == 0)
                columnsToRemove.Add(needArrangeCollumn);
        }

        foreach (var column in columnsToRemove)
            _needMovingColorBlocks.Remove(column);
    }

    public void FindNeedArrangeCollumnInUpdate()
    {
        var needArrangeCollumns = FindNeedArrangeCollumns();
        if (needArrangeCollumns.Count == 0) return;

        for (int x = 0; x < needArrangeCollumns.Count; ++x)
        {
            var needArrangeCollumn = needArrangeCollumns[x];
            if (_needMovingColorBlocks.ContainsKey(needArrangeCollumn)) continue;

            _needMovingColorBlocks.Add(needArrangeCollumn, new List<GameObject>());
            for (int y = 0; y < topGrid.GridSize.y; ++y)
            {
                var gridPos = new int2(needArrangeCollumn, y);

                var currentIndex = topGrid.ConvertGridPosToIndex(gridPos);
                var colorBlock = _colorBlocks[currentIndex];
                if (colorBlock == null) continue;
                if (!colorBlock.TryGetComponent<IMoveable>(out var moveable)) continue;

                var startPos = topGrid.ConvertGridPosToWorldPos(gridPos);
                var downGrid = gridPos + new int2(0, -1);
                var targetIndex = topGrid.ConvertGridPosToIndex(downGrid);
                var targetPos = topGrid.ConvertIndexToWorldPos(targetIndex);

                moveable.SetInitPostion(startPos);
                moveable.SetLockedPosition(targetPos);
                _needMovingColorBlocks[needArrangeCollumn].Add(colorBlock.gameObject);
                _colorBlocks[currentIndex] = null;
            }
        }
    }
}