using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossBehavior : MonoBehaviour
{
    #region Unity Inspector Variables
    [Header("Boss Components")]
    [Tooltip("Prefab for the projectile that the boss will spawn.")]
    [SerializeField] private GameObject projectilePrefab; // Prefab for the projectile
    [Tooltip("Teleport positions for the boss to use during the fight.")]
    [SerializeField] private Transform[] teleportPositions; // Array of teleport positions for the boss
    private Transform currentTeleportPosition; // Current teleport position for the boss
    [Tooltip("Check to enable testing boss behavior.")]
    [SerializeField] private bool isTesting = false; // Flag to enable testing mode
    private Transform player; // Reference to the player's transform
    private Animator anim; // Reference to the animator component for animations

    #endregion

    #region Unity Methods
    void Awake()
    {
        // Find the playr by tag and assign it to the player variable
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag assigned.");
        }

        // Get the Animator component attached to this GameObject
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator component not found! Make sure this GameObject has an Animator component attached.");
        }

        currentTeleportPosition = teleportPositions[0]; // Initialize the current teleport position to the first position in the array
        if (teleportPositions.Length == 0)
        {
            Debug.LogError("No teleport positions assigned! Please assign at least one teleport position in the inspector.");
        }
    }

    void Update()
    {
        if (isTesting)
        {
            // If testing mode is enabled, the boss will spawn a projetile when the P key is pressed
            if (Input.GetKeyDown(KeyCode.P))
            {
                anim.SetTrigger("Fall"); // Trigger the "Fall" animation
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                anim.SetTrigger("Shoot"); // Trigger the "Shoot" animation
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                anim.SetTrigger("Move"); // Trigger the "Move" Animation
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                TeleportBossAnimation(); // Teleport the boss to a new position
            }
        }
    }

    void SpawnProjectile()
    {
        if (projectilePrefab != null && player != null)
        {
            Instantiate(projectilePrefab, new Vector3(player.position.x, transform.position.y + 15, player.position.z), Quaternion.identity); // Spawn the projectile above the player
        }
        else
        {
            Debug.LogError("Projectile prefab or player reference is missing!"); // Log an error if the prefab or player is not set
        }
    }

    void TeleportBoss()
    {
        if (teleportPositions.Length > 0)
        {
            // Choose a random teleport position other than the current one
            int randomIndex = Random.Range(0, teleportPositions.Length);
            while (teleportPositions[randomIndex] == currentTeleportPosition)
            {
                randomIndex = Random.Range(0, teleportPositions.Length); // Keep choosing a new index until it's different
            }
            currentTeleportPosition = teleportPositions[randomIndex]; // Update the current teleport position
            transform.position = currentTeleportPosition.position; // Teleport the boss to the new position
            Debug.Log("Boss teleported to: " + currentTeleportPosition.position); // Log the teleport action
        }
    }

    #endregion

    #region Coroutine Methods

    private IEnumerator SpawnProjectileCoroutine(float delay)
    {
        int count = 0;
        while (count < 5)
        {
            yield return new WaitForSeconds(delay); // Wait for the specified delay
            SpawnProjectile(); // Call the method to spawn a projectile
            count++; // Increment the count 
        }
    }

    #endregion

    #region Anmation Methods

    void StartProjectileSpawn()
    {
        StartCoroutine(SpawnProjectileCoroutine(1f)); // Start the coroutine to spawn projectiles every 2 seconds
    }

    public void TeleportBossAnimation()
    {
        Debug.Log("Teleporting boss..."); // Log the teleport action
        TeleportBoss(); // Call the method to teleport the boss
    }

    void SaySomething()
    {
        Debug.Log("Boss says something!"); // Log a message when the boss says something
    }
    #endregion
}
