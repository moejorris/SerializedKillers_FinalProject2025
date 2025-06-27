using UnityEngine;

public class Player_HealthComponent : Health
{
    [SerializeField] PlayerHealth playerHealthUI;
    public override void TakeDamage(float damage = 0, Player_ScriptSteal scriptSteal = null)
    {
        base.TakeDamage(damage, scriptSteal);

        playerHealthUI.TakeDamage(damage);
    }

    public override void Heal(float healAmount = 0)
    {
        base.Heal(healAmount);

        playerHealthUI.HealDamage(healAmount);
    }
}
