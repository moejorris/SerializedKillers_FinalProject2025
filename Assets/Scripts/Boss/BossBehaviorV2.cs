using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BossState { None, Fire, Electric, Water } // Enumeration for elemental states

public class BossBehaviorV2 : MonoBehaviour, IElemental, IDamageable, ITargetable
{
    #region Unity Variables

    [Header("Boss Settings")]
    [Tooltip("Max health of the boss")]
    [SerializeField] private float health = 100f;
    [Tooltip("Shield Health of the boss")]
    [SerializeField] private float maxShieldHealth = 30f;
    [Tooltip("Amount of damage the shield takes from the player")]
    [SerializeField] private float shieldDamage = 2.5f;
    private float shieldHealth = 0f;
    [Tooltip("Amount of damage the boss takes from the player")]
    [SerializeField] private float vulnerableDamage = 1f;
    [Tooltip("Multipler for the boss's weakness to the player's attack")]
    [SerializeField] private float weaknessMultiplier = 2f;
    [Tooltip("Attack Interval in seconds")]
    [SerializeField] private float attackInterval = 15f;
    private float attackTimer = 0f;
    [Tooltip("Vulnerable Duration in seconds")]
    [SerializeField] private float vulnerableDuration = 5f;
    private float vulnerableTimer = 0f;
    [Tooltip("Max number of attacks the boss can take before becoming invincible")]
    [SerializeField] private int maxVulnerableAttacks = 3; // Maximum number of attacks the boss can take while vulnerable
    [Tooltip("Max number of attacks before the boss can use before becoming vulnerable")]
    [SerializeField] private int maxAttacks = 3; // Maximum number of attacks before the boss becomes vulnerable
    [Tooltip("Radius for the keyboard smash attack")]
    [SerializeField] private float smashRadius = 5f; // Radius for the keyboard smash attack

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
    [Tooltip("Prefab for the heart object to spawn when the boss is hurt")]
    [SerializeField] private GameObject heartPrefab; // Prefab for the heart object to spawn when the boss is hurt
    [Tooltip("Keys that will fly off the keyboard when the boss slams his hands down")]
    [SerializeField] private GameObject[] keysToSmash; // Array of keys to smash
    [Tooltip("Animator for the boss's hands")]
    [SerializeField] private Animator handAnim; // Animator for the boss's hands
    [Tooltip("Particle effect for the keyboard smash attack")]
    [SerializeField] private GameObject keyboardSmashFX; // Particle effect for the keyboard smash attack
    private ParticleSystem keyboardSmashPS; // Particle system for the keyboard smash effect
    [Tooltip("Particle Effect for the boss spawning in")]
    [SerializeField] private GameObject bossSpawnFX;
    
    private ParticleSystem bossSpawnPS; // Particle system for the boss spawning in

    [Header("Shield Flash Effect")]
    [Tooltip("Shield Materials")]
    [SerializeField] private Material originalShieldMaterial;
    [SerializeField] private Material redFlashMaterial;
    [SerializeField] private Renderer shieldRenderer; // Assign the shield's renderer
    [SerializeField] private float flashDuration = 0.2f;
    private Coroutine currentFlashCoroutine;



    [Header("Testing Settings")]
    [Tooltip("Testing Flag")]
    [SerializeField] private bool testing = false;
    [Tooltip("Elemental State of the boss")]
    [SerializeField] private BossState currentState; // C rrent elemental state of the boss
    [Header("SFX")]
    [SerializeField] private SoundEffectSO sfx_keyboardClick;
    [SerializeField] private SoundEffectSO sfx_keyboardSmash;
    private Collider bossDoor;    

    private bool hasNoMoreAttacks = false;
    private Transform player; // Reference to the player's transform
    private bool lookAtPlayer = true; // Flag to control whether the boss should look at the player
    private Animator anim; // Reference to the boss's animator
    private float lastStateThreshold = 1f; // Last state threshold for the boss to change its state
    private bool isVulnerable = false; // Flag to control whether the boss is vulnerable
    private int vulnAttacks = 0; // Number of attacks the boss has taken while vulnerable
    private int attacksUsed = 0; // Number of attacks used by the boss
    private bool canTakeDamage = true;
    private bool isTransitioning = false;
    private bool isDead = false; // Flag to control whether the boss is dead
    private Renderer bossRenderer; // Reference to the boss's renderer for color changes
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool readyToFight = false; 
    private List<BossState> availableStates = new List<BossState>(); // List of available states for the boss
    private List<BossState> unusedStates = new List<BossState>(); // List of unused states for the boss
    private List<GameObject> spawnedEnemies;
    [Header("Targeting")]
    public float TargetScore { get; set; }
    public float TargetScoreWeight { get => 2f; } //boss is twice as likely to be targeted
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
        else
        {
            anim.enabled = false; // Disable the animator by default
            Debug.Log("Boss animator disabled until fight starts.");
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

        healthBar = GameObject.FindGameObjectWithTag("Canvas").transform.Find("BossHealth/Health").GetComponent<RectTransform>();

        keyboardSmashPS = keyboardSmashFX.GetComponent<ParticleSystem>();
        bossSpawnPS = bossSpawnFX.GetComponent<ParticleSystem>();

        bossDoor = GameObject.FindGameObjectWithTag("BossDoor").GetComponent<Collider>();
        bossDoor.isTrigger = false;

        bossRenderer = GetComponent<Renderer>();
        shieldHealth = maxShieldHealth; // Initialize the shield health to the maximum shield health
        SpawnBoss();

    }

    void Update()
    {
        if (!readyToFight) return;
        if (player == null) return; // Exit if the player is not found

        if (HasDefeatedPlayer())
        {
            OnPlayerDefeated();
            return;
        }

        if (testing)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Attack();
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
            if (Input.GetKeyDown(KeyCode.K))
            {
                PlayKeyboardClick();
            }
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
            if (isVulnerable) return; // Exit if the boss is vulnerable
            attackTimer += Time.deltaTime; // Increment the attack timer
            if (attackTimer >= attackInterval)
            {
                Debug.Log("Boss is attacking!"); // Log the attack
                Attack(); // Call the attack method when the attack timer reaches the attack interval
            }
            else if (attackTimer == 1f)
            {
                Debug.Log("Boss is attacking in one seccond");
            }
        }
    }

    void OnPlayerDefeated()
    {
        Debug.Log("Player defeated!");
        StopAllCoroutines();
        attackTimer = 0f;
        lookAtPlayer = false; // Stop the boss from looking at the player
        bossDoor.isTrigger = true; // Set the boss door to be a trigger

        Destroy(gameObject, 0.5f);
    }

    private bool HasDefeatedPlayer()
    {
        if (player == null) return false;

        Player_HealthComponent playerHealth = player.GetComponent<Player_HealthComponent>();
        if (playerHealth != null)
        {
            return playerHealth.isDead;
        }

        return false;
    }

    void LateUpdate()
    {
        if (!readyToFight) return;
        if (player == null) return; // Exit if the player is not found
        if (!lookAtPlayer) return; // Exit if the boss should not look at the player
        Vector3 direction = player.position - transform.position; // Calculate direction to the player
        direction.y = 0; // Ignore vertical difference
        Quaternion lookRotation = Quaternion.LookRotation(direction); // Create a rotation that looks at the player
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Smoothly rotate towards the target rotation
    }

    void StartFight()
    {
        Debug.Log("StartFight called");
        readyToFight = true;
        healthBar.parent.gameObject.SetActive(true);
        anim.enabled = true;
        attackTimer = 2.5f; // Start the attack timer
    }

    void SpawnBoss()
    {
        bossSpawnPS.Play();
        StartCoroutine(SpawnBossCoroutine());
    }


    IEnumerator SpawnBossCoroutine()
    {
        // lerp Scale boss to 13, 0.5, 5
        Vector3 targetScale = new Vector3(13f, 0.5f, 5f);
        Vector3 shurnkScale = new Vector3(0.01f, 0.01f, 0.01f);

        yield return null;
        transform.localScale = shurnkScale;
        float lerpDuration = 1f;
        float timeElapsed = 0f;
        Vector3 initialScale = transform.localScale;
        while (timeElapsed < lerpDuration)
        {
            float t = Mathf.Clamp01(timeElapsed / lerpDuration);
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
        StartFight();
    }
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red; // Set the color for the gizmo
            Gizmos.DrawLine(transform.position, player.position); // Draw a line from the boss to the player
        }

        // Draw the sphere for the keyboard smash
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, smashRadius);
    }

    // Call this when the shield takes damage
    private void FlashShieldRed()
    {
        if (shieldRenderer == null) return;

        // Stop any existing flash coroutine
        if (currentFlashCoroutine != null)
        {
            StopCoroutine(currentFlashCoroutine);
        }

        // Start new flash coroutine
        currentFlashCoroutine = StartCoroutine(FlashShieldCoroutine());
    }

    private IEnumerator FlashShieldCoroutine()
    {
        // Change to red material
        shieldRenderer.material = redFlashMaterial;

        // Wait for flash duration
        yield return new WaitForSeconds(flashDuration);

        // Change back to original material
        shieldRenderer.material = originalShieldMaterial;

        currentFlashCoroutine = null;
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

    public void PlaySound(SoundEffectSO clip, Transform target)
    {
        SoundManager.instance.PlaySoundEffectOnObject(clip, transform);
    }

    #endregion
    #region Elemental  Methods
    void ChangeElementalState(BossState newState)
    {
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
        SpawnWeakandStrongEnemy();

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
            Instantiate(attackPrefabs[1], new Vector3(player.position.x + Random.Range(-10f, 10f), player.position.y - 1f, player.position.z), Quaternion.identity); // Spawn the thunderbolt above the player
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
    public void KnockPlayerBack(float force)
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        dir.y = 0.5f;
        dir *= force;
        PlayerController.instance.ForceHandler.AddForce(dir, ForceMode.VelocityChange);
    }

    public void TakeDamage(float damage = 0)
    {
        if (PlayerController.instance?.CombatMachine?.isAttacking == true)
        {
            InteractElement();
        }
        else
        {
            Debug.Log("Boss can only take damage from player attacks.");
        }
    }

    void KeyboardSmash()
    {
        // Create a physics overlap sphere to detect the player, then send the player flying away
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, smashRadius, LayerMask.GetMask("Player")); // Get all colliders within the smash radius
        foreach (Collider hitCollider in hitColliders)
        {
            Debug.Log("Player hit by keyboard smash!"); // Log message for debugging
            Player_HealthComponent playerHealth = hitCollider.GetComponent<Player_HealthComponent>();
            KnockPlayerBack(Random.Range(20, 30)); // Knock the player back with a random force
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(2.5f); // Deal damage to the player
            }
        }

        // Play the keyboard smash sound
        PlayKeyboardSmash();
        // First check if the keysToSmash array is not empty
        if (keysToSmash.Length == 0)
        {
            Debug.LogError("keysToSmash array is empty. Please assign keys to smash.");
            return;
        }
        // Choose 5 radom keys from the keysToSmash array
        List<GameObject> keysToSmashList = new List<GameObject>(keysToSmash);
        List<GameObject> keysToSmashRandom = new List<GameObject>();
        for (int i = 0; i < 5 && keysToSmashList.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, keysToSmashList.Count); // Get a random index from the keysToSmashList
            GameObject keyToSmash = keysToSmashList[randomIndex]; // Get the key to smash
            // Debug.Log("Key to smash: " + keyToSmash.name); // Log the key to smash
            keysToSmashList.RemoveAt(randomIndex); // Remove the key from the list
            keysToSmashRandom.Add(keyToSmash); // Add the key to the list of keys to smash

            // Smash the keys
            keyboardSmashPS.Play(); // Play the keyboard smash particle effect
            foreach (GameObject key in keysToSmashRandom)
            {
                // Remove the key from the parent object
                key.transform.SetParent(null);
                Rigidbody rb = key.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    // Add a force to the keys to make them fly up
                    rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
                    rb.AddForce(Vector3.forward * 2f, ForceMode.Impulse);
                    Debug.Log("Key smashed: " + key.name); // Log the key that was smashed
                }
                Destroy(key, 2f); // Destroy the key after 2 seconds
            }
        }
    }

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
            if (currentState == BossState.None)
            {
                SpawnWeakandStrongEnemy(); // Spawn weak and strong enemies if the boss is in the None state
            }
        }
    }

    void FallToGround()
    {
        StartCoroutine(FallToGroundCoroutine());
    }

    private IEnumerator FallToGroundCoroutine()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                Vector3 startPosition = transform.position;
                Vector3 targetPosition = hit.point + Vector3.up * 1.5f;

                // Move slightly forward (in the boss's forward direction) while falling
                Vector3 forwardOffset = transform.forward * 3f; // Adjust 2f as needed for more/less forward movement
                targetPosition += forwardOffset;

                float distanceToGround = hit.distance;
                Debug.Log("Distance to ground: " + distanceToGround);
                float fallDuration = Mathf.Sqrt(distanceToGround / 5f);
                float elapsedTime = 0f;

                while (elapsedTime < fallDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / fallDuration;

                    t = t * t;

                    transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                    Debug.Log("Boss falling to ground");

                    yield return null;
                }

                transform.position = targetPosition;
                originalPosition = transform.position;
                originalRotation = transform.rotation;
                Debug.Log("Boss landed on ground");
            }
        }
    }

    void Die()
    {
        if (isDead) return; // If the boss is already dead, return
        SceneSwitcher.instance.Invoke("ReturnToMenu", 2.0f);
        Debug.Log("Boss died!");
        anim.SetTrigger("Die");
        isDead = true;
    }

    public void InteractElement(Behavior behavior = null)
    {
        if (!canTakeDamage || isTransitioning)
        {
            Debug.Log("Boss is transitioning or cannot take damage.");
            return;
        }

        bool isWeakness = CheckElementalWeakness(behavior);
        float damageAmount = 0f;

        // Handle shield damage first
        if (shieldHealth > 0)
        {
            damageAmount = shieldDamage;
            FlashShieldRed();

            if (isWeakness)
            {
                shieldHealth -= damageAmount * weaknessMultiplier; // Double the damage if the attack is a weakness
                Debug.Log("Weakness detected! Shield damage doubled.");
            }
            else
            {
                shieldHealth -= damageAmount;
                Debug.Log("Shield took " + damageAmount + " damage! Shield has " + shieldHealth + " health left!");
            }

            if (shieldHealth <= 0)
            {
                Debug.Log("Shield destroyed! Boss becoming vulnerable.");
                StartVulnerable();
            }
            damageAmount = 0f; // Reset the damage amount for the next check
            return;
        }

        if (!isVulnerable)
        {
            Debug.Log("Boss is not vulnerable. Attack had no effect to health.");
            return;
        }

        damageAmount = vulnerableDamage;
        if (isWeakness)
        {
            damageAmount *= weaknessMultiplier; // Double the damage if the attack is a weakness
            Debug.Log("Weakness detected! Damage doubled.");
        }

        PlayerController.instance.Mana.GainMana(5f);
        health -= damageAmount;
        vulnAttacks++;

        Debug.Log("Boss took damage: " + damageAmount + ". Current health: " + health); // Log the damage taken
        // Trigger Animations
        anim.SetTrigger("Hit"); // Trigger the hit animation
        StartCoroutine(ResetPositionAfterDelay(0.7f)); // Reset the boss's position and rotation after a delay

        UpdateUI();

        if (health <= 0)
        {
            Debug.Log("Boss has been defeated!");
            Die();
        }
    }


    private IEnumerator ResetPositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for specified delay
        StartCoroutine(SmoothResetPosition()); // Start the smooth reset coroutine
    }

    private IEnumerator SmoothResetPosition()
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        float duration = 0.15f;
        float elapsedTime = 0f;

        bool wasLookingAtPlayer = lookAtPlayer; // Store the current lookAtPlayer state
        lookAtPlayer = false; // Set lookAtPlayer to false

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = t * t;

            transform.position = Vector3.Lerp(startPosition, originalPosition, t);
            transform.rotation = Quaternion.Lerp(startRotation, originalRotation, t);

            yield return null; // Wait for the next frame
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        lookAtPlayer = wasLookingAtPlayer; // Restore the original lookAtPlayer state

        Debug.Log("Boss position and rotation reset to original values.");
    }

    private bool CheckElementalWeakness(Behavior behavior)
    {
        // Fix: Check behavior first, then behaviorName
        if (behavior == null || string.IsNullOrEmpty(behavior.behaviorName))
            return false;

        return currentState switch
        {
            BossState.Fire => behavior.behaviorName.ToLower() == "water",
            BossState.Water => behavior.behaviorName.ToLower() == "electric",
            BossState.Electric => behavior.behaviorName.ToLower() == "fire",
            BossState.None => false,
            _ => false
        };
    }

    void StartVulnerable()
    {
        if (isVulnerable) return; // Exit if the boss is already vulnerable
        shieldHealth = 0; // Reset shield health
        StopAllCoroutines(); // Stop any ongoing coroutines
        attackTimer = 0f; // Reset the attack timer
        anim.SetTrigger("Weak"); // Trigger the weak animation
        Debug.Log("Boss is about to become vulnerable!"); // Log the start of the vulnerable state
    }

    void EndVulnerable()
    {
        if (!isVulnerable) return; // Exit if the boss is not vulnerable
        BlockDamage(); // Block damage during transition
        isVulnerable = false; // Set the boss to not vulnerable
        vulnAttacks = 0; // Reset the number of attacks taken
        vulnerableTimer = vulnerableDuration; // Reset the vulnerable duration
        // handAnim.SetTrigger("Hand EndWeak"); // Trigger the end weak animation
        anim.SetTrigger("EndWeak"); // Trigger the end weak animation
        Debug.Log("Boss is no longer vulnerable!"); // Log the end of the vulnerable state
        anim.SetTrigger("Move"); // Trigger the move animation to teleport the boss after vulnerability ends
        shieldHealth = maxShieldHealth; // Reset shield health
        attacksUsed = 0;
        attackTimer = 0f;
        shieldRenderer.material = originalShieldMaterial; // Reset the shield material to the original
        // Invoke("ChangeState", 2f); // Change the state after a delay
    }

    void ChangeState()
    {
        float healthPercentage = health / 100f; // Calculate the health percentage
        if (healthPercentage <= 0.75f && lastStateThreshold > 0.75f)
        {
            SpawnHearts(3);
            lastStateThreshold = 0.75f;
            anim.SetTrigger("Change"); // Trigger the change animation
        }
        else if (healthPercentage <= 0.5f && lastStateThreshold > 0.5f)
        {
            SpawnHearts(3);
            lastStateThreshold = 0.5f;
            anim.SetTrigger("Change"); // Trigger the change animation
        }
        else if (healthPercentage <= 0.25f && lastStateThreshold > 0.25f)
        {
            SpawnHearts(3);
            lastStateThreshold = 0.25f;
            anim.SetTrigger("Change"); // Trigger the change animation
        }
        else if (healthPercentage <= 0f)
        {
            // Boss will play fake death animation, then reemerge then fight harder.
            Debug.Log("Boss has reached the second phase!"); // Log the second phase
            Destroy(gameObject, 5f); // Destroy the boss after 5 seconds
            return; // Exit the method
        }
    }

    void SpawnHearts(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Rigidbody rb = Instantiate(heartPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 5f, ForceMode.Impulse);
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
        anim.SetTrigger("Summon"); // Trigger the summon animation
        // handAnim.SetTrigger("Hand Summon"); // Trigger the hand summon animation
        attackTimer = 0f; // Reset the attack timer
        if (attacksUsed >= maxAttacks) // If the maximum number of attacks has been used
        {
            hasNoMoreAttacks = true;
            // attacksUsed = 0; // Reset the number of attacks used
            Debug.Log("Boss has no more attacks! Starting vulnerable state.");
        }
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
            if (count >= 4)
            {
                if (hasNoMoreAttacks)
                {
                    StartVulnerable(); // Start the vulnerable state
                    hasNoMoreAttacks = false;
                    Debug.Log("Boss has no more attacks! Starting vulnerable state.");
                }
            }
            attackTimer = 0f; // Reset the attack timer after each attack so the boss cannot attack while summoning projectiles
        }
    }

    void SpawnWeakandStrongEnemy()
    {
        if (enemySpawnPoints.Length < 2 || enemiesToSpawn.Length < 3)
        {
            Debug.LogError("Not enough enemy spawn points or enemy prefabs assigned!"); // Log error if not enough spawn points or prefabs are assigned
            return; // Exit the method if not enough spawn points or prefabs are assigned
        }

        int weaknessEnemyIndex = -1;
        int resistedEnemyIndex = -1;
        switch (currentState)
        {
            case BossState.Fire:
                resistedEnemyIndex = 0; // Fire resists itself, so it spawns a fire enemy
                weaknessEnemyIndex = 2; // Water is weak against fire, so it spawns a water enemy
                break;
            case BossState.Electric:
                resistedEnemyIndex = 1; // Electric resists itself, so it spawns an electric enemy
                weaknessEnemyIndex = 0; // Fire is weak against electric, so it spawns a fire enemy
                break;
            case BossState.Water:
                resistedEnemyIndex = 2; // Water resists itself, so it spawns a water enemy
                weaknessEnemyIndex = 1; // Electric is weak against water, so it spawns an electric enemy
                break;
            default:
                resistedEnemyIndex = Random.Range(0, enemiesToSpawn.Length); // Randomly select an enemy if no state is set
                weaknessEnemyIndex = Random.Range(0, enemiesToSpawn.Length); // Randomly select an enemy if no state is set
                break;
        }

        // Spawn the enemies at different spawn points
        int firstSpawnIndex = Random.Range(0, enemySpawnPoints.Length);
        int secondSpawnIndex;
        do
        {
            secondSpawnIndex = Random.Range(0, enemySpawnPoints.Length);
        } while (secondSpawnIndex == firstSpawnIndex);

        Instantiate(enemiesToSpawn[resistedEnemyIndex], enemySpawnPoints[firstSpawnIndex].position, Quaternion.identity);
        Instantiate(enemiesToSpawn[weaknessEnemyIndex], enemySpawnPoints[secondSpawnIndex].position, Quaternion.identity);
    }
    #endregion
    #region Animation Events
    void BlockDamage()
    {
        canTakeDamage = false;
        isTransitioning = true;
        Debug.Log("Boss damage blocked - transition started");
    }

    void AllowDamage()
    {
        canTakeDamage = true;
        isTransitioning = false;
        Debug.Log("Boss damage allowed - transition ended");
    }

    void StartVulnerableTransition()
    {
        BlockDamage(); // Block damage during transition
        Debug.Log("Starting vulnerable transition - damage blocked");
    }

    void CompleteVulnerableTransition()
    {
        AllowDamage();
        Debug.Log("Vulnerable transition complete - damage allowed");
    }

    void StartInvulnerableTransition()
    {
        BlockDamage(); // Block damage during transition
        Debug.Log("Starting invulnerable transition - damage blocked");
    }

    void CompleteInvulnerableTransition()
    {
        AllowDamage();
        Debug.Log("Invulnerable transition complete - damage allowed");
    }

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
        AllowDamage(); // Allow the boss to take damage
        Debug.Log("Boss is now vulnerable!");
    }

    void StopVulnerable()
    {
        isVulnerable = false;
        vulnAttacks = 0;
        shieldHealth = maxShieldHealth; // Reset shield if needed
        AllowDamage(); // Allow damage now that transition is complete
        Debug.Log("Boss is no longer vulnearble!");
    }

    void PlayKeyboardClick()
    {
        PlaySound(sfx_keyboardClick, transform); // Play the keyboard click sound effect
        Debug.Log("Keyboard click sound played.");
    }

    void PlayKeyboardSmash()
    {
        PlaySound(sfx_keyboardSmash, transform); // Play the keyboard smash sound effect
        Debug.Log("Keyboard smash sound played.");
    }
    #endregion
}