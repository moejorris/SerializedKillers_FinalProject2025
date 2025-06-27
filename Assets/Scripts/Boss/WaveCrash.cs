using UnityEngine;

public class WaveCrash : MonoBehaviour
{
    private Transform target;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float speed = 5f;
    private bool isMoving = false;

    private Vector3 moveDirection;

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController").gameObject.transform;
        if (target == null)
        {
            Debug.LogError("Player not found! Ensure the player has the 'Player' tag. Destroying the wave");
            Destroy(gameObject, 1f);
        }

        moveDirection = (target.position - transform.position).normalized; // Calculate the direction towards the player
        moveDirection.y = 0; // Ignore vertical changes to keep the movement horizontal
        moveDirection = moveDirection.normalized;
    }

    void Update()
    {
        // Have wave find player and move in that direction without turning or fixing itself
        if (target == null) return; // If target is not set, do nothing
        if (isMoving)
        {
            transform.position += moveDirection * speed * Time.deltaTime; // Move the wave towards the player
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player hit!");
            PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log("Player hit!");
                playerHealth.TakeDamage(damage);
            }
        }
    }

    void StartMoving()
    {
        isMoving = true; // Start moving the wave towards the player
    }

    void StartDestroying()
    {
        Destroy(gameObject, lifetime); // Schedule the wave for destruction after its lifetime
    }
}
