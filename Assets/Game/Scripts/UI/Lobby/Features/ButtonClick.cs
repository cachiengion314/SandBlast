using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SonDT
{
  public class ButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
  {
    [SerializeField] private Button SeftButton;
    [SerializeField] private Toggle SeftToggle;
    [SerializeField] private float _scaleOffset = 0.03f;
    [SerializeField] private bool _isCheckUpdateScale;

    private float _oriScale;

    private void Start()
    {
      _oriScale = transform.localScale.x;
    }

    public void SetOriScale()
    {
      _oriScale = transform.localScale.x;
    }
    public void SetScaleOne()
    {
      _oriScale = 1;
    }
    private void OnValidate()
    {
      SeftButton = gameObject.GetComponent<Button>();
      SeftToggle = gameObject.GetComponent<Toggle>();
    }

    private void Update()
    {
      if (_isCheckUpdateScale)
      {
        var a = Mathf.Clamp(transform.localScale.x, _oriScale - _scaleOffset, _oriScale);
        transform.localScale = Vector3.one * a;
      }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      if ((SeftButton != null && SeftButton.interactable == false) ||
          (SeftToggle != null && SeftToggle.interactable == false) ||
          (transform.localScale.x <= _oriScale - _scaleOffset)) return;
      DOTween.Kill(12345890);
      transform.DOScale(Vector3.one * -_scaleOffset, 0.1f).SetEase(Ease.InOutQuad).SetRelative(true).SetId(12345890);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      if ((SeftButton != null && SeftButton.interactable == false) ||
          (SeftToggle != null && SeftToggle.interactable == false)) return;
      // transform.DOScale(Vector3.one * _scaleOffset, 0.1f).SetEase(Ease.InOutQuad).SetRelative(true);
      transform.DOScale(_oriScale, 0.1f).SetEase(Ease.InOutQuad);
      // SoundManager.Instance.PlayOneShot(CommonSound.Click);
    }
  }
}