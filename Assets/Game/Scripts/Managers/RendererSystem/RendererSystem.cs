using Unity.Mathematics;
using UnityEngine;

public class RendererSystem : MonoBehaviour
{
  public static RendererSystem Instance { get; private set; }

  [Header("Dependences")]
  [SerializeField] ThemeObj[] themes;
  int _currentThemeIndex = 0;

  void Start()
  {
    if (Instance == null)
      Instance = this;
    else Destroy(gameObject);
  }

  public ThemeObj GetCurrentTheme()
  {
    if (_currentThemeIndex >= 0 && _currentThemeIndex <= themes.Length - 1)
      return themes[_currentThemeIndex];
    return themes[0];
  }

  public Color GetColorBy(int colorValue)
  {
    var theme = GetCurrentTheme();
    var index = math.max(colorValue, 0);
    if (index > theme.colorValues.Length - 1) return theme.colorValues[^1];
    return theme.colorValues[index];
  }
}
