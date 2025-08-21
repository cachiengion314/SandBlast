using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public partial class M20LevelSystem
{
    void OnOutOfAmmunition(GameObject blast)
    {
        if (!blast.TryGetComponent(out IColorBlock colorBlast)) return;
        if (colorBlast.GetColorValue() == -1) return;
        colorBlast.SetColorValue(-1);

        if (!blast.TryGetComponent<ISpriteRend>(out var sprite)) return;
        var duration = 0.3f;
        DOTween.To(
            () => 1f,
            (value) => sprite.GetBodyRenderer().material.SetFloat("_Saturation", value),
            0f, duration
        ).SetEase(Ease.InOutSine);
        blast.transform.DORotate(new Vector3(0, 0, 0), duration)
             .SetEase(Ease.InOutSine); 
        if (blast.TryGetComponent(out IColorBlock color))
            SpawnColorSplashEfxAt(blast.transform.position, color.GetColorValue());
    }
    void OnColorBlockDestroyedByBullet(GameObject colorBlock)
    {
        SpawnSplashExplosionEfx(colorBlock.transform.position);

        var duration = .12f;
        colorBlock.transform
         .DOScale(.45f, duration)
         .OnComplete(() =>
         {
             SoundManager.Instance.PlayDestoyColorBlockSfx();
             if (colorBlock.TryGetComponent(out IColorBlock color))
                 SpawnColorSplashEfxAt(colorBlock.transform.position, color.GetColorValue());
             Destroy(colorBlock);
         });

        var neighbors = topGrid.FindNeighborsAt(colorBlock.transform.position);
        for (int i = 0; i < neighbors.Length; ++i)
        {
            var neighborPos = neighbors[i];
            if (neighborPos.Equals(new float3(-1, -1, -1))) continue;
            var idx = topGrid.ConvertWorldPosToIndex(neighborPos);
            var obj = _colorBlocks[idx];
            if (obj == null) continue;
            if (!obj.TryGetComponent<ISpriteRend>(out var rend)) continue;
            if (DOTween.IsTweening(rend.GetBodyRenderer().transform)) continue;

            rend.GetBodyRenderer()
              .transform.DOShakePosition(duration, .15f);
        }

        if (!colorBlock.TryGetComponent<ISpriteRend>(out var sprite)) return;
        sprite.SetSortingOrder(sprite.GetSortingOrder() + 2);
        var targetPos = colorBlock.transform.position + Vector3.up * .3f;
        sprite.GetBodyRenderer()
          .transform.DOMove(targetPos, duration);
    }

    void OnFireTarget(GameObject blastBlock, GameObject target)
    {
        if (!blastBlock.TryGetComponent<ISpriteRend>(out var sprite)) return;

        var originalScale = sprite.GetBodyRenderer().transform.localScale;
        sprite.GetBodyRenderer()
          .transform.DOScale(.8f * originalScale, fireRate / 2f)
          .SetLoops(2, LoopType.Yoyo);
    }
}
