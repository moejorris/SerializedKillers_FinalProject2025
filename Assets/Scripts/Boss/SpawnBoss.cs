using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab; // Prefab of the boss to spawn
    [SerializeField] private Transform spawnPoint; // Point where the boss will spawn
    [SerializeField] private float spawnDelay = 2f; // Delay before spawning the boss
    private bool hasSpawned = false; // Flag to check if the boss has already spawned

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !hasSpawned)
        {
            hasSpawned = true; // Set the flag to true to stop multiple spawns
            if (bossPrefab != null && spawnPoint != null)
            {
                Debug.Log("Player entered the spawn area. Spawning boss..."); // Log the event
                gameObject.GetComponent<BoxCollider>().enabled = false; // Disable the collider to prevent further spawns
                Invoke(nameof(SpawnBossMethod), spawnDelay); // Invoke the SpawnBossMethod after the specified delay
            }
            else
            {
                Debug.LogError("Boss prefab or spawn point is not set!"); // Log an error if prefab or spawn point is not set
            }
        }
    }

    void SpawnBossMethod()
    {
        GameObject boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity); // Instantiate the boss at the spawn point
        Debug.Log("Boss spawned at " + spawnPoint.position); // Log the spawn position
        hasSpawned = false; // Set the flag to let the boss respawn if the player dies and respawns
    }
}
