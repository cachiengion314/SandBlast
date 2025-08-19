using TMPro;
using UnityEngine;

public class VerticleBgCanvas : MonoBehaviour
{
  [SerializeField] Transform[] levels;

  private void Start()
  {
    SetupLevel();
  }

  void SetupLevel()
  {
    var currLevelNumber = GameManager.Instance.CurrentLevelIndex + 1;
    var lastLevelNumber = currLevelNumber - 2;
    for (int i = 0; i < levels.Length; ++i)
    {
      var currLevel = levels[i];
      var levelText = currLevel.GetComponentInChildren<TMP_Text>();
      if (lastLevelNumber + i <= 0)
      {
        currLevel.gameObject.SetActive(false);
      }
      levelText.text = (lastLevelNumber + i).ToString();
    }
  }
}
