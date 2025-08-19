using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class GridEditorControl : MonoBehaviour
{
    [SerializeField] ThemeObj themeObj;
    [SerializeField] SpriteRenderer bodyRenderer;
    public GirdEditorControlType type;
    public void OnValidate()
    {
        if (bodyRenderer == null) return;
        if (type == GirdEditorControlType.None)
            bodyRenderer.color = Color.white;
    }
}

public enum GirdEditorControlType
{
    None,
}
