using UnityEngine;

public class WaveCrash : MonoBehaviour
{
    private Transform target;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float speed = 5f;

    private Vector3 moveDirection;

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target == null)
        {
            Debug.LogError("Player not found! Ensure the player has the 'Player' tag. Destroying the wave");
            Destroy(gameObject, 1f);
        }

        moveDirection = (target.position - transform.position).normalized; // Calculate the direction towards the player
        moveDirection.y = 0; // Ignore vertical changes to keep the movement horizontal
        moveDirection = moveDirection.normalized;
        Destroy(gameObject, lifetime); // Destroy the wave after its lifetime
    }

    void Update()
    {
        // Have wave find player and move in that direction without turning or fixing itself
        transform.position += moveDirection * speed * Time.deltaTime; // Move the wave towards the player
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by the wave crash! Player takes" + damage + " damage.");
        }
    }
}
