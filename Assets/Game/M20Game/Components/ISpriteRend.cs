using UnityEngine;

public interface ISpriteRend
{
  public SpriteRenderer GetBodyRenderer();
  public void SetLayerName(string layerName);
  public int GetSortingOrder();
  public void SetSortingOrder(int sortingOrder);
}