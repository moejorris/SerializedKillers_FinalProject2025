using UnityEngine;

public class ThunderStrike : MonoBehaviour
{
    #region Unity Inspector Variables
    [Header("Thunder Strike Settings")]
    private Transform target; // Target to move towards
    private bool isChasing = true; // Flag to indicate if the thunder strike is chasing the target
    [Tooltip("Speed of the thunder strike")]
    [SerializeField] private float speed = 5f; // Speed of the thunder strike
    [Tooltip("Damage of the thunder strike")]
    [SerializeField] private float damage = 10f; // Damage of the thunder strike
    [Tooltip("Position of the thunder blast")]
    [SerializeField] private Transform thunderBlast; // Position of the thunder blast
    [Tooltip("Radius of the thunder strike")]
    [SerializeField] private float radius = 2f; // Radius of the thunder strike
    [Tooltip("Thunder FX Object")]
    [SerializeField] private GameObject thunderFX; // Thunder FX prefab
    [Header("Sound Effects")]
    [Tooltip("Thunder Strike Sound Effect")]
    [SerializeField] private SoundEffectSO sfx_thunderStrike; // Sound effect for the thunder strike
    private ParticleSystem thunderFXParticleSystem; // Reference to the particle system of the thunder FX

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

        // Refrence the particle system of the thunder FX
        thunderFXParticleSystem = thunderFX.GetComponent<ParticleSystem>();
    }

    void OnDrawGizmosSelected()
    {
        // Draw the sphere around the thunder strike
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(thunderBlast.position, radius);
    }

    void Update()
    {
        if (isChasing && target != null)
        {
            Vector3 direction = target.position - transform.position; // Calculate direction to the target
            transform.position += direction.normalized * speed * Time.deltaTime; // Move towards the target
            // Use a raycast ti check for the ground
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    // set the y position of the thunder strike to the ground
                    transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                }
            }
        }
    }

    public void PlaySound(SoundEffectSO clip, Transform transform)
    {
        SoundManager.instance.PlaySoundEffectOnObject(clip, transform); // Play the sound effect on the thunder strike object
        Debug.Log("Sound played: " + clip.name);
    }

    #endregion
    #region Animation Event Methods

    void DestroySelf()
    {
        Destroy(gameObject); // Destroy the thunder strike object
    }

    void StopChasing()
    {
        isChasing = false; // Stop chasing the target
    }

    void PlayThunderSFX()
    {
        PlaySound(sfx_thunderStrike, transform); // Play the thunder strike sound effect
        Debug.Log("Thunder strike sound played.");
    }

    void ThunderBlast()
    {
        // Create a physics overlap shpere to detect collsisions with the player
        Collider[] colliders = Physics.OverlapSphere(thunderBlast.position, radius, LayerMask.GetMask("Player"));
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log("Thunder strike hit the player!");
                Player_HealthComponent playerHealth = collider.GetComponent<Player_HealthComponent>();
                var scriptSteal = PlayerController.instance.ScriptSteal;
                bool hasThunderActive = scriptSteal.GetHeldBehavior() != null && scriptSteal.GetHeldBehavior().behaviorName == "thunder" && scriptSteal.BehaviorActive();
                if (hasThunderActive)
                {
                    Debug.Log("Player has electric active, no damage taken");
                    return;
                }

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
    }

    void PlayThunderFX()
    {
        // Play the thunder FX
        thunderFXParticleSystem.Play();
    }
    #endregion
}
