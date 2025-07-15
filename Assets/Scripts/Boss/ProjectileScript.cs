using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private int damage = 10; // Damage dealt by the projectile
    private float lifeTime = 5f; // Lifetime of the projectile

    void Awake()
    {
        Destroy(gameObject, lifeTime); // Destroy the projectile after its lifetime
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player_HealthComponent playerHealth = collision.gameObject.GetComponent<Player_HealthComponent>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject); // Destroy the projectile on hit
        }
        else
        {
            Debug.Log("Projectile hit something!"); // Log message for debugging
            // If the projectile hits the ground, destroy it
            Destroy(gameObject);
        }
    }
}
