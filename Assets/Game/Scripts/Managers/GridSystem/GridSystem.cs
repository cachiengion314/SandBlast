using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public static class GridSystem
{
    public static int ConvertGridPosToIndex(in int2 gridPos, int2 GridSize)
    {
        var index = GridSize.y * gridPos.x + gridPos.y;
        return index;
    }

    public static int2 ConvertIndexToGridPos(in int index, int2 GridSize)
    {
        int x = (int)(uint)math.floor(index / GridSize.y);
        int y = index - (x * GridSize.y);
        return new int2(x, y);
    }

    public static float3 ConvertGridPosToWorldPos(in int2 gridPos, int2 GridSize, float2 GridScale, float3 GridPos)
    {
        float2 Offset = new Vector2((GridSize.x - 1) / 2f, (GridSize.y - 1) / 2f);
        var A2 = gridPos;
        var O2 = Offset;
        var O2A2 = A2 - O2;
        var A1 = new float2(O2A2.x * GridScale.x, O2A2.y * GridScale.y);
        var worldPos = new float3(A1.x, A1.y, 0);
        return worldPos + GridPos;
    }

    public static int2 ConvertWorldPosToGridPos(in float3 worldPos, int2 GridSize, float2 GridScale, float3 GridPos)
    {
        float2 Offset = new Vector2((GridSize.x - 1) / 2f, (GridSize.y - 1) / 2f);
        var O1A1 = worldPos - GridPos;
        var O2A2 = new float2(O1A1.x * 1 / GridScale.x, O1A1.y * 1 / GridScale.y);
        var A2 = Offset + new float2(O2A2.x, O2A2.y);
        int xRound = (int)math.round(A2.x);
        int yRound = (int)math.round(A2.y);
        var gridPos = new int2(xRound, yRound);
        return gridPos;
    }

    public static int ConvertWorldPosToIndex(in float3 worldPos, int2 GridSize, float2 GridScale, float3 GridPos)
    {
        var grid = ConvertWorldPosToGridPos(worldPos, GridSize, GridScale, GridPos);
        var index = ConvertGridPosToIndex(grid, GridSize);
        return index;
    }

    public static float3 ConvertIndexToWorldPos(in int index, int2 GridSize, float2 GridScale, float3 GridPos)
    {
        var grid = ConvertIndexToGridPos(index, GridSize);
        var worldPos = ConvertGridPosToWorldPos(grid, GridSize, GridScale, GridPos);
        return worldPos;
    }

    public static bool IsPosOutsideAt(float3 worldPos, int2 GridSize, float2 GridScale, float3 GridPos)
    {
        int2 gridPos = ConvertWorldPosToGridPos(worldPos, GridSize, GridScale, GridPos);
        return IsGridPosOutsideAt(gridPos, GridSize);
    }

    public static bool IsGridPosOutsideAt(int2 gridPos, int2 GridSize)
    {
        if (gridPos.x > GridSize.x - 1 || gridPos.x < 0) return true;
        if (gridPos.y > GridSize.y - 1 || gridPos.y < 0) return true;
        return false;
    }

    public static NativeArray<float3> FindNeighborsAt(float3 worldPos, int2 GridSize, float2 GridScale, float3 GridPos)
    {
        using var directions = new NativeArray<int2>(
            new int2[]
            { new(1, 0), new(0, 1), new(-1, 0), new(0, -1)},
            Allocator.Temp);

        var neighbors = new NativeArray<float3>(directions.Length, Allocator.Temp);
        var gridPos = ConvertWorldPosToGridPos(worldPos, GridSize, GridScale, GridPos);
        for (int i = 0; i < directions.Length; ++i)
        {
            var dir = directions[i];
            var neighbor = gridPos + dir;
            if (IsGridPosOutsideAt(neighbor, GridSize))
            {
                neighbors[i] = new float3(-1, -1, -1);
                continue;
            }
            float3 wPos = ConvertGridPosToWorldPos(neighbor, GridSize, GridScale, GridPos);
            neighbors[i] = wPos;
        }
        return neighbors;
    }
}