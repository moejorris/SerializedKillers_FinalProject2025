using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] protected float maxHealth = 10f;
    [SerializeField] protected float currentHealth = 100f;

    public virtual void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage = 0, Player_ScriptSteal scriptSteal = null)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float healAmount = 0)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public virtual void Die()
    {
        Debug.Log("Die!");
    }
}
