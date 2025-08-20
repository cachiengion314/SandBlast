using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public partial class M20LevelSystem
{
    void OnOutOfAmmunition(GameObject blast)
    {
        if (!blast.TryGetComponent<ISpriteRend>(out var sprite)) return;
        if (DOTween.IsTweening(sprite.GetBodyRenderer().transform)) return;

        // var duration = .2f;
        // sprite.GetBodyRenderer().DOColor(Color.gray, duration * 2f);
        // var originalScale = sprite.GetBodyRenderer().transform.localScale;
        // sprite.GetBodyRenderer().transform.DOScale(1.3f * originalScale, duration)
        //   .SetLoops(2, LoopType.Yoyo)
        //   .OnComplete(
        //     () =>
        //     {
        //         sprite.GetBodyRenderer().transform.DOScale(.7f * originalScale, duration)
        //     .SetLoops(2, LoopType.Incremental)
        //     .OnComplete(
        //       () =>
        //       {
        //             SoundManager.Instance.PlayDestoyShootingBlockSfx();

        //             ShakeCameraBy(new float3(.0f, -.25f, .0f));
        //             AddToShakeQueue(blast.transform.position);
        //             ShakeBottomGrid(blast.transform.position);
        //             if (blast.TryGetComponent(out IColorBlock color))
        //                 SpawnColorSplashEfxAt(blast.transform.position, color.GetColorValue());

        //             _firingSlots.Remove(blast);
        //             Destroy(blast);
        //         }
        //     );
        //     }
        //   );
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
