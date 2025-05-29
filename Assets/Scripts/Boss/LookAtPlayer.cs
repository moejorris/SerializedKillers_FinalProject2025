using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform player; // Reference to the player's transform
    private bool lookAtPlayer = true; // Flag to control whether the boss should look at the player
    private Quaternion targetRotation; // Target rotation for the boss

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag assigned.");
        }
        targetRotation = transform.rotation; // Initialize target rotation to the current rotation
    }

    void Update()
    {
        if (player != null)
        {
            if (lookAtPlayer)
            {
                Vector3 direction = player.position - transform.position; // Calculate direction to the player
                direction.y = 0; // Ignore vertical difference
                if (direction.sqrMagnitude > 0.01f) // Check if the player is not too close
                {
                    targetRotation = Quaternion.LookRotation(direction); // Create a rotation that looks at the player
                }
            }

            Quaternion yOnly = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // Keep the boss upright
            transform.rotation = Quaternion.Slerp(transform.rotation, yOnly, Time.deltaTime * 5f); // Smoothly rotate towards the target rotation
            // Make the boss look at the player
            // Vector3 direction = player.position - transform.position; // Calculate direction to the player
            // direction.y = 0; // Ignore vertical difference
            // Quaternion rotation = Quaternion.LookRotation(direction); // Create a rotation that looks at the player
            // transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f); // Increase speed

            // // Directly set the rotation without smoothing
            // transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0); // Keep the boss upright
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

    void StopLooking()
    {
        lookAtPlayer = false; // Set the flag to stop looking at the player
        targetRotation = transform.rotation; // Reset the target rotation to the current rotation
    }

    void StartLooking()
    {
        lookAtPlayer = true; // Set the flag to start looking at the player    
    }
}
