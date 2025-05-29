using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform player; // Reference to the player's transform

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag assigned.");
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Make the boss look at the player
            Vector3 direction = player.position - transform.position; // Calculate direction to the player
            direction.y = 0; // Ignore vertical difference
            Quaternion rotation = Quaternion.LookRotation(direction); // Create a rotation that looks at the player
            
            // Directly set the rotation without smoothing
            transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0); // Keep the boss upright
        }
    }

    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red; // Set the color for the gizmo
            Gizmos.DrawLine(transform.position, player.position); // Draw a line from the boss to the player
        }
    }
}
