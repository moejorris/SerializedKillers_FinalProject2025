using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class BossBehaviorV2 : MonoBehaviour
{
    #region Unity Inspector Variables

    [Header("Boss Components")]
    [Tooltip("Boss Object Child")]
    [SerializeField] private GameObject bossObj; // Game Object refrence for boss child object
    [Tooltip("Prefab for the projectile that the boss will spawn.")]
    [SerializeField] private GameObject projectilePrefab; // Prefab for the projectile that the boss will use when spawning in objects
    [Tooltip("Teleport positions for the boss to use during the fight")]
    [SerializeField] private Transform[] teleportPositions; // Array of teleport positions for the boss to utilize
    private Transform currentTeleportPosition; // Current teleport position for the boss

    [Tooltip("String to display when testing functionality")]
    [SerializeField] private string testString; // String to use to test functionality

    [Tooltip("Check to enable testing boss behavior.")]
    [SerializeField] private bool isTesting = false; // Flag to enable testing mode
    private Transform player; //  Reference to the player's transform
    private Animator anim;

    #endregion
    #region Unity Methods
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' Tag Assigned to it");
        }

        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator component not found! Make sure this GameObject has an animator component connected!");
        }

        currentTeleportPosition = teleportPositions[0]; // Initialize the current teleport position
        if (teleportPositions.Length == 0)
        {
            Debug.LogError("No teleport positions assigned! Please assign at least 2 teleport positions in the inspector!");
        }
    }

    void Update()
    {
        if (isTesting)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                anim.SetTrigger("Fall"); // Trigger the "Fall" Animation
            }
            if(Input.GetKeyDown(KeyCode.O))
            {
                anim.SetTrigger("Shoot"); // Trigger the "Shoot" Animation
            }
            if(Input.GetKeyDown(KeyCode.I))
            {
                anim.SetTrigger("Move"); // Trigger the "Move" Animation
            }
        }
    }

    void SpawnProjectile()
    {
        if (projectilePrefab != null && player != null)
        {
            Instantiate(projectilePrefab, new Vector3(player.position.x, player.position.y + 15, player.position.z), Quaternion.identity); // Spawn the projectile above the player
        }
        else if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab refrence is missing!");
        }
        else if (player == null)
        {
            Debug.LogError("Player refrence is missing");
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
                randomIndex = Random.Range(0, teleportPositions.Length); // Keep choosing a new index until a new one is chosen
            }
            currentTeleportPosition = teleportPositions[randomIndex]; // Update the current teleport position
            bossObj.transform.position = currentTeleportPosition.position; // Teleport the boss to the new teleport point
            Debug.Log("Boss teleported to: " + currentTeleportPosition.position);
        }
    }
    
    #endregion
    #region Coroutine Methods

    private IEnumerator SpawnProjectileCoroutine(float delay)
    {
        int count = 0;
        while (count < 5)
        {
            yield return new WaitForSeconds(delay);
            SpawnProjectile(); // Call the method to spawn a projectile
            count++; // Increment the count
        }
    }

    #endregion

    #region Animation Methods

    void StartProjectileSpawn()
    {
        StartCoroutine(SpawnProjectileCoroutine(1f)); // Start the coroutine to start spawning in projectile prefabs
    }

    public void TeleportBossAnimation()
    {
        Debug.Log("Teleporting boss....");
        TeleportBoss(); // Call the method to teleport the boss
    }

    #endregion
}
