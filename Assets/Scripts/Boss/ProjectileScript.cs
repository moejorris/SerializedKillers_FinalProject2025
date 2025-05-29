using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Projectile hit the player!"); // Log message for debugging
            // If the projectile hits the player, destroy the projectile
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Projectile hit the ground!"); // Log message for debugging
            // If the projectile hits the ground, destroy it
            Destroy(gameObject);
        }
    }
}
