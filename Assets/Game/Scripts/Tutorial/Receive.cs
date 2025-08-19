using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class Tutorial
{
    [Space(20)]
    [Header("Receive Panel")]
    [SerializeField] GameObject receiveModal;
    [SerializeField] Image receiveImg;
    [SerializeField] TextMeshProUGUI nameBooster;
    Action OnReceive;

    public void ShowReceivePanelAt(string keyTutorial, Sprite rewardSprite, string nameBooster, Action OnReceive = null)
    {
        this.keyTutorial = keyTutorial;
        this.OnReceive = OnReceive;

        receiveModal.SetActive(true);
        receiveImg.sprite = rewardSprite;
        this.nameBooster.text = nameBooster;
    }

    public void HideReceivePanel()
    {
        receiveModal.SetActive(false);
    }

    public void Receive()
    {
        OnReceive?.Invoke();
    }
}