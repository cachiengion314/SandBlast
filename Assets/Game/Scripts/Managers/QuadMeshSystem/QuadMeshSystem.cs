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
  public float2 ScaleSize;
  [Range(1, 2000)]
  public int QuadCapacity;
  [Header("Texture")]
  public float2 TextureResolution;
  public float2 LowerLeftBoundPosition;
  public float2 UpperRightBoundPosition;

  public void InitComponents()
  {
    vertices = new Vector3[4 * QuadCapacity];
    uv = new Vector2[4 * QuadCapacity];
    triangles = new int[6 * QuadCapacity];

    // Dispose existing NativeArrays if they exist
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
    mesh.vertices = vertices;
    mesh.uv = uv;
    mesh.triangles = triangles;
    meshFilter.mesh = mesh;
  }

  public void OrderUVMappingAt(int index, float2 lowerLeftBoundPos, float2 upperRightBoundPos)
  {
    uv[index * 4] = new float2(
      lowerLeftBoundPos.x / TextureResolution.x,
      lowerLeftBoundPos.y / TextureResolution.y
    );
    uv[index * 4 + 1] = new float2(
      lowerLeftBoundPos.x / TextureResolution.x,
      upperRightBoundPos.y / TextureResolution.y
    );
    uv[index * 4 + 2] = new float2(
      upperRightBoundPos.x / TextureResolution.x,
      upperRightBoundPos.y / TextureResolution.y
    );
    uv[index * 4 + 3] = new float2(
      lowerLeftBoundPos.x / TextureResolution.x,
      upperRightBoundPos.y / TextureResolution.y
    );
  }

  /// <summary>
  /// _lowerLeftBoundPos = -1, _upperRightBoundPos = -1 for a default value
  /// </summary>
  /// <param name="pos"></param>
  /// <param name="index"></param>
  /// <param name="_lowerLeftBoundPos"></param>
  /// <param name="_upperRightBoundPos"></param>
  public void OrderQuadMeshAt(
    int index,
    float3 pos,
    float2 _lowerLeftBoundPos,
    float2 _upperRightBoundPos
  )
  {
    vertices[index * 4]
      = pos + new float3(-.5f * ScaleSize.x, -.5f * ScaleSize.y, 0);
    vertices[index * 4 + 1]
      = pos + new float3(-.5f * ScaleSize.x, .5f * ScaleSize.y, 0);
    vertices[index * 4 + 2]
      = pos + new float3(.5f * ScaleSize.x, .5f * ScaleSize.y, 0);
    vertices[index * 4 + 3]
      = pos + new float3(.5f * ScaleSize.x, -.5f * ScaleSize.y, 0);

    triangles[index * 6] = index * 4;
    triangles[index * 6 + 1] = index * 4 + 1;
    triangles[index * 6 + 2] = index * 4 + 2;

    triangles[index * 6 + 3] = index * 4;
    triangles[index * 6 + 4] = index * 4 + 2;
    triangles[index * 6 + 5] = index * 4 + 3;

    var lowerLeftBoundPos = _lowerLeftBoundPos;
    if (lowerLeftBoundPos.Equals(-1)) lowerLeftBoundPos = LowerLeftBoundPosition;
    var upperRightBoundPos = _upperRightBoundPos;
    if (upperRightBoundPos.Equals(-1)) upperRightBoundPos = UpperRightBoundPosition;
    OrderUVMappingAt(index, lowerLeftBoundPos, upperRightBoundPos);
  }
}