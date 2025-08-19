using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HomeLayout : MonoBehaviour
{
  public static HomeLayout Instance { get; private set; }

  [Header("Internal dependencies")]
  [SerializeField] RectTransform tabShopRect;
  [SerializeField] RectTransform tabHomeRect;
  [SerializeField] RectTransform tabRankingRect;
  [SerializeField] GridLayoutGroup gridLayoutGroup;

  private int _currentPage = -999;
  private int[] _pageIDs = new int[] { -1, 0, 1 };

  private void Awake()
  {
    Instance = this;
  }

  IEnumerator Start()
  {
    yield return null;
    gridLayoutGroup.enabled = false;

    HomeTouch();
  }

  public void OpenShopModal()
  {
    if (_currentPage == _pageIDs[0]) return;
    tabShopRect.gameObject.SetActive(true);
    tabHomeRect.gameObject.SetActive(false);
    tabRankingRect.gameObject.SetActive(false);
  }

  public void HomeTouch()
  {
    if (_currentPage == _pageIDs[1]) return;

    tabShopRect.gameObject.SetActive(false);
    tabHomeRect.gameObject.SetActive(true);
    tabRankingRect.gameObject.SetActive(false);
  }

  public void TouchTask()
  {
    if (_currentPage == _pageIDs[2]) return;

    tabShopRect.gameObject.SetActive(false);
    tabHomeRect.gameObject.SetActive(false);
    tabRankingRect.gameObject.SetActive(true);
  }
}
