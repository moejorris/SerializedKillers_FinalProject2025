using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Notes for continuing We need to implement the following features: 
// 1. Implement the boss's elemental attacks 
// 2. Fix the boss animation errors
// 3. Implement the boss's damage system
// 4. Begin implementing makeing the boss fight on its own
// All of this should be completed before the end of the week, so we can start working more on fine tuning the boss fight and adding more features to it.

public enum  ElementalState{ None, Fire, Electric, Water } // Enum to represent the elemental states of the boss
public class BossBehavior : MonoBehaviour // Tried to inherit from EnemyAI_Base to use its functionality
{
    #region Unity Inspector Variables
    [Header("Boss Components")]
    [Tooltip("Prefab for the objects that the boss will spawn.")]
    [SerializeField] private GameObject[] projectilePrefabs; // Prefab for the objects that the boss will spawn
    [Tooltip("Boss object refrence for the boss behavior script.")]
    [SerializeField] private GameObject bossObject; // Reference to the boss object
    [Tooltip("Keyboard object refrence for the boss behavior to change colors depending on the elemental state.")]
    [SerializeField] private GameObject keyboardObject; // Reference to the keyboard object for changing colors based on elemental state
    [Tooltip("Teleport positions for the boss to use during the fight.")]
    [SerializeField] private Transform[] teleportPositions; // Array of teleport positions for the boss
    private Transform currentTeleportPosition; // Current teleport position for the boss
    [Tooltip("Positions for boss to spawn waves from.")]
    [SerializeField] private Transform[] waveSpawnPositions; // Array of positions for the boss to spawn waves from
    [Tooltip(" Current Health for the boss.")]
    [SerializeField] private float currentHealth = 100f; // Health for the boss
    private float lastStateThreshold = 1f; // Last state threshold for the boss to change states
    private bool canTakeDamage = true; // Flag to check if the boss can take damage
    [Header("UI Components")]
    [Tooltip("Reference to the boss health bar RectTransform.")]
    [SerializeField] private RectTransform healthBar; // Assign this in the inspector
    [Tooltip("Attack cooldown for the boss.")]
    [SerializeField] private float attackCooldown = 10f; // Cooldown time for the boss's attacks
    private float attackTimer = 0f; // Timer to track the attack cooldown
    private int teleportCounter = 0; // Counter to track the number of hits the boss has taken before teleporting
    [Tooltip("Check to enable testing boss behavior.")]
    [SerializeField] private bool isTesting = false; // Flag to enable testing mode
    [Header("Boss Elemental States")]
    [SerializeField] private ElementalState currentState; // Current elemental state of the boss
    private Renderer keyboardRenderer; // Reference to the keyboard renderer for changing colors based on elemental state
    private Transform player; // Reference to the player's transform
    private Animator anim; // Reference to the animator component for animations
    private LookAtPlayer lookAtPlayer; // Reference to the LookAtPlayer script for making the boss face the player
    private bool playerClose = false; // Flag to check if the player is close enough to the boss to deal damage

    private List<ElementalState> availableStates = new List<ElementalState>();
    private List<ElementalState> unusedStates = new List<ElementalState>(); // List to keep track of unused elemental states
    [SerializeField] private bool isFollowing = false; // Flag to check if the boss is following the player

    #endregion

    #region Unity Methods
    void Awake()
    {
        // Find the playr by tag and assign it to the player variable
        player = GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController").gameObject.transform;
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

        // Check if the projectile prefab is assigned
        if (projectilePrefabs.Length == 0)
        {
            Debug.LogError("Projectile prefabs not assigned! Please assign at least one projectile prefab in the inspector.");
            return; // Exit if no projectile prefabs are assigned
        }

        // Check if the boss object is assigned
        if (bossObject == null)
        {
            Debug.LogError("Boss object not assigned! Please assign the boss object in the inspector.");
            return; // Exit if the boss object is not assigned
        }

        // Get the Renderer component from the keyboard object to change its color based on elemental state
        keyboardRenderer = keyboardObject.GetComponent<Renderer>();
        if (keyboardRenderer == null)
        {
            Debug.LogError("Keyboard object does not have a Renderer component! Please add a Renderer component to the keyboard object.");
            return; // Exit if the keyboard object does not have a Renderer component
        }

        // Initialize availableStates with all elements except None
        availableStates = new List<ElementalState> { ElementalState.Fire, ElementalState.Electric, ElementalState.Water };
        unusedStates = new List<ElementalState>(availableStates); // Initialize unusedStates with all available states

        lookAtPlayer = GetComponentInChildren<LookAtPlayer>();
        if (lookAtPlayer == null)
        {
            Debug.LogError("LookAtPlayer component not found! Make sure the boss has a LookAtPlayer component attached.");
        }

        Debug.Log("Boss has " + currentHealth + " health and is ready for battle!"); // Log the initial health of the boss
    }

    void Update()
    {
        if (isTesting)
        {
            // If testing mode is enabled, the boss will spawn a projectile when the P key is pressed
            if (Input.GetKeyDown(KeyCode.I))
            {
                anim.SetTrigger("Move"); // Trigger the "Move" Animation
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                anim.SetTrigger("Change"); // Trigger the "Change" Animation
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                anim.SetTrigger("Attack"); // Trigger the "Attack" Animation
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                anim.SetTrigger("Swing"); // Trigger the "Swing" Animation
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                TeleportBoss(); // Teleport the boss when the Y key is pressed
            }
            if (Input.GetMouseButtonDown(0))
            {
                TakeDamage(2.5f); // Take damage when the left mouse button is clicked
            }
        }

        // Check if the boss's attack cooldown is over and if so attack the player
        if (currentHealth > 0)
        {
            if (attackTimer >= attackCooldown)
            {
                // Call the attack method or logic here
                anim.SetTrigger("Attack"); // Trigger the "Attack" Animation
                attackTimer = 0f; // Reset the attack timer
            }
            else
            {
                attackTimer += Time.deltaTime; // Increase the timer
            }
        }
    }

    void TakeDamage(float damage)
    {
        // Ignore this after we merge all health systems for enemies
        if (!canTakeDamage || currentHealth <= 0) return; // If the boss cannot take damage or is already dead, do nothing
        currentHealth -= damage; // Reduce the boss's health by the damage taken
        Debug.Log("Boss took " + damage + " damage! Remaining health: " + currentHealth); // Log the damage taken and remaining health
        // If the boss's health falls below a multiple of 25 or below that multiple, change the elemental state to a random state
        float healthPercentage = currentHealth / 100f; // Calculate the health percentage of the boss

        teleportCounter++; // Increase the teleport counter each time the boss takes damage
        Debug.Log("Boss will teleport after " + (3 - teleportCounter) + " more hits!"); // Log the number of hits left before teleporting
        if (teleportCounter >= 5 && teleportPositions.Length > 0)
        {
            teleportCounter = 0; // Reset the teleport counter
            anim.SetTrigger("Move"); // Trigger the "Teleport" Animation
            canTakeDamage = false; // Prevent taking damage until the teleport is complete
        }

        // Check if the boss is below the threshold for changing states
        if (healthPercentage <= 0.75f && lastStateThreshold > 0.75f)
        {
            lastStateThreshold = 0.75f;
            canTakeDamage = false; // Prevent taking damage until the state change is complete
            anim.SetTrigger("Change");
        }
        else if (healthPercentage <= 0.5f && lastStateThreshold > 0.5f)
        {
            lastStateThreshold = 0.5f;
            canTakeDamage = false; // Prevent taking damage until the state change is complete
            anim.SetTrigger("Change");
        }
        else if (healthPercentage <= 0.25f && lastStateThreshold > 0.25f)
        {
            lastStateThreshold = 0.25f;
            canTakeDamage = false; // Prevent taking damage until the state change is complete
            anim.SetTrigger("Change");
        }
        else if (currentHealth <= 0)
        {
            Debug.Log("Boss defeated!"); // Log the boss defeat
            anim.SetTrigger("Die"); // Trigger the "Die" Animation
            canTakeDamage = false; // Prevent taking damage until the state change is complete
            return; // Exit the method if the boss is defeated
        }

        UpdateUI(); // Update the UI to reflect the boss's current health
    }


    void UpdateUI()
    {
        // Update the health bar by reducing its scale on the x-axis through its rect transform
        if (healthBar != null)
        {
            float healthPercentage = currentHealth / 100f; // Calculate the health percentage of the boss
            healthBar.localScale = new Vector3(healthPercentage, 1f, 1f); // Update the health bar scale based on the health percentage
        }
        else
        {
            Debug.LogError("Health bar reference not assigned! Please assign it in the inspector.");
        }
    }

    #region Fire Attack Methods

    void ChangeToFire()
    {
        currentState = ElementalState.Fire; // Set the current state to fire
        Debug.Log("Boss changed to Fire state!"); // Log the change to fire state
        keyboardRenderer.material.color = Color.red; // Change the keyboard color to red for fire state
    }

    void SpawnFirePillar()
    {
        // Logic to spawn a fire pillar near the players position
        if (projectilePrefabs.Length > 1 && player != null)
        {
            Instantiate(projectilePrefabs[1], new Vector3(player.position.x + Random.Range(-10f, 10f), player.position.y + 2, player.position.z + Random.Range(-10f, 10f)), Quaternion.Euler(-90, 0, 0)); // Spawn the fire pillar at a random position around the player
        }
        else
        {
            Debug.LogError("Fire projectile prefab or player reference is missing!"); // Log an error if the prefab or player is not set
        }
    }

    #endregion
    #region Electric Attack Methods

    void ChangeToElectric()
    {
        currentState = ElementalState.Electric; // Set the current state to electric
        Debug.Log("Boss changed to Electric state!"); // Log the change to electric state
        keyboardRenderer.material.color = Color.yellow; // Change the keyboard color to yellow for electric state
    }

    void SpawnThunderbolt()
    {
        if (projectilePrefabs.Length > 2 && player != null)
        {
            Instantiate(projectilePrefabs[2], new Vector3(player.position.x, 97.85f, player.position.z), Quaternion.identity); // Spawn the thunderbolt above the player
        }
        else
        {
            Debug.LogError("Thunderbolt prefab or player reference is missing!"); // Log an error if the prefab or player is not set
        }
    }

    #endregion
    #region Water Attack Methods

    void ChangeToWater()
    {
        currentState = ElementalState.Water; // Set the current state to water
        Debug.Log("Boss changed to Water state!"); // Log the change to water state
        keyboardRenderer.material.color = Color.blue; // Change the keyboard color to blue for water state
    }

    void SpawnWaveCrash()
    {
        if (projectilePrefabs.Length > 0 && player != null)
        {
            // Spawn the wave crash projectile at a random position around the player but facing towards the player
            Transform waveSpawnPosition = waveSpawnPositions[Random.Range(0, waveSpawnPositions.Length)]; // Choose a random spawn position for the wave
            GameObject wave = Instantiate(projectilePrefabs[3], waveSpawnPosition.position, Quaternion.identity); // Instantiate the wave prefab at the chosen position

            // Make the wave face the player
            if (wave != null)
            {
                Vector3 direction = (player.position - wave.transform.position).normalized;
                direction.y = 0; // Keep the wave upright (no tilt)
                if (direction != Vector3.zero)
                {
                    wave.transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }
    }



    #endregion
    #region Non-Elemental Attack Methods

    void SpawnFallingRock()
    {
        if (projectilePrefabs.Length > 0 && player != null)
        {
            Instantiate(projectilePrefabs[0], new Vector3(player.position.x, player.position.y + 15f, player.position.z), Quaternion.identity); // Spawn the falling rock above the player
        }
        else
        {
            Debug.LogError("Falling rock prefab or player reference is missing!"); // Log an error if the prefab or player is not set
        }
    }
    #endregion
    void TeleportBoss()
    {
        if (teleportPositions.Length > 0)
        {
            if (teleportPositions.Length == 0) return; // If no teleport positions are assigned, exit the method
            // Choose a random teleport position other than the current one
            int randomIndex = Random.Range(0, teleportPositions.Length);
            while (teleportPositions[randomIndex] == currentTeleportPosition)
            {
                randomIndex = Random.Range(0, teleportPositions.Length); // Keep choosing a new index until it's different
            }
            currentTeleportPosition = teleportPositions[randomIndex]; // Update the current teleport position
            bossObject.transform.position = currentTeleportPosition.position; // Teleport the boss to the new position
            canTakeDamage = true; // Allow the boss to take damage again after teleporting
            Debug.Log("Boss teleported to: " + currentTeleportPosition.position); // Log the teleport action
        }
    }

    void CycleElementalState()
    {
        // Get the next state in the enum (skipping None)
        int next = ((int)currentState + 1) % 5; // 5 = number of state in the enum
        if (next == 0) next = 1; // Skip None State, go to fire
        Debug.Log("Cycling to next elemental state: " + (ElementalState)next); // Log the next state
    }

    void Attack1() // Called by the Attack Trigger in the Animator
    {
        StopAllCoroutines(); // Stop any ongoing coroutines to prevent multiple attacks at once
        Debug.Log("Boss is attacking!"); // Log the attack action
        StartCoroutine(SpawnProjectileCoroutine(2f)); // Start the coroutine to spawn projectiles every 2 seconds
    }

    #endregion

    #region Coroutine Methods

    private IEnumerator SpawnProjectileCoroutine(float delay)
    {
        for (int count = 0; count < 5; count++)
        {
            yield return new WaitForSeconds(delay); // Wait for the specified delay before spawning the next projectile
            switch (currentState)
            {
                case ElementalState.Fire:
                    SpawnFirePillar(); // Call the method to spawn a fire pillar
                    break;
                case ElementalState.Electric:
                    SpawnThunderbolt(); // Call the method to spawn a thunderbolt
                    break;
                case ElementalState.Water:
                    SpawnWaveCrash(); // Call the method to spawn a water projectile
                    break;
                case ElementalState.None:
                    SpawnFallingRock(); // Call the method to spawn a falling rock
                    break;
                default:
                    Debug.LogWarning("No elemental state set! Cannot spawn projectiles."); // Log a warning if no elemental state is set
                    break;
            }
        }
    }

    #endregion

    #region Anmation Methods

    public void TeleportBossAnimation() // This method will be called by the animation event
    {
        Debug.Log("Teleporting boss..."); // Log the teleport action
        TeleportBoss(); // Call the method to teleport the boss
    }

    void SaySomething() // This method is to be used to test animation events
    {
        Debug.Log("Boss says something!"); // Log a message when the boss says something
    }

    void ChangeElementalState(ElementalState newState) // This method will be called by the animation event to change the boss to fire state
    {
        currentState = newState; // Set the current state to the new state
        switch (currentState)
        {
            case ElementalState.Fire:
                ChangeToFire(); // Call the method to change to fire state
                Debug.Log("Boss changed to Fire state!"); // Log the change to fire state
                break;
            case ElementalState.Electric:
                ChangeToElectric(); // Call the method to change to electric state
                Debug.Log("Boss changed to Electric state!"); // Log the change to electric state
                break;
            case ElementalState.Water:
                ChangeToWater(); // Call the method to change to water state
                Debug.Log("Boss changed to Water state!"); // Log the change to water state
                break;
            default:
                Debug.LogWarning("No elemental state set!"); // Log a warning if no elemental state is set
                break;
        }
    }

    public void RandomElementalState()
    {
        // Remove the current state from the unused list if present
        unusedStates.Remove(currentState);

        // If all states have been used, reset the list (excluding the current state)
        if (unusedStates.Count == 0)
        {
            unusedStates = new List<ElementalState> { ElementalState.Fire, ElementalState.Electric, ElementalState.Water };
            unusedStates.Remove(currentState);
        }

        // Pick a random new state from the unused list
        int randomIndex = Random.Range(0, unusedStates.Count);
        ElementalState newState = unusedStates[randomIndex];
        ChangeElementalState(newState);

        // Remove the chosen state so it won't be picked again in this cycle
        unusedStates.Remove(newState);

        Debug.Log("Randomly changed to: " + newState);
        canTakeDamage = true; // Allow the boss to take damage again after changing state
    }

    void StopLooking()
    {
        if (lookAtPlayer != null)
        {
            lookAtPlayer.StopLooking(); // Call the method to stop looking at the player
        }
    }

    void StartLooking()
    {
        if (lookAtPlayer != null)
        {
            lookAtPlayer.StartLooking(); // Call the method to start looking at the player
        }
    }

    void DestroyBoss()
    {
        Debug.Log("Destroying boss..."); // Log the action of destroying the boss
        StopAllCoroutines(); // Stop any ongoing coroutines to prevent issues during destruction
        Destroy(gameObject); // Destroy the parent boss GameObject
    }
    #endregion
}