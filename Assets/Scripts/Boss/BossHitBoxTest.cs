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
}
