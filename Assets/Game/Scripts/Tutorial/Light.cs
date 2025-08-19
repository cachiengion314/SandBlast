using UnityEngine;

public partial class Tutorial
{
    [Header("Dark Panel")]
    [SerializeField] GameObject darkPanel;
    [SerializeField] SpriteMask lightPanel;
    [SerializeField] Sprite squareSpr;
    [SerializeField] Sprite circleSpr;

    public void ShowSquareLightAt(string keyTutorial, int index)
    {
        var posParent = posParents.GetChild(index);
        Vector3 pos = posParent.localPosition;
        Vector3 size = posParent.localScale;
        ShowSquareLightAt(keyTutorial, pos, size);
    }

    public void ShowCircleLightAt(string keyTutorial, int index)
    {
        var posParent = posParents.GetChild(index);
        Vector3 pos = posParent.localPosition;
        Vector3 size = posParent.localScale;
        ShowCircleLightAt(keyTutorial, pos, size);
    }

    public void ShowSquareLightAt(string keyTutorial, Vector3 pos, Vector3 size)
    {
        this.keyTutorial = keyTutorial;

        darkPanel.SetActive(true);
        lightPanel.sprite = squareSpr;
        lightPanel.transform.localPosition = pos;
        lightPanel.transform.localScale = size;
    }

    public void ShowCircleLightAt(string keyTutorial, Vector3 pos, Vector3 size)
    {
        this.keyTutorial = keyTutorial;

        darkPanel.SetActive(true);
        lightPanel.sprite = circleSpr;
        lightPanel.transform.localPosition = pos;
        lightPanel.transform.localScale = size;
    }

    public void HideLight()
    {
        darkPanel.SetActive(false);
    }
}