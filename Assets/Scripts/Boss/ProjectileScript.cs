using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private float lifeTime = 5f; // Lifetime of the projectile

    void Awake()
    {
        Destroy(gameObject, lifeTime); // Destroy the projectile after its lifetime
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Projectile hit the player!"); // Log message for debugging
            // If the projectile hits the player, destroy the projectile
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Projectile hit something!"); // Log message for debugging
            // If the projectile hits the ground, destroy it
            Destroy(gameObject);
        }
    }
}
