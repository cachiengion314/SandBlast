using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class GridEditorControl : MonoBehaviour
{
    [SerializeField] ThemeObj themeObj;
    [SerializeField] SpriteRenderer bodyRenderer;
    public ColorBlockPartitionData data;
    public GirdEditorControlType type;
    public void OnValidate()
    {
        if (bodyRenderer == null) return;
        if (type == GirdEditorControlType.None)
            bodyRenderer.color = Color.white;
        if (type == GirdEditorControlType.Block)
            bodyRenderer.color = themeObj.colorValues[data.ColorValue];
    }
}

public enum GirdEditorControlType
{
    None,
    Block,
}
