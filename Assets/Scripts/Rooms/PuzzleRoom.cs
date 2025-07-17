using System.Collections.Generic;
using UnityEngine;

public class PuzzleRoom : Room
{

    [Header("Puzzle Room Respawning")]
    [SerializeField] private List<GameObject> requiredEnemyTypes;
    [SerializeField] private List<Transform> requiredEnemyRespawnPoints;
    [SerializeField] private LayerMask enemyLayer;
    private List<GameObject> respawnEnemies = new List<GameObject>();

    public override void Start()
    {
        base.Start();
        OpenDoors(entranceDoors);
    }

    public override void RoomComplete()
    {
        base.RoomComplete();
        OpenDoors(exitDoors);
    }

    public override void CheckEnemies()
    {
        RemoveListNulls(respawnEnemies);

        for (int i = 0; i < requiredEnemyTypes.Count; i++)
        {
            if (requiredEnemyRespawnPoints[i].childCount <= 0)
            {
                SpawnEnemy(requiredEnemyTypes[i], requiredEnemyRespawnPoints[i]);
            }
        }
    }

    public override void SpawnEnemy(GameObject enemy, Transform position)
    {
        GameObject spawnedEnemy = Instantiate(enemy, position.position, Quaternion.identity);

        spawnedEnemy.transform.parent = position;

        Instantiate(smokeSpawnPrefab, position.position, Quaternion.identity);

        RemoveListNulls(respawnEnemies);
        respawnEnemies.Add(spawnedEnemy);
    }

    public override void BeginChallenge()
    {
        base.BeginChallenge();
        CloseDoors(entranceDoors);
    }
}
