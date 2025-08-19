using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public partial class Tutorial
{
    [Space(20)]
    [Header("Tutorial Panel")]
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] GameObject exitBtn;
    Action OnExit;

    public void ShowTutorialPanelAt(string keyTutorial, string text, Vector3 pos, bool showBtn = false, Action OnExit = null)
    {
        this.keyTutorial = keyTutorial;
        this.OnExit = OnExit;

        panel.SetActive(true);
        panel.transform.localPosition = pos;
        exitBtn.SetActive(showBtn);
        textMesh.text = text;
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    public void Exit()
    {
        OnExit?.Invoke();
    }
}