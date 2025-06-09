using UnityEngine;

public class ThunderStrike : MonoBehaviour
{
    #region Unity Inspector Variables
    [Header("Thunder Strike Settings")]
    private Transform target; // Target to move towards
    private bool isChasing = true; // Flag to indicate if the thunder strike is chasing the target
    [SerializeField] private float speed = 5f; // Speed of the thunder strike



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
            direction.y = 0; // Ignore vertical difference to keep the strike horizontal
            transform.position += direction.normalized * speed * Time.deltaTime; // Move towards the target
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by the thunder strike!");
            // Damage effect logic can be added here
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

    #endregion
}
