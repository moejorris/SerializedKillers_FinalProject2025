using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CombatRoom : Room
{
    [Header("Combat Room Respawning")]
    [SerializeField] private List<Transform> enemySpawnPoints;
    [SerializeField] private List<GameObject> enemiesTypesToSpawn;
    private List<GameObject> combatEnemies = new List<GameObject>();
    [SerializeField] private Transform combatCamPosition;



    public override void Start()
    {
        roomCheckTimer = 10;
        OpenDoors(entranceDoors); // by default they're open
    }

    public override void CheckEnemies()
    {
        //RemoveListNulls(combatEnemies);
        bool enemiesAlive = false;

        foreach (GameObject enemy in combatEnemies)
        {
            if (enemy == null) Debug.Log("Enemy Null");
            else enemiesAlive = true;
        }

        if (!enemiesAlive)
        {
            // enemies all dead (unless count starts at 0 freaking LAME!)
            RoomComplete();
        }
    }

    public override void SpawnEnemy(GameObject enemy, Transform position)
    {
        Debug.Log(enemy.gameObject.name);
        GameObject spawnedEnemy = Instantiate(enemy, position.position, Quaternion.identity);

        //spawnedEnemy.transform.parent = position;

        Instantiate(smokeSpawnPrefab, position.position, Quaternion.identity);

        enemyDisplay.AddEnemy(spawnedEnemy);
        combatEnemies.Add(spawnedEnemy);
    }

    public override void ResetRoom()
    {
        base.ResetRoom();

        for (int i = 0; i < combatEnemies.Count; i++)
        {
            if (combatEnemies[i] != null) Destroy(combatEnemies[i]);
        }
        RemoveListNulls(combatEnemies);

        challengeStarted = false;
        BeginChallenge();
    }

    public override void RoomComplete()
    {
        base.RoomComplete();
        OpenDoors(exitDoors);
    }

    public override void BeginChallenge()
    {
        if (challengeStarted) return;
        base.BeginChallenge();
        CloseDoors(entranceDoors);
        StartCoroutine("CombatRoomCutscene");
    }

    IEnumerator CombatRoomCutscene()
    {
        //if (combatCamPosition != null) PlayerCamRotate.instance.StartCutscene(combatCamPosition.position, combatCamPosition.rotation, 1, 1, 1, 1, 1);

        for (int i = 0; i < enemySpawnPoints.Count; i++)
        {
            Debug.Log(enemiesTypesToSpawn[i].gameObject.name);
            SpawnEnemy(enemiesTypesToSpawn[i], enemySpawnPoints[i]);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
