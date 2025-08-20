using UnityEngine;

public interface IDamageable
{
  public void SetInitHealth(int health);
  public int GetHealth();
  public void SetHealth(int health);
  public bool IsDead();
  public bool IsDamage();
  public GameObject GetWhoLocked();
  public GameObject GetWhoPicked();
  public void SetWhoLocked(GameObject block);
  public void SetWhoPicked(GameObject block);
}