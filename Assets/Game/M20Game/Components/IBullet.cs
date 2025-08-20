using Unity.Mathematics;

public interface IBullet
{
  public void SetVelocity(float3 velocity);
  public float3 GetVelocity();
  public int GetDamage();
  public void SetDamage(int damage);
  public float GetLifeTimer();
  public void SetLifeTimer(float duration);
}