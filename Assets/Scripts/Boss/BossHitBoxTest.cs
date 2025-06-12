using UnityEngine;

public class BossHitBoxTest : MonoBehaviour
{
    [Tooltip("Amount of damage to maybe apply to the player when hit.")]
    [SerializeField] private int damage = 10; // Amount of damage to apply to the player

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the collider belongs to the player
        {
            Debug.Log("Player hit by attack! Applying damage: " + damage); // Log the hit and damage
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Check if the collided object is the player
        {
            Debug.Log("Player collided with attack! Applying damage: " + damage); // Log the collision and damage
        }
    }
}
