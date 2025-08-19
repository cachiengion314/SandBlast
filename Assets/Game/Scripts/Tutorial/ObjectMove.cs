using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public partial class Tutorial
{
    [Space(20)]
    [Header("Object")]
    [SerializeField] Image obj;
    [SerializeField] Sprite handSpr;
    [SerializeField] Sprite arrowSpr;

    public void DoHandMoveAt(string keyTutorial, Vector2 startValue, Vector2 endValue, float angle = 0, float duration = 1f)
    {
        this.keyTutorial = keyTutorial;

        obj.sprite = handSpr;
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        obj.gameObject.SetActive(true);
        DoAnim(obj.transform, startValue, endValue, duration);
    }

    public void DoArrowMoveAt(string keyTutorial, Vector2 startValue, Vector2 endValue, float angle = 0, float duration = 1f)
    {
        this.keyTutorial = keyTutorial;

        obj.sprite = arrowSpr;
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        obj.gameObject.SetActive(true);
        DoAnim(obj.transform, startValue, endValue, duration);
    }

    public void DoHandMoveAt(string keyTutorial, int index, float angle = 0, float duration = 1f)
    {
        var posParent = posParents.GetChild(index);
        Vector2 startValue = posParent.GetChild(0).position;
        Vector2 endValue = posParent.GetChild(1).position;

        this.keyTutorial = keyTutorial;

        obj.sprite = handSpr;
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        obj.gameObject.SetActive(true);
        DoAnim(obj.transform, startValue, endValue, duration);
    }

    public void DoArrowMoveAt(string keyTutorial, int index, float angle = 0, float duration = 1f)
    {
        this.keyTutorial = keyTutorial;

        var posParent = posParents.GetChild(index);
        Vector2 startValue = posParent.GetChild(0).position;
        Vector2 endValue = posParent.GetChild(1).position;

        obj.sprite = arrowSpr;
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        obj.gameObject.SetActive(true);
        DoAnim(obj.transform, startValue, endValue, duration);
    }

    void DoAnim(Transform obj, Vector2 startValue, Vector2 endValue, float duration = 1f)
    {
        obj.DOKill();
        obj.position = startValue;
        obj.DOMove(endValue, duration)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart);
    }
    public void HideObject()
    {
        obj.transform.DOKill();
        obj.gameObject.SetActive(false);
    }
}