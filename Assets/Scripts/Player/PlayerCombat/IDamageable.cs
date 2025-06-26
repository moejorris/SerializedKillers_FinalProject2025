using UnityEngine;

public interface IDamageable
{
    public void TakeDamage();
    public void TakeDamage(float damage);
    public void TakeDamage(Player_ScriptSteal scriptSteal);
    public void TakeDamage(float damage, Player_ScriptSteal scriptSteal);
}
