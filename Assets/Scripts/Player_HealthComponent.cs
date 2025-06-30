using UnityEngine;

public class Player_HealthComponent : Health
{
    [SerializeField] PlayerHealth playerHealthUI => GameObject.FindGameObjectWithTag("Canvas").GetComponent<PlayerHealth>();
    public override void TakeDamage(float damage = 0, Player_ScriptSteal scriptSteal = null)
    {
        base.TakeDamage(damage, scriptSteal);

        if (currentHealth < 0) currentHealth = 0;

        playerHealthUI.TakeDamage(currentHealth);
    }

    public override void Heal(float healAmount = 0)
    {
        base.Heal(healAmount);

        playerHealthUI.HealDamage(currentHealth);
    }
}
