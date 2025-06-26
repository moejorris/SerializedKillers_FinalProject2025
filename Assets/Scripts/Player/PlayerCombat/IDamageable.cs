using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damage = 0, Player_ScriptSteal scriptSteal = null);
}
