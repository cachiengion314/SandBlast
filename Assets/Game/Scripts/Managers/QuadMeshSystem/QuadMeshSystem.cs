using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class QuadMeshSystem : MonoBehaviour
{
  [Header("Quad Meshes")]
  [SerializeField] MeshFilter meshFilter;
  Vector3[] vertices;
  Vector2[] uv;
  int[] triangles;
  Mesh mesh;
  /// <summary>
  /// Burst complie capable
  /// </summary>
  NativeArray<float3> _vertices;
  NativeArray<float2> _uv;
  NativeArray<int> _triangles;
  /// <summary>
  /// Settings
  /// </summary>
  public float2 QuadScale;
  [Range(1, 12800)]
  public int QuadCapacity;
  [Header("Texture")]
  public int2 GridResolution;
  public int2 TextureResolution;
  public int2 LowerLeftBoundPosition;
  public int2 UpperRightBoundPosition;

  public NativeArray<int2> GetTexBoundPositionsFrom(in int2 uvGridPos)
  {
    var amountTexEachGridInX = (int)math.floor(TextureResolution.x / (float)GridResolution.x);
    var amountTexEachGridInY = (int)math.floor(TextureResolution.y / (float)GridResolution.y);
    var lowerLeftTexPos = new int2(
      amountTexEachGridInX * uvGridPos.x, amountTexEachGridInY * uvGridPos.y
    );
    var upperRightTexPos = new int2(
      amountTexEachGridInX * (uvGridPos.x + 1), amountTexEachGridInY * (uvGridPos.y + 1)
    );

    var arr = new NativeArray<int2>(2, Allocator.Temp);
    arr[0] = lowerLeftTexPos;
    arr[1] = upperRightTexPos;
    return arr;
  }

  public int2 ConvertIndexToGridPos(in int index)
  {
    int x = index % GridResolution.x;
    int y = index / GridResolution.x;
    return new int2(x, y);
  }

  public int ConvertGirdPosToIndex(in int2 girdPos)
  {
    int index = girdPos.x + girdPos.y * GridResolution.x;
    return index;
  }

  public void InitComponents()
  {
    vertices = new Vector3[4 * QuadCapacity];
    uv = new Vector2[4 * QuadCapacity];
    triangles = new int[6 * QuadCapacity];

    if (_vertices.IsCreated) _vertices.Dispose();
    if (_uv.IsCreated) _uv.Dispose();
    if (_triangles.IsCreated) _triangles.Dispose();

    _vertices = new NativeArray<float3>(4 * QuadCapacity, Allocator.Persistent);
    _uv = new NativeArray<float2>(4 * QuadCapacity, Allocator.Persistent);
    _triangles = new NativeArray<int>(6 * QuadCapacity, Allocator.Persistent);

    mesh = new();
    var bounds = new Bounds
    {
      size = new(QuadCapacity, QuadCapacity),
      center = new(0, 0)
    };
    mesh.bounds = bounds;
  }

  public void DisposeComponents()
  {
    _vertices.Dispose();
    _uv.Dispose();
    _triangles.Dispose();
  }

  public void ApplyDrawOrders()
  {
    for (int i = 0; i < _vertices.Length; ++i)
    {
      vertices[i] = _vertices[i];
      uv[i] = _uv[i];
    }
    for (int i = 0; i < triangles.Length; ++i)
      triangles[i] = _triangles[i];

    mesh.vertices = vertices;
    mesh.uv = uv;
    mesh.triangles = triangles;
    meshFilter.mesh = mesh;
  }

  public void OrderUVMappingAt(int index, int2 uvGridPos)
  {
    using var arr = GetTexBoundPositionsFrom(uvGridPos);
    var lowerLeftBoundPos = arr[0];
    var upperRightBoundPos = arr[1];
    OrderUVMappingAt(index, lowerLeftBoundPos, upperRightBoundPos);
  }

  public void OrderUVMappingAt(int index, int2 lowerLeftBoundPos, int2 upperRightBoundPos)
  {
    _uv[index * 4] = new float2(
      lowerLeftBoundPos.x / (float)TextureResolution.x,
      lowerLeftBoundPos.y / (float)TextureResolution.y
    );
    _uv[index * 4 + 1] = new float2(
      lowerLeftBoundPos.x / (float)TextureResolution.x,
      upperRightBoundPos.y / (float)TextureResolution.y
    );
    _uv[index * 4 + 2] = new float2(
      upperRightBoundPos.x / (float)TextureResolution.x,
      upperRightBoundPos.y / (float)TextureResolution.y
    );
    _uv[index * 4 + 3] = new float2(
      lowerLeftBoundPos.x / (float)TextureResolution.x,
      upperRightBoundPos.y / (float)TextureResolution.y
    );
  }

  public void OrderQuadMeshAt(
    int index,
    float3 pos,
    int2 _uvGridPos
  )
  {
    _vertices[index * 4]
      = pos + new float3(-.5f * QuadScale.x, -.5f * QuadScale.y, 0);
    _vertices[index * 4 + 1]
      = pos + new float3(-.5f * QuadScale.x, .5f * QuadScale.y, 0);
    _vertices[index * 4 + 2]
      = pos + new float3(.5f * QuadScale.x, .5f * QuadScale.y, 0);
    _vertices[index * 4 + 3]
      = pos + new float3(.5f * QuadScale.x, -.5f * QuadScale.y, 0);

    _triangles[index * 6] = index * 4;
    _triangles[index * 6 + 1] = index * 4 + 1;
    _triangles[index * 6 + 2] = index * 4 + 2;

    _triangles[index * 6 + 3] = index * 4;
    _triangles[index * 6 + 4] = index * 4 + 2;
    _triangles[index * 6 + 5] = index * 4 + 3;

    if (_uvGridPos.Equals(-1))
    {
      OrderUVMappingAt(index, LowerLeftBoundPosition, UpperRightBoundPosition);
      return;
    }

    var arr = GetTexBoundPositionsFrom(_uvGridPos);
    OrderUVMappingAt(index, arr[0], arr[1]);
  }
}