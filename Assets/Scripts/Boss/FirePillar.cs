using UnityEngine;

public class FirePillar : MonoBehaviour
{
    #region Unity Inspector Variables
    [Header("Fire Pillar Settings")]
    private Transform target; // Target to move towards
    private bool isChasing = true; // Flag to indicate if the fire pillar is chasing the target
    [SerializeField] private float speed = 5f; // Speed of the fire pillar
    [SerializeField] private float lifeTime = 3f; // Lifetime of the fire pillar
    #endregion

    #region Unity Methods
    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform; // Find the player by tag
        if (target == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag assigned.");
            return; // Exit if the player is not found
        }
    }

    void Update()
    {
        if (isChasing && target != null)
        {
            Vector3 direction = target.position - transform.position; // Calculate direction to the target
            transform.position += direction.normalized * speed * Time.deltaTime; // Move towards the target
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // If the fire pillar collides with the player, apply damage or effects while inside the pillar
            Debug.Log("Player hit by the fire pillar!");
            // Here you can add code to apply damage or effects to the player
        }
    }
    #endregion

    #region Animation Event Methods
    void StartChasing()
    {
        isChasing = true; // Start chasing the target
        Destroy(gameObject, lifeTime); // Ensure the fire pillar is destroyed after its lifetime
    }
    #endregion
}

