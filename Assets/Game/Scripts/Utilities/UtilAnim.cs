using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public enum AnimType
{
  Spin,
  Rotate,
  PopUp,
  Scale
}

public class UtilAnim : MonoBehaviour
{
  [Header("Settings")]
  [SerializeField] AnimType type;
  [Range(0, 50)]
  [SerializeField] float speed;

  [Header("Rotate setting")]
  [Range(-180, 180)]
  [SerializeField] float toRotateAngles;

  [Header("PopUp setting")]
  [Range(0, 10)]
  [SerializeField] float toScale;

  [Header("Scale setting")]
  [Tooltip("Value limit scale when animation zoomout")]
  [SerializeField] float3 defaultScale;
  [Tooltip("Value limit scale when animation zoomin")]
  [SerializeField] float3 maxScale;

  float _speedFactor = 20;

  private void Start()
  {
    StartAnim();
  }

  public void StartAnim()
  {
    if (type == AnimType.Spin)
    {
      Spin();
    }
    else if (type == AnimType.Rotate)
    {
      Rotate();
    }
    else if (type == AnimType.PopUp)
    {
      PopUp();
    }
    else if (type == AnimType.Scale)
    {
      Scale();
    }
  }

  void Spin()
  {
    LeanTween
      .rotateAround(gameObject, -Vector3.forward, 360f, _speedFactor * 1f / speed)
      .setLoopClamp();
  }

  void Rotate()
  {
    LeanTween.moveLocalY(gameObject, 1, _speedFactor * 1f / speed)
      .setEaseInOutElastic()
      .setLoopPingPong();
    LeanTween.rotateZ(gameObject, toRotateAngles, _speedFactor * 1f / speed)
      .setEaseInOutElastic()
      .setLoopPingPong();
  }

  void PopUp()
  {
    var seq = LeanTween.sequence();
    seq.insert(
      LeanTween.scaleX(gameObject, toScale, _speedFactor * 1f / speed)
        .setEaseOutElastic()
        .setLoopPingPong()
    );
    seq.insert(
      LeanTween.scaleY(gameObject, toScale, _speedFactor * 1f / speed)
        .setEaseOutElastic()
        .setLoopPingPong()
    );
  }

  void Scale()
  {
    DOTween.Kill(transform);
    transform.localScale = defaultScale;

    transform.DOScale(maxScale, _speedFactor * 1f / speed)
      // .SetEase(Ease.)
      .SetLoops(-1, LoopType.Yoyo);
  }
}
