using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Room : MonoBehaviour
{
    [Header("Room Objective")]
    public List<Animator> exitDoors;
    public List<Animator> entranceDoors;
    [SerializeField] Transform roomSuccessCamera;
    [SerializeField] private bool challengeStarted = false;
    //[SerializeField] private Transform postCheckpoint;
    //[SerializeField] private Transform preCheckpoint;

    [Header("Room Respawning (If Applicable)")]
    [SerializeField] private GameObject smokeSpawnPrefab;
    [SerializeField] private List<GameObject> requiredEnemyTypes;
    [SerializeField] private List<Transform> requiredEnemyRespawnPoints;
    private BoxCollider box => GetComponent<BoxCollider>();
    [SerializeField] private LayerMask enemyLayer;
    private List<GameObject> respawnEnemies = new List<GameObject>();

    private bool playerInRoom = false;
    private float timer;

    bool roomCompleted = false;

    [SerializeField] private GameObject roomTextPrefab;
    [SerializeField] private string roomTitle = "Room Title Here";
    [SerializeField] private RoomType roomType;
    public enum RoomType { CombatRoom, PuzzleRoom };

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(box.bounds.center, box.size);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        //InitialEnemyPass();
        timer = 2;
    }

    public virtual void Update()
    {
        if (playerInRoom)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 2;
                CheckEnemies();
            }
        }
    }

    public virtual void CheckEnemies()
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

    //public void InitialEnemyPass() // creates list from overlapsphere
    //{
    //    Collider[] enems = Physics.OverlapBox(box.bounds.center, box.size / 2, Quaternion.identity, enemyLayer);
    //    foreach (Collider enemy in enems)
    //    {
    //        respawnEnemies.Add(enemy.gameObject);
    //    }
    //}

    public void SpawnEnemy(GameObject enemy, Transform position)
    {
        GameObject spawnedEnemy = Instantiate(enemy, position.position, Quaternion.identity);

        spawnedEnemy.transform.parent = position;

        Instantiate(smokeSpawnPrefab, position.position, Quaternion.identity);
        RemoveListNulls(respawnEnemies);
        respawnEnemies.Add(spawnedEnemy);
    }

    public void RemoveListNulls(List<GameObject> checkedEnemies) // goes through and clears any null enemies that were killed
    {
        for (int i = 0; i < checkedEnemies.Count; i++)
        {
            if (checkedEnemies[i] == null)
            {
                checkedEnemies.RemoveAt(i);
            }
        }
    }

    //public bool EnemyTypeExists(Behavior element) // checks if this enemy type exists
    //{
    //    foreach (GameObject enemy in respawnEnemies)
    //    {
    //        if (enemy.transform.Find("Skull") != null)
    //        {
    //            if (enemy.transform.Find("Skull").GetComponent<EnemyAI_Base>().heldBehavior == element)
    //            {
    //                return true;
    //            }
    //        }
    //        else if (enemy.GetComponent<EnemyAI_Base>() && enemy.GetComponent<EnemyAI_Base>().heldBehavior == element)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other == PlayerController.instance.Collider)
        {
            playerInRoom = true;
            BeginChallenge();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == PlayerController.instance.Collider)
        {
            playerInRoom = false;
        }
    }

    public virtual void RoomComplete()
    {
        if (roomCompleted) return; //stops from running more than once. If this runs more than once, the cutscene plays multiple times :(
        roomCompleted = true;

        //if (postCheckpoint == null) return;
        //PlayerController.instance.Respawn.respawnPoint = postCheckpoint.position;
        if (roomSuccessCamera != null)
        {
            PlayerCamRotate.instance.StartCutscene(roomSuccessCamera.position, roomSuccessCamera.rotation);
        }
    }

    public virtual void BeginChallenge()
    {
        if (challengeStarted) return;
        challengeStarted = true;

        if (roomTextPrefab != null)
        {
            GameObject roomText = Instantiate(roomTextPrefab, GameObject.FindGameObjectWithTag("Canvas").transform).gameObject;
            roomText.transform.Find("Title").GetComponent<TMP_Text>().text = roomTitle;
            if (roomType == RoomType.PuzzleRoom) roomText.transform.Find("RoomType").GetComponent<TMP_Text>().text = "[Puzzle Room]";
            else roomText.transform.Find("RoomType").GetComponent<TMP_Text>().text = "[Combat Room]";
        }
        //PlayerController.instance.Respawn.respawnPoint = postCheckpoint.position;
    }

    public void OpenDoors(List<Animator> doors)
    {
        foreach (Animator door in doors)
        {
            door.SetBool("Open", true);
        }
    }

    public void CloseDoors(List<Animator> doors)
    {
        foreach (Animator door in doors)
        {
            door.SetBool("Open", false);
        }
    }
}

