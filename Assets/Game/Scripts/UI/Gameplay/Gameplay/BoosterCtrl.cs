using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class BoosterCtrl : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI amountBooster;
    [SerializeField] GameObject UnlockBooster;
    [SerializeField] GameObject LockBooster;
    [SerializeField] GameObject EmptyBooster;

    public void SetAmountBooster(int amount)
    {
        amountBooster.text = amount.ToString();
    }

    public void Unlock()
    {
        UnlockBooster.SetActive(true);
        LockBooster.SetActive(false);
        EmptyBooster.SetActive(false);
    }

    public void Lock()
    {
        UnlockBooster.SetActive(false);
        LockBooster.SetActive(true);
        EmptyBooster.SetActive(false);
    }

    public void Empty()
    {
        UnlockBooster.SetActive(false);
        LockBooster.SetActive(false);
        EmptyBooster.SetActive(true);
    }
    public void PlayAnim()
    {
        var duration = 0.3f;
        transform.DOScale(new Vector3(1.2f, 1.2f,1f), duration)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Yoyo);
    }
    public void StopAnim()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
    }
}
