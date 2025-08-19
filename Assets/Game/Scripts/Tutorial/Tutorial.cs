using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class Tutorial : MonoBehaviour
{
    [Header("Pos")]
    [SerializeField] Transform posParents;

    string keyTutorial = "";

    public bool IsTutorialAt(string keyTutorial)
    {
        return this.keyTutorial.Equals(keyTutorial);
    }

    public void StopTutorial()
    {
        this.keyTutorial = "";
    }
}
