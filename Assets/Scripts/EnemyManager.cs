using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] thing;
    public List<EnemyAI_Base> enemies;

    public void UpdateEnemyList()
    {
        enemies.Clear();
        GameObject[] enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyArray)
        {
            enemies.Add(enemy.GetComponent<EnemyAI_Base>());
        }

        Debug.Log("Update Enemy List called! It has " + enemies.Count);
    }

    public void SelectEnemy(EnemyAI_Base enemy)
    {
        UpdateEnemyList();
        foreach (EnemyAI_Base enemyScript in enemies)
        {
            enemyScript.UnHighlightEnemy();
        }
        enemy.HighlightEnemy();
    }

    public void DeselectlEnemies()
    {
        UpdateEnemyList();
        foreach (EnemyAI_Base enemyScript in enemies)
        {
            enemyScript.UnHighlightEnemy();
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
            }
        }
    }

    public void UpdateEnemyBehaviors(Behavior behavior = null)
    {
        UpdateEnemyList();
        foreach (EnemyAI_Base enemyScript in enemies)
        {
            if (enemyScript.heldBehavior == behavior)
            {
                enemyScript.DeactivateBehavior();
            }
            else if (!enemyScript.behaviorActive)
            {
                enemyScript.ActivateBehavior();
            }
        }
    }
}
