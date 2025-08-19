using HoangNam;
using Unity.Mathematics;
using UnityEngine;

public partial class GridWorld : MonoBehaviour
{
  [Header("Setting")]
  [Range(0, 10)]
  [SerializeField] int colorIndex;
  float3x3 _originalMatrix;
  public float3x3 OriginalMatrix { get { return _originalMatrix; } }
  float3x3 _rotatedMatrix;
  public float3x3 RotatedMatrix { get { return _rotatedMatrix; } }
  [Range(-90, 90)]
  [SerializeField] float degAroundX;
  public float DegAroundX { get { return degAroundX; } }
  [SerializeField] int2 gridSize;
  public int2 GridSize { get { return gridSize; } set { gridSize = value; } }
  [SerializeField] float2 gridScale;
  public float2 GridScale { get { return gridScale; } set { gridScale = value; } }
  public float2 Offset { get; private set; }

  private void Update()
  {
#if UNITY_EDITOR
    DrawGrid();
#endif
  }

  public void InitConvertedComponents()
  {
    _originalMatrix = HoangNam.Utility.GetMatrixWith(0);
    _rotatedMatrix = HoangNam.Utility.GetMatrixWith(degAroundX);
    Offset = new Vector2((gridSize.x - 1) / 2f, (gridSize.y - 1) / 2f);
  }

  public int ConvertGridPosToIndex(in int2 gridPos)
  {
    var index = gridSize.y * gridPos.x + gridPos.y;
    return index;
  }

  public int2 ConvertIndexToGridPos(in int index)
  {
    int x = (int)(uint)math.floor(index / gridSize.y);
    int y = index - (x * gridSize.y);
    return new int2(x, y);
  }

  public float3 ConvertGridPosToWorldPos(in int2 gridPos)
  {
    var A2 = gridPos;
    var O2 = Offset;
    var O2A2 = A2 - O2;
    var A1 = new float2(O2A2.x * gridScale.x, O2A2.y * gridScale.y);
    var worldPos = new float3(A1.x, A1.y, 0);
    return ConvertToRotated(worldPos);
  }

  public int2 ConvertWorldPosToGridPos(in float3 worldPos)
  {
    var O1A1 = ConvertToNonRotated(worldPos);
    var O2A2 = new float2(O1A1.x * 1 / gridScale.x, O1A1.y * 1 / gridScale.y);
    var A2 = Offset + new float2(O2A2.x, O2A2.y);
    int xRound = (int)math.round(A2.x);
    int yRound = (int)math.round(A2.y);
    var gridPos = new int2(xRound, yRound);
    return gridPos;
  }

  public int ConvertWorldPosToIndex(in float3 worldPos)
  {
    var grid = ConvertWorldPosToGridPos(worldPos);
    var index = ConvertGridPosToIndex(grid);
    return index;
  }

  public float3 ConvertIndexToWorldPos(in int index)
  {
    var grid = ConvertIndexToGridPos(index);
    var worldPos = ConvertGridPosToWorldPos(grid);
    return worldPos;
  }

  public bool IsPosOutsideAt(float3 worldPos)
  {
    int2 gridPos = ConvertWorldPosToGridPos(worldPos);
    return IsGridPosOutsideAt(gridPos);
  }

  public bool IsGridPosOutsideAt(int2 gridPos)
  {
    if (gridPos.x > gridSize.x - 1 || gridPos.x < 0) return true;
    if (gridPos.y > gridSize.y - 1 || gridPos.y < 0) return true;
    return false;
  }

  public float3 ConvertToRotated(in float3 worldPos)
  {
    return math.mul(worldPos, _rotatedMatrix) + (float3)transform.position;
  }

  /// <summary>
  /// return a world position without any rotating processes.
  /// </summary>
  /// <returns></returns>
  public float3 ConvertToNonRotated(in float3 worldPos)
  {
    return math.mul(worldPos - (float3)transform.position, math.inverse(_rotatedMatrix));
  }

  /// <summary>
  /// Only for debugging
  /// </summary>
  void DrawGrid()
  {
    for (int x = 0; x < gridSize.x; ++x)
    {
      for (int y = 0; y < gridSize.y; ++y)
      {
        var worldPos = ConvertGridPosToWorldPos(new int2(x, y));
        HoangNam.Utility.DrawQuad(worldPos, gridScale, degAroundX, (ColorIndex)colorIndex);
      }
    }
  }

  /// <summary>
  /// Only for debugging
  /// </summary>
  void OnDrawGizmosSelected()
  {
    var color = HoangNam.Utility.GetColorFrom((ColorIndex)colorIndex);
    Gizmos.color = color;

    var centerPos = transform.position;
    var rotatedMatrix = HoangNam.Utility.GetMatrixWith(degAroundX);
    var x = gridSize.x * gridScale.x;
    var y = gridSize.y * gridScale.y;
    var leftDir = new Vector3(-x / 2f, 0, 0);
    var bottomDir = new Vector3(0, -y / 2f, 0);
    var rightDir = new Vector3(x / 2f, 0, 0);
    var topDir = new Vector3(0, y / 2f, 0);
    Vector3 bottomLeftDir = math.mul(bottomDir + leftDir, rotatedMatrix);
    Vector3 bottomRightDir = math.mul(bottomDir + rightDir, rotatedMatrix);
    Vector3 topLeftDir = math.mul(topDir + leftDir, rotatedMatrix);
    Vector3 topRightDir = math.mul(topDir + rightDir, rotatedMatrix);

    var bottomLeftPos = centerPos + bottomLeftDir;
    var bottomRightPos = centerPos + bottomRightDir;
    var topLeftPos = centerPos + topLeftDir;
    var topRightPos = centerPos + topRightDir;

    var points = new Vector3[8]
    {
      bottomLeftPos,
      bottomRightPos,
      bottomRightPos,
      topRightPos,
      topRightPos,
      topLeftPos,
      topLeftPos,
      bottomLeftPos
    };

    Gizmos.DrawLineList(points);
  }
}
