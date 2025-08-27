using Unity.Mathematics;
using UnityEngine;

public partial class M20LevelSystem
{
    [SerializeField] BulletControl bulletPref;
    [SerializeField] ColorBlockControl colorBlockPref;
    [SerializeField] ParticleSystem splashExplosionEfx;
    [SerializeField] YellowBloodExplosion2D colorSplashEfx;
    [SerializeField] BlastBlockControl blastBlockPref;
    public ColorBlockControl SpawnColorBlockAt(int index, Transform parent)
    {
        var pos = GridSystem.ConvertIndexToWorldPos(index, GridSize, GridScale, GridPos);
        var obj = Instantiate(colorBlockPref, parent);
        obj.transform.position = pos;
        return obj;
    }
    public ParticleSystem SpawnSplashExplosionEfx(float3 pos)
    {
        var obj = Instantiate(splashExplosionEfx, pos, splashExplosionEfx.transform.rotation);
        return obj;
    }

    public YellowBloodExplosion2D SpawnColorSplashEfxAt(float3 pos, int colorValue)
    {
        var obj = Instantiate(colorSplashEfx, pos, colorSplashEfx.transform.rotation);
        var color = RendererSystem.Instance.GetColorBy(colorValue);
        obj.SetColor(color);
        return obj;
    }

    public BulletControl SpawnBulletAt(float3 pos, float3 velocity, int _damage = 1)
    {
        var bullet = _bulletsPool.Get();
        bullet.transform.position = pos;
        bullet.SetVelocity(velocity);
        bullet.SetDamage(_damage);
        return bullet;
    }

    public BlastBlockControl SpawnBlastBlockAt(float3 pos, Transform parent)
    {
        var obj = Instantiate(blastBlockPref, parent);
        obj.transform.position = pos;
        return obj;
    }
}