using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player_ScriptSteal : MonoBehaviour
{
    public InputActionReference northButton; // Pressing Y
    public PlayerInput playerInput;

    public EnemyAI_Base selectedEnemy;
    public EnemyManager enemyManager => GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemySelection();
    }

    public void UpdateEnemySelection()
    {
        if (selectedEnemy == null || Vector3.Distance(transform.position, selectedEnemy.transform.position) > 10)
        {
            selectedEnemy = null;
            enemyManager.DeselectlEnemies();
        }
    }

    public void ChangeSelectedEnemy(EnemyAI_Base enemy)
    {
        if (selectedEnemy != enemy)
        {
            enemyManager.SelectEnemy(enemy);
            selectedEnemy = enemy;
        }
    }

    public bool InputIsKeyboard()
    {
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
