using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class BlastBlockControl : MonoBehaviour
  , ISpriteRend
  , IColorBlock
  , IMoveable
  , IGun
  , IMuzzlePosition
{
  [Header("Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [SerializeField] TextMeshPro amountText;
  [SerializeField] Transform muzzlePosition;
  [Header("Datas")]
  int _index;
  int _colorValue;
  int _ammunition;
  float3 _lockedPosition;
  Transform _lockedTarget;
  float3 _initPosition;
  float3[] _path;

  public int GetColorValue()
  {
    return _colorValue;
  }

  public void SetColorValue(int colorValue)
  {
    _colorValue = colorValue;
    bodyRenderer.sprite = RendererSystem.Instance.GetBlastBlockAt(colorValue);
  }

  public void SetIndex(int index)
  {
    _index = index;
  }

  public int GetIndex()
  {
    return _index;
  }

  public void SetLockedPosition(float3 lockedPosition)
  {
    _lockedPosition = lockedPosition;
  }

  public float3 GetLockedPosition()
  {
    return _lockedPosition;
  }

  public Transform GetLockedTarget()
  {
    return _lockedTarget;
  }

  public void SetLockedTarget(Transform lockedTarget)
  {
    _lockedTarget = lockedTarget;
  }

  public void SetAmmunition(int ammunition)
  {
    _ammunition = ammunition;
    amountText.text = ammunition.ToString();
  }

  public int GetAmmunition()
  {
    return _ammunition;
  }

  public void SetInitPostion(float3 pos)
  {
    _initPosition = pos;
  }

  public float3 GetInitPostion()
  {
    return _initPosition;
  }

  public void SetPath(float3[] path)
  {
    _path = path;
  }

  public float3[] GetPath()
  {
    return _path;
  }

  public SpriteRenderer GetBodyRenderer()
  {
    return bodyRenderer;
  }

  public int GetSortingOrder()
  {
    return bodyRenderer.sortingOrder;
  }

  public void SetSortingOrder(int sortingOrder)
  {
    bodyRenderer.sortingOrder = sortingOrder;
  }

  public float3 GetMuzzlePosition()
  {
    return muzzlePosition.position;
  }

  public void SetLayerName(string layerName)
  {
    bodyRenderer.sortingLayerName = layerName;
  }
}