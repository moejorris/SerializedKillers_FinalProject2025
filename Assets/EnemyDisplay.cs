using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class EnemyDisplay : MonoBehaviour
{
    [SerializeField] private List<GameObject> SpiteBulbsAlive = new List<GameObject>();
    [SerializeField] private List<GameObject> OverclocksAlive = new List<GameObject>();
    [SerializeField] private List<GameObject> SobbySkullsAlive = new List<GameObject>();

    private GameObject spiteBulbsPanel => transform.Find("ElectricEnemy").gameObject;
    private Image spiteBulbIcon => transform.Find("ElectricEnemy/Icon").GetComponent<Image>();
    private TMP_Text spiteBulbCount => transform.Find("ElectricEnemy/Count").GetComponent<TMP_Text>();

    private GameObject overclocksPanel => transform.Find("FireEnemy").gameObject;
    private Image overclockIcon => transform.Find("FireEnemy/Icon").GetComponent<Image>();
    private TMP_Text overclockCount => transform.Find("FireEnemy/Count").GetComponent<TMP_Text>();

    private GameObject sobbySkullsPanel => transform.Find("WaterEnemy").gameObject;
    private Image sobbySkullIcon => transform.Find("WaterEnemy/Icon").GetComponent<Image>();
    private TMP_Text sobbySkullCount => transform.Find("WaterEnemy/Count").GetComponent<TMP_Text>();
    private bool enemiesBeingAdded = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCounts();
    }

    public void AddEnemy(GameObject enemy)
    {
        EnemyAI_Base enemyScript;

        if (enemy.GetComponent<EnemyAI_Base>() != null)
        {
            enemyScript = enemy.GetComponent<EnemyAI_Base>();
        }
        else if (enemy.transform.Find("Skull") != null && enemy.transform.Find("Skull").GetComponent<EnemyAI_Base>() != null)
        {
            enemyScript = enemy.transform.Find("Skull").GetComponent<EnemyAI_Base>();
        }
        else return;

        enemiesBeingAdded = true;

        switch(enemyScript.heldBehavior.behaviorName)
        {
            case "water": SobbySkullsAlive.Add(enemy); break;
            case "electric": SpiteBulbsAlive.Add(enemy); break;
            case "fire": OverclocksAlive.Add(enemy); break;
            default: break;
        }

        enemiesBeingAdded = false;
    }

    public void OpenMenu()
    {

    }

    public void CloseMenu()
    {
        ClearEnemies();
    }

    public void ClearEnemies()
    {
        SpiteBulbsAlive.Clear();
        OverclocksAlive.Clear();
        SobbySkullsAlive.Clear();
    }

    public void UpdateCounts()
    {
        if (enemiesBeingAdded) return;

        for (int i = 0; i < SpiteBulbsAlive.Count; i++)
        {
            if (SpiteBulbsAlive[i] == null) SpiteBulbsAlive.RemoveAt(i);
        }

        spiteBulbCount.text = SpiteBulbsAlive.Count.ToString();
        spiteBulbsPanel.SetActive(SpiteBulbsAlive.Count > 0);

        for (int i = 0; i < OverclocksAlive.Count; i++)
        {
            if (OverclocksAlive[i] == null) OverclocksAlive.RemoveAt(i);
        }

        overclockCount.text = OverclocksAlive.Count.ToString();
        overclocksPanel.SetActive(OverclocksAlive.Count > 0);

        for (int i = 0; i < SobbySkullsAlive.Count; i++)
        {
            if (SobbySkullsAlive[i] == null) SobbySkullsAlive.RemoveAt(i);
        }

        sobbySkullCount.text = SobbySkullsAlive.Count.ToString();
        sobbySkullsPanel.SetActive(SobbySkullsAlive.Count > 0);
    }
}
