using UnityEngine;
using UnityEngine.UI;

public class AutoSetSizeFooterLobby : MonoBehaviour
{
  [Header("Settings")]
  [SerializeField] GridLayoutGroup gridLayoutGroup;
  [SerializeField] int amountBtns;

  private void Start()
  {
    Canvas.ForceUpdateCanvases();

    var widthCam = gridLayoutGroup.GetComponent<RectTransform>().rect.width;
    var sizeX = widthCam / amountBtns;
    
    var _cellSize = gridLayoutGroup.cellSize;
    _cellSize.x = sizeX;
    gridLayoutGroup.cellSize = _cellSize;
  }
}
