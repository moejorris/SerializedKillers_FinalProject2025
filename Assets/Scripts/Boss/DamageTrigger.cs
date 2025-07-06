using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private float damage = 10f; // Damage of the thunder strike
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player hit!");
            Player_HealthComponent playerHealth = other.GetComponent<Player_HealthComponent>();
            if (playerHealth != null)
            {
                Debug.Log("Player hit!");
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
