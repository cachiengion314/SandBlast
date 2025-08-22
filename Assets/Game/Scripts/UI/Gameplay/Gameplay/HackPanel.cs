using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public partial class GameplayPanel
{
    [SerializeField] TMP_InputField levelInput;
    [SerializeField] TMP_InputField coinInput;
    [SerializeField] TMP_InputField nameBoosterInput;
    [SerializeField] TMP_InputField amountBoosterInput;
    public void NextLevel()
    {
        if (!int.TryParse(levelInput.text, out int value)) return;
        if (value < 1) return;
        GameManager.Instance.CurrentLevelIndex = value - 1;
        SceneManager.LoadScene(KeyString.NAME_SCENE_GAMEPLAY);
    }

    public void AddCoin()
    {
        if (!int.TryParse(coinInput.text, out int value)) return;
        if (value < 0) return;
        GameManager.Instance.CurrentCoin = value;
    }

    public void AddBooster()
    {
        if (!int.TryParse(amountBoosterInput.text, out int value)) return;
        if (value < 0) return;
        if (nameBoosterInput.text.Equals("1")) GameManager.Instance.Booster1 = value;
        if (nameBoosterInput.text.Equals("2")) GameManager.Instance.Booster2 = value;
        if (nameBoosterInput.text.Equals("3")) GameManager.Instance.Booster3 = value;
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }
}