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

  public int2 GetUVGridPosFrom(int colorValue)
  {
    var theme = GetCurrentTheme();
    var index = math.max(colorValue, 0);
    if (index > theme.UVGridPositions.Length - 1) return theme.UVGridPositions[^1];
    return theme.UVGridPositions[index];
  }

  public Color GetColorBy(int colorValue)
  {
    var theme = GetCurrentTheme();
    var index = math.max(colorValue, 0);
    if (index > theme.colorValues.Length - 1) return theme.colorValues[^1];
    return theme.colorValues[index];
  }

  public Sprite GetColorBlockAt(int colorValue)
  {
    var theme = GetCurrentTheme();
    if (colorValue < 0 || colorValue > theme.colorBlocks.Length - 1) return theme.colorBlocks[^1];
    return theme.colorBlocks[colorValue];
  }

  public Sprite GetBlastBlockAt(int colorValue)
  {
    var theme = GetCurrentTheme();
    if (colorValue < 0 || colorValue > theme.blastBlockSprites.Length - 1) return theme.blastBlockSprites[^1];
    return theme.blastBlockSprites[colorValue];
  }
}
