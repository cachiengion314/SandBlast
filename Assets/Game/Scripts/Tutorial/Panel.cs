using System;
using UnityEngine;
using UnityEngine.UI;

public partial class Tutorial
{
    [Space(20)]
    [Header("Dark Panel")]
    [SerializeField] Image tapPanel;
    Action OnTap;
    public void ShowTapPanelAt(string keyTutorial, bool isDrak = true, Action OnTap = null)
    {
        this.keyTutorial = keyTutorial;
        this.OnTap = OnTap;

        tapPanel.gameObject.SetActive(true);
        var color = tapPanel.color;
        if (isDrak) color.a = 0.8f;
        else color.a = 0f;
        tapPanel.color = color;
    }

    public void HideTapPanel()
    {
        tapPanel.gameObject.SetActive(false);
    }

    public void Tap()
    {
        OnTap?.Invoke();
    }
}