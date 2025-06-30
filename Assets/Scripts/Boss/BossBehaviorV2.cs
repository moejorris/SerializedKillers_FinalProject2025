using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BossState { None, Fire, Electric, Water } // Enumeration for elemental states

public class BossBehaviorV2 : MonoBehaviour, IDamageable
{
    #region Unity Variables
    [Header("Boss Settings")]
    [Tooltip("Max health of the boss")]
    [SerializeField] private float health = 100f;
    [Tooltip("Shield Health of the boss")]
    [SerializeField] private float shieldHealth = 30f;
    [Tooltip("Attack Interval in seconds")]
    [SerializeField] private float attackInterval = 10f;
    private float attackTimer = 0f;
    [Tooltip("Vulnerable Duration in seconds")]
    [SerializeField] private float vulnerableDuration = 5f;
    private float vulnerableTimer = 0f;
    // [Tooltip("Enemy Spawn Interval in seconds")]
    // [SerializeField] private float spawnInterval = 15f;
    // private float spawnTimer = 0f;
    [Tooltip("Max number of attacks the boss can take before becoming invincible")]
    [SerializeField] private int maxVulnerableAttacks = 3; // Maximum number of attacks the boss can take while vulnerable
    [Tooltip("Max number of attacks before the boss can use before becoming vulnerable")]
    [SerializeField] private int maxAttacks = 3; // Maximum number of attacks before the boss becomes vulnerable
    [Header("Boss Components")]
    [Tooltip("Reference to the boss health bar RectTransform.")]
    [SerializeField] private RectTransform healthBar; // Assign this in the inspector
    [Tooltip("Prefabs for enemies the boss will spawn in periodically")]
    [SerializeField] private GameObject[] enemiesToSpawn; // Array of enemies for the boss to spawn in
    [Tooltip("Spawn Points for enemies to spawn into")]
    [SerializeField] private Transform[] enemySpawnPoints; // Array of spawn positions
    [Tooltip("Prefabs for the boss's attacks")]
    [SerializeField] private GameObject[] attackPrefabs; // Array of attack prefabs
    [Tooltip("Teleport positions for the boss to teleport to")]
    [SerializeField] private Transform[] teleportPositions; // Array of teleport positions
    private Transform currentTeleportPosition; // Current teleport position
    [Tooltip("Wave Spawn Positions for the Water State")]
    [SerializeField] private Transform[] waveSpawnPositions; // Array of spawn positions for the water state attack
    [Tooltip("Boss Particle Effects")]
    [SerializeField] private GameObject[] particleEffects; // Array of particle effects for the boss
    [Header("Testing Settings")]
    [Tooltip("Testing Flag")]
    [SerializeField] private bool testing = false;
    [Tooltip("Boss Starts in Second Fight Phase")]
    [SerializeField] private bool secondPhase = false;
    [Tooltip("Elemental State of the boss")]
    [SerializeField] private BossState currentState; // Current elemental state of the boss
    private bool hasNoMoreAttacks = false;
    private Transform player; // Reference to the player's transform
    private bool lookAtPlayer = true; // Flag to control whether the boss should look at the player
    private Animator anim; // Reference to the boss's animator
    private bool isFollowing = false; // Flag to control whether the boss is hovering above the player
    private float lastStateThreshold = 1f; // Last state threshold for the boss to change its state
    private bool isVulnerable = false; // Flag to control whether the boss is vulnerable
    private int vulnAttacks = 0; // Number of attacks the boss has taken while vulnerable
    private int attacksUsed = 0; // Number of attacks used by the boss
    private Renderer bossRenderer; // Reference to the boss's renderer for color changes
    private List<BossState> availableStates = new List<BossState>(); // List of available states for the boss
    private List<BossState> unusedStates = new List<BossState>(); // List of unused states for the boss
    #endregion
    #region Unity Methods
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController").gameObject.transform; // Find the player by tag
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag assigned.");
        }

        anim = GetComponent<Animator>(); // Get the boss's animator component
        if (anim == null)
        {
            Debug.LogError("Animator component not found on the boss! Make sure the boss has an Animator component attached!");
        }

        currentTeleportPosition = teleportPositions[0]; // Initialize the current teleport position to the first position in the array
        if (teleportPositions.Length == 0) // Check if teleport positions are assigned
        {
            Debug.LogError("No teleport positions assigned! Please assign teleport positions in the inspector.");
            return;
        }

        if (attackPrefabs.Length == 0) // Check if attack prefabs are assigned
        {
            Debug.LogError("No Attack Prefabs assigned! Please assign attack prefabs in the inspector.");
            return;
        }
        bossRenderer = GetComponent<Renderer>(); // Get the boss's renderer component
    }

    void Update()
    {
        if (player == null) return; // Exit if the player is not found
        if (testing)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                anim.SetTrigger("Move");
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                anim.SetTrigger("Change");
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                StartVulnerable();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                anim.SetTrigger("Summon");
            }
            // if (Input.GetMouseButtonDown(0))
            // {
            //     TakeDamage(5f); // Simulate taking damage when the left mouse button is clicked
            //     Debug.Log("Boss took damage from mouse click!");
            // }
        }

        if (isVulnerable)
        {
            vulnerableTimer -= Time.deltaTime; // Decrease the vulnerable timer
            attackTimer = 0f; // Reset the attack timer while vulnerable
            if (vulnerableTimer <= 0f) // If the vulnerable timer has reached zero
            {
                EndVulnerable(); // End the vulnerable State
            }
        }

        if (health > 0f)
        {
            attackTimer += Time.deltaTime; // Increment the attack timer
            if (attackTimer >= attackInterval)
            {
                Attack(); // Call the attack method when the attack timer reaches the attack interval
            }
            else if (attackTimer == 1f)
            {
                Debug.Log("Boss is attacking in one seccond");
            }
        }


    }

    void LateUpdate()
    {
        if (player == null) return; // Exit if the player is not found
        if (!lookAtPlayer) return; // Exit if the boss should not look at the player
        Vector3 direction = player.position - transform.position; // Calculate direction to the player
        direction.y = 0; // Ignore vertical difference
        Quaternion lookRotation = Quaternion.LookRotation(direction); // Create a rotation that looks at the player
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Smoothly rotate towards the target rotation
    }

    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red; // Set the color for the gizmo
            Gizmos.DrawLine(transform.position, player.position); // Draw a line from the boss to the player
        }
    }

    void UpdateUI()
    {
        // Update the health bar by reducing its scale on the x-axis through its rect transform
        if (healthBar != null)
        {
            float healthPercentage = health / 100f; // Calculate the health percentage of the boss
            healthBar.localScale = new Vector3(healthPercentage, 1f, 1f); // Update the health bar scale based on the health percentage
        }
        else
        {
            Debug.LogError("Health bar reference not assigned! Please assign it in the inspector.");
        }
    }

    #endregion
    #region Elemental  Methods
    void ChangeElementalState(BossState newState)
    {
        SpawnEnemies();
        currentState = newState; // Set the current state to the new state
        switch (currentState)
        {
            case BossState.Fire:
                ChangeToFire();
                Debug.Log("Boss changed to Fire State!");
                break;
            case BossState.Electric:
                ChangeToElectric();
                Debug.Log("Boss changed to Electric State!");
                break;
            case BossState.Water:
                ChangeToWater();
                Debug.Log("Boss changed to Water State!");
                break;
            default:
                Debug.LogWarning("No elemental state set!"); // Log a warning if no state is set
                break;
        }
    }

    void SpawnFallingRocks()
    {
        if (attackPrefabs.Length > 0 && player != null)
        {
            Instantiate(attackPrefabs[3], new Vector3(player.position.x, player.position.y + 10f, player.position.z), Quaternion.identity); // Spawn the falling rocks above the player
        }
        else
        {
            Debug.LogError("Falling rocks attack prefab not assigned or player not found!"); // Log error if prefab is not assigned
        }
    }

    #region Fire State Methods
    void ChangeToFire()
    {
        currentState = BossState.Fire; // Set the current state to Fire
        Debug.Log("Boss changed to Fire State!"); // Log the state change
        bossRenderer.material.color = Color.red; // Change the boss's color to red
        foreach (GameObject effect in particleEffects)
        {
            effect.SetActive(false); // Deactivate all particle effects
        }
        particleEffects[0].SetActive(true);
    }

    void SpawnFirePillar()
    {
        if (attackPrefabs.Length > 0 && player != null)
        {
            Instantiate(attackPrefabs[0], new Vector3(player.position.x + Random.Range(-10f, 10f), player.position.y + 2, player.position.z + Random.Range(-10f, 10f)), Quaternion.Euler(-90, 0, 0)); // Spawn the fire pillar at a random position around the playe
        }
        else
        {
            Debug.LogError("Fire attack prefab not assigned or player not found!"); // Log error if prefab is not assigned
        }
    }
    #endregion

    #region Electric State Methods
    void ChangeToElectric()
    {
        currentState = BossState.Electric; // Set the current state to Electric
        Debug.Log("Boss changed to Electric State!"); // Log the state change
        bossRenderer.material.color = Color.yellow; // Change the boss's color to yellow
        foreach (GameObject effect in particleEffects)
        {
            effect.SetActive(false); // Deactivate all particle effects
        }
        particleEffects[1].SetActive(true); // Activate the electric particle effect
    }

    void SpawnThunderbolt()
    {
        if (attackPrefabs.Length > 0 && player != null)
        {
            Instantiate(attackPrefabs[1], new Vector3(player.position.x, player.position.y - 1f, player.position.z), Quaternion.identity); // Spawn the thunderbolt above the player
        }
        else
        {
            Debug.LogError("Electric attack prefab not assigned or player not found!"); // Log error if prefab is not assigned
        }
    }
    #endregion

    #region Water State Methods
    void ChangeToWater()
    {
        currentState = BossState.Water; // Set the current state to Water
        Debug.Log("Boss changed to Water State!"); // Log the state change
        bossRenderer.material.color = Color.blue; // Change the boss's color to blue
        foreach (GameObject effect in particleEffects)
        {
            effect.SetActive(false); // Deactivate all particle effects
        }
        particleEffects[2].SetActive(true);
    }

    void SpawnWaveCrash()
    {
        if (attackPrefabs.Length > 0 && player != null)
        {
            Transform waveSpawnPosition = waveSpawnPositions[Random.Range(0, waveSpawnPositions.Length)]; // Choose a random spawn point
            GameObject wave = Instantiate(attackPrefabs[2], waveSpawnPosition.position, Quaternion.identity); // Spawn the wave at the chosen position

            // Make the wave face the player
            if (wave != null)
            {
                Vector3 direction = (player.position - wave.transform.position).normalized; // Calculate direction to the player
                direction.y = 0; // Ignore vertical difference
                if (direction != Vector3.zero)
                {
                    wave.transform.rotation = Quaternion.LookRotation(direction); // Set the wave's rotation to face the player
                }
            }
        }
    }

    #endregion
    void CycleState()
    {
        unusedStates.Remove(currentState); // Remove the current state from the unused states list

        // If all states have been used, reset the list 
        if (unusedStates.Count == 0)
        {
            unusedStates = new List<BossState> { BossState.Fire, BossState.Electric, BossState.Water }; // Reset the unused states list
            unusedStates.Remove(currentState); // Remove the current state from the unused states list
        }

        int randomIndex = Random.Range(0, unusedStates.Count); // Get a random index from the unused states list
        BossState newState = unusedStates[randomIndex]; // Get the new state from the unused
        ChangeElementalState(newState); // Change the elemental state of the boss

        unusedStates.Remove(newState); // Remove the new state from the unused states list
        Debug.Log("Boss cycled to new state: " + newState); // Log the state change

        attackTimer = 0f; // Reset the attack timer after changing state
    }
    #endregion
    #region Boss Methods
    void StartTeleport()
    {
        StartCoroutine(TeleportAfterDelay(0.5f)); // Start the teleport coroutine
    }

    private IEnumerator TeleportAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the teleport delay
        TeleportBoss(); // Teleport the boss to a random position
    }
    void TeleportBoss()
    {
        if (teleportPositions.Length > 0)
        {
            int randomIndex = Random.Range(0, teleportPositions.Length); // Get a random index from the teleport positions array
            while (teleportPositions[randomIndex] == currentTeleportPosition) // Makes sure the boss doesn't teleport to the same point
            {
                randomIndex = Random.Range(0, teleportPositions.Length); // Get a new position
            }
            currentTeleportPosition = teleportPositions[randomIndex]; // Update the current teleport position
            transform.position = currentTeleportPosition.position; // Teleport the boss to the new position
            Debug.Log("Boss teleported to: " + currentTeleportPosition.position); // Log the teleport

            attackTimer = 0f; // Reset the attack timer after teleporting
        }
    }

    public void TakeDamage(float damage = 5, Player_ScriptSteal scriptSteal = null)
    {
        if (!isVulnerable)
        {
            damage = 5f;
            shieldHealth -= damage;
            Debug.Log("Shield took " + damage + " damage! Shield has " + shieldHealth + " health left");
            if (shieldHealth <= 0)
            {
                StartVulnerable();
            }
        }
        if (!isVulnerable) return; // Exit if the boss is not vulnerable
        damage = 5f;
        health -=  damage; // Reduce the boss's health by the damage amount
        vulnAttacks++;
        vulnerableTimer = vulnerableDuration; // Reset the vulnerable timer
        Debug.Log("Boss took damage: " + damage + ". Current health: " + health); // Log the damage taken
        anim.SetTrigger("Hit"); // Trigger the hit animation
        anim.SetInteger("Damage", vulnAttacks); // Increment the attack timer to allow multiple hits until boss is incivible again
        if (vulnAttacks >= maxVulnerableAttacks)
        {
            EndVulnerable(); // End the vulnerable state
        }
        UpdateUI();
    }

    void StartVulnerable()
    {
        if (isVulnerable) return; // Exit if the boss is already vulnerable
        isVulnerable = true;
        anim.SetTrigger("Weak"); // Trigger the weak animation
        Debug.Log("Boss is about to become vulnerable!"); // Log the start of the vulnerable state
    }

    void EndVulnerable()
    {
        if (!isVulnerable) return; // Exit if the boss is not vulnerable
        isVulnerable = false; // Set the boss to not vulnerable
        vulnAttacks = 0; // Reset the number of attacks taken
        vulnerableTimer = vulnerableDuration; // Reset the vulnerable duration
        anim.SetTrigger("EndWeak"); // Trigger the end weak animation
        Debug.Log("Boss is no longer vulnerable!"); // Log the end of the vulnerable state
        anim.SetTrigger("Move"); // Trigger the move animation to teleport the boss after vulnerability ends
        shieldHealth = 30f; // Reset shield health
        attacksUsed = 0;
        ChangeState(); // Call State Method
    }

    void ChangeState()
    {
        float healthPercentage = health / 100f; // Calculate the health percentage
        if (healthPercentage <= 0.75f && lastStateThreshold > 0.75f)
        {
            lastStateThreshold = 0.75f;
            anim.SetTrigger("Change"); // Trigger the change animation
        }
        else if (healthPercentage <= 0.5f && lastStateThreshold > 0.5f)
        {
            lastStateThreshold = 0.5f;
            anim.SetTrigger("Change"); // Trigger the change animation
        }
        else if (healthPercentage <= 0.25f && lastStateThreshold > 0.25f)
        {
            lastStateThreshold = 0.25f;
            anim.SetTrigger("Change"); // Trigger the change animation
        }
        else if (healthPercentage <= 0f)
        {
            // Boss will play fake death animation, then reemerge then fight harder.
            secondPhase = true; // Set the second phase flag to true
            Debug.Log("Boss has reached the second phase!"); // Log the second phase
            Destroy(gameObject, 5f); // Destroy the boss after 5 seconds
            return; // Exit the method
        }
    }

    void SummonAttack()
    {
        StopAllCoroutines(); // Stop any ongoing coroutines
        Debug.Log("Boss is summoning an attack!"); // Log the attack summon
        StartCoroutine(SummonProjectileCoroutine(2f)); // Start the summon projectile coroutine
    }

    void Attack()
    {
        attacksUsed++;
        if (attacksUsed >= maxAttacks) // If the maximum number of attacks has been used
        {
            hasNoMoreAttacks = true;
            attacksUsed = 0; // Reset the number of attacks used
            return; // Exit the attack method
        }
        anim.SetTrigger("Summon"); // Trigger the summon animation
        attackTimer = 0f; // Reset the attack timer
    }

    private IEnumerator SummonProjectileCoroutine(float delay)
    {
        for (int count = 0; count < 5; count++)
        {
            yield return new WaitForSeconds(delay); // Wait for the specified delay
            switch (currentState)
            {
                case BossState.Fire:
                    SpawnFirePillar(); // Spawn a fire pillar
                    break;
                case BossState.Electric:
                    SpawnThunderbolt(); // Spawn a thunderbolt
                    break;
                case BossState.Water:
                    SpawnWaveCrash(); // Spawn a wave crash
                    break;
                case BossState.None:
                    SpawnFallingRocks(); // Spawn falling rocks if no state is set
                    break;
                default:
                    Debug.LogWarning("No State is set! Cannot spawn elemental based projectile"); // Log a warning if no attack is spawned
                    break;
            }
        }
        if (hasNoMoreAttacks)
        {
            StartVulnerable();
            hasNoMoreAttacks = false;
        }
    }

    void SpawnEnemies()
    {
        foreach (Transform spawn in enemySpawnPoints)
        {
            int randomIndex = Random.Range(0, enemiesToSpawn.Length);
            Instantiate(enemiesToSpawn[randomIndex], spawn.position, spawn.rotation);
        }
        // spawnTimer = 0f;
    }
    #endregion
    #region Animation Events
    void StopLooking()
    {
        lookAtPlayer = false; // Set the flag to stop looking at the player
        Debug.Log("Boss stopped looking at the player.");
    }

    void StartLooking()
    {
        lookAtPlayer = true; // Set the flag to start looking at the player
        // Immediately look at the player when starting to look
        transform.LookAt(player); // Make the boss look at the player
        Debug.Log("Boss started looking at the player.");
    }

    void BecomeVulnerable() // Method to be called with an animation event
    {
        isVulnerable = true; // Set the boss to vulnerable
        vulnerableTimer = vulnerableDuration; // Reset the vulnerable timer
        vulnAttacks = 0; // Reset the number of attacks taken
    }
    #endregion
}