using UnityEngine;

[CreateAssetMenu(fileName = "ThemeObj", menuName = "ScriptableObjects/ThemeObj", order = 0)]
public class ThemeObj : ScriptableObject
{
  [Header("Elements")]
  public Color[] colorValues;
}
