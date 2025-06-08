using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] thing;
    public List<EnemyAI_Base> enemies;

    //public List<GameObject> thing2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateEnemyList()
    {
        enemies.Clear();
        GameObject[] enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyArray)
        {
            enemies.Add(enemy.GetComponent<EnemyAI_Base>());
        }
    }

    public void ActivateBehavior(Behavior behavior)
    {
        UpdateEnemyList();
        foreach (EnemyAI_Base enemyScript in enemies)
        {
            if (enemyScript.heldBehavior == behavior)
            {
                enemyScript.ActivateBehavior();
                //enemyScript.transform.Find("Canvas/Image").GetComponent<Image>().color = Color.white;
            }
        }
    }

    public void DeactivateBehavior(Behavior behavior)
    {
        UpdateEnemyList();
        foreach (EnemyAI_Base enemyScript in enemies)
        {
            if (enemyScript.heldBehavior == behavior)
            {
                enemyScript.DeactivateBehavior();
                //enemyScript.transform.Find("Canvas/Image").GetComponent<Image>().color = Color.red;
            }
        }
    }
}
