using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [Header("Room Config")]
    [SerializeField] private List<GameObject> requiredEnemyTypes;
    [SerializeField] private List<Transform> respawnPoints;
    BoxCollider box => GetComponent<BoxCollider>();
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private List<GameObject> currentEnemies;

    [SerializeField] private bool playerInRoom = false;
    private float timer;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(box.bounds.center, box.size);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        timer = 5;
    }

    private void Update()
    {
        if (playerInRoom)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 5;
                CheckEnemies();
            }
        }
    }

    public void CheckEnemies()
    {
        UpdateList();
        for (int i = 0; i < requiredEnemyTypes.Count; i++)
        {
            if (requiredEnemyTypes[i].transform.Find("Skull") != null)
            {
                if (!EnemyTypeExists(requiredEnemyTypes[i].transform.Find("Skull").GetComponent<EnemyAI_Base>().heldBehavior))
                {
                    SpawnEnemy(requiredEnemyTypes[i], respawnPoints[i]);
                }
            }
            else if (!EnemyTypeExists(requiredEnemyTypes[i].GetComponent<EnemyAI_Base>().heldBehavior))
            {

                SpawnEnemy(requiredEnemyTypes[i], respawnPoints[i]);
            }

        }
    }

    public void Initialize() // creates list from overlapsphere
    {
        Collider[] enems = Physics.OverlapBox(box.bounds.center, box.size / 2, Quaternion.identity, enemyLayer);
        foreach (Collider enemy in enems)
        {
            currentEnemies.Add(enemy.gameObject);
        }
    }

    public void SpawnEnemy(GameObject enemy, Transform position)
    {
        GameObject spawnedEnemy = Instantiate(enemy, position);
        UpdateList();
        currentEnemies.Add(spawnedEnemy);
    }

    public void UpdateList() // goes through and clears any null enemies that were killed
    {
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            if (currentEnemies[i] == null)
            {
                currentEnemies.RemoveAt(i);
            }
        }
    }

    public bool EnemyTypeExists(Behavior element) // checks if this enemy type exists
    {
        foreach (GameObject enemy in currentEnemies)
        {
            if (enemy.transform.Find("Skull") != null)
            {
                if (enemy.transform.Find("Skull").GetComponent<EnemyAI_Base>().heldBehavior == element)
                {
                    return true;
                }
            }
            else if (enemy.GetComponent<EnemyAI_Base>() && enemy.GetComponent<EnemyAI_Base>().heldBehavior == element)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player_HealthComponent>())
        {
            playerInRoom = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player_HealthComponent>())
        {
            playerInRoom = false;
        }
    }
}

