using UnityEngine;

public class FirePillar : MonoBehaviour, IDamageable, IElemental
{
    #region Unity Inspector Variables
    [Header("Fire Pillar Settings")]
    private Transform target; // Target to move towards
    private bool isChasing = true; // Flag to indicate if the fire pillar is chasing the target
    [SerializeField] private float speed = 5f; // Speed of the fire pillar
    [SerializeField] private float lifeTime = 3f; // Lifetime of the fire pillar
    [SerializeField] private float damage = 2.5f; // Damage of the fire pillar
    [SerializeField] private GameObject pathOfFire;
    [SerializeField] private float pathSpawnInterval = 0.5f; // Interval for spawning fire paths
    [SerializeField] private float damageInterval = 0.75f; // Interval for dealing damage to the player
    [SerializeField] private GameObject smokeObj;
    [SerializeField] private SoundEffectSO sfx_douseSFX; // Sound effect for extinguishing the fire pillar
    private ParticleSystem smokePS;
    private Animator anim; // Animator for the fire pillar
    private float pathTimer; // Timer for the fire path
    private bool isExtinguished = false; // Flag to indicate if the fire pillar is extinguished

    private float damageTimer = 0f;
    #endregion

    #region Unity Methods
    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController").gameObject.transform; // Find the player by tag
        if (target == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag assigned.");
            return; // Exit if the player is not found
        }
        anim = GetComponent<Animator>(); // Get the animator component
        smokePS = smokeObj.GetComponent<ParticleSystem>();
    }

    public void PlaySound(SoundEffectSO clip, Transform target)
    {
        SoundManager.instance.PlaySoundEffectOnObject(clip, target); // Play the sound effect on the fire pillar object
        Debug.Log("Sound played: " + clip.name);
    }



    void Update()
    {
        if (isChasing && target != null)
        {
            Vector3 direction = target.position - transform.position; // Calculate direction to the target
            transform.position += direction.normalized * speed * Time.deltaTime; // Move towards the target
        }

        // Spawn fire paths
        pathTimer += Time.deltaTime;
        if (pathTimer >= pathSpawnInterval)
        {
            SpawnFirePath();
            pathTimer = 0f;
        }
    }

    void SpawnFirePath()
    {
        if (pathOfFire != null)
        {
            Instantiate(pathOfFire, transform.position, Quaternion.identity); // Spawn the fire path at the current position
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                Debug.Log("Player hit by the fire pillar!");
                PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    var playerController = PlayerController.instance;
                    var scriptSteal = playerController.ScriptSteal;
                    bool hasFireActive = scriptSteal.GetHeldBehavior() != null && scriptSteal.GetHeldBehavior().behaviorName == "fire" && scriptSteal.BehaviorActive();
                    if (hasFireActive)
                    {
                        Debug.Log("Player has fire active, no damage taken");
                        return;
                    }
                    
                    if (isExtinguished) return; // If the fire pillar is extinguished, do not deal damage
                    playerHealth.TakeDamage(damage);
                }

                damageTimer = 0f; // Reset timer
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            damageTimer = 0f;
        }
    }

    void Extinguish()
    {
        anim.SetTrigger("Extinguish"); // Trigger extinguish animation
        isExtinguished = true; // Set the flag to indicate the fire pillar is extinguished
        smokePS.Play(); // Play the smoke particle system
        PlaySound(sfx_douseSFX, transform); // Play the extinguish sound effect
        Destroy(gameObject, 0.5f); // Destroy the fire pillar after the extinguish animation
    }
    #endregion

    #region Animation Event Methods
    void StartChasing()
    {
        isChasing = true; // Start chasing the target
        Destroy(gameObject, lifeTime); // Ensure the fire pillar is destroyed after its lifetime
    }

    public void TakeDamage(float damage)
    {
        InteractElement();
    }

    public void InteractElement(Behavior behavior = null)
    {
        // Check if the player has the water behavior active
        var playerController = PlayerController.instance;
        var scriptSteal = playerController.ScriptSteal;
        bool hasWaterActive = scriptSteal.GetHeldBehavior() != null && scriptSteal.GetHeldBehavior().behaviorName == "water" && scriptSteal.BehaviorActive();
        if (hasWaterActive)
        {
            Extinguish();
        }
    }
    #endregion
}

