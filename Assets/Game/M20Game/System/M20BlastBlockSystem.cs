using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;
public partial class M20LevelSystem
{
    [SerializeField] Transform blastSlots;
    [SerializeField] int maxAmmunition = 10;
    [SerializeField] int maxSlot = 8;
    int _currentSlot;
    Transform FindQueueSlotsPosParent(int currentSlot)
    {
        _currentSlot = currentSlot;
        if (_currentSlot > maxSlot) _currentSlot = maxSlot;

        foreach (Transform child in blastSlots)
        {
            if (child.name.Equals($"{_currentSlot}SlotPos"))
                return child;
        }
        return null;
    }
    public void SetAmmunitionBlastColorAt(int colorValue)
    {
        var blastIndexEmpty = FindIndexEmptyBlast();
        if (blastIndexEmpty == -1) return;

        var blast = _firingSlots[blastIndexEmpty];
        if (!blast.TryGetComponent(out IGun blastGun)) return;
        blastGun.SetAmmunition(maxAmmunition);
        if (!blast.TryGetComponent(out IColorBlock blastColor)) return;
        blastColor.SetColorValue(colorValue);
        if (!blast.TryGetComponent(out ISpriteRend spriteRend)) return;
        var duration = 0.3f;
        DOTween.To(
            () => 0f,
            (value) => spriteRend.GetBodyRenderer().material.SetFloat("_Saturation", value),
            1f, duration
        ).SetEase(Ease.InOutSine);
        SoundManager.Instance.PlayFullOfBulletSfx();
    }

    int FindIndexEmptyBlast()
    {
        for (int i = 0; i < _firingSlots.Count; i++)
        {
            var blast = _firingSlots[i];
            if (blast == null) continue;
            if (!blast.TryGetComponent(out IGun blastGun)) continue;
            if (blastGun.GetAmmunition() == 0) return i;
        }
        return -1;
    }
}