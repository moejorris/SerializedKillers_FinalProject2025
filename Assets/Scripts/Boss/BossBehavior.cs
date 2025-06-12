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
public class BossBehavior : MonoBehaviour
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
    [Tooltip("Check to enable testing boss behavior.")]
    [SerializeField] private bool isTesting = false; // Flag to enable testing mode
    [Header("Boss Elemental States")]
    [SerializeField] private ElementalState currentState; // Current elemental state of the boss
    private Renderer keyboardRenderer; // Reference to the keyboard renderer for changing colors based on elemental state
    private Transform player; // Reference to the player's transform
    private Animator anim; // Reference to the animator component for animations

    private List<ElementalState> availableStates = new List<ElementalState>();
    private List<ElementalState> unusedStates = new List<ElementalState>(); // List to keep track of unused elemental states

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
        availableStates = new List<ElementalState> { ElementalState.Fire, ElementalState.Electric, ElementalState.Water};
        unusedStates = new List<ElementalState>(availableStates); // Initialize unusedStates with all available states
    }

    void Update()
    {
        if (isTesting)
        {
            // If testing mode is enabled, the boss will spawn a projetile when the P key is pressed
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
                Instantiate(projectilePrefabs[1], new Vector3(player.position.x + Random.Range(-5f, 5f), transform.position.y, player.position.z + Random.Range(-5f, 5f)), Quaternion.identity);
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
            Instantiate(projectilePrefabs[2], new Vector3(player.position.x, 0, player.position.z), Quaternion.identity); // Spawn the thunderbolt above the player
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
            GameObject wave = Instantiate(projectilePrefabs[3], new Vector3(player.position.x + Random.Range(-15f, 15f), 1, player.position.z + Random.Range(-15f, 15f)), Quaternion.identity);

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

    void Attack1()
    {
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
            }
        }
    }

    #endregion

    #region Anmation Methods

    void StartProjectileSpawn() // This method will be called by the animation event to start spawning projectiles
    {
        StartCoroutine(SpawnProjectileCoroutine(1f)); // Start the coroutine to spawn projectiles every 2 seconds
    }

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
    #endregion

    public void RandomElementalState()
    {
        // Remove the current state from the unused list if present
        unusedStates.Remove(currentState);

        // If all states have been used, reset the list (excluding the current state)
        if (unusedStates.Count == 0)
        {
            unusedStates = new List<ElementalState> { ElementalState.Fire, ElementalState.Electric, ElementalState.Water};
            unusedStates.Remove(currentState);
        }

        // Pick a random new state from the unused list
        int randomIndex = Random.Range(0, unusedStates.Count);
        ElementalState newState = unusedStates[randomIndex];
        ChangeElementalState(newState);

        // Remove the chosen state so it won't be picked again in this cycle
        unusedStates.Remove(newState);

        Debug.Log("Randomly changed to: " + newState);
    }
}