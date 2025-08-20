using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  [SerializeField] QuadMeshSystem quadMeshSystem;
  [Range(1, 2000)]
  [SerializeField] int currentQuadAmount;

  void SpawnQuadMeshes(int amount)
  {
    quadMeshSystem.InitComponents();
  }

  void DisposeQuadMeshSystem()
  {
    quadMeshSystem.DisposeComponents();
  }

  void ControlQuadsInUpdate()
  {
    var amount = math.min(currentQuadAmount, quadMeshSystem.QuadCapacity);
    for (int i = 0; i < amount; ++i)
    {
      var pos = _quadPositions[i];
      var velocity = new float3(0, 0, 0);
      if (Input.GetKey(KeyCode.UpArrow)) velocity = new float3(0, 1, 0);
      if (Input.GetKey(KeyCode.DownArrow)) velocity = new float3(0, -1, 0);
      if (Input.GetKey(KeyCode.LeftArrow)) velocity = new float3(-1, 0, 0);
      if (Input.GetKey(KeyCode.RightArrow)) velocity = new float3(1, 0, 0);

      pos += 10 * Time.deltaTime * velocity;

      quadMeshSystem.OrderQuadMeshAt(i, pos, -1, -1);
      _quadPositions[i] = pos;
    }

    if (Input.GetKey(KeyCode.Space))
    {
      currentQuadAmount++;
    }

    if (Input.GetKey(KeyCode.C))
    {
      quadMeshSystem.OrderUVMappingAt(0, new float2(32, 32), new float2(64, 64));
    }

    quadMeshSystem.ApplyDrawOrders();
  }
}