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
            direction.y = 0; // Ignore vertical difference to keep the strike horizontal
            transform.position += direction.normalized * speed * Time.deltaTime; // Move towards the target
        }
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
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
    }

    #endregion
}
