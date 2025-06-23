using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player_ScriptSteal : MonoBehaviour
{
    public Color scriptEffectColor = Color.white;

    [SerializeField] private float scriptStealRange = 10;
    [SerializeField] private InputActionReference northButton; // Pressing Y
    [SerializeField] private PlayerInput playerInput;

    private bool scriptStealing = false;
    private bool returningScript = false;
    private float scriptReturnTimer = 2f;
    public float scriptReturnTime = 1;

    [SerializeField] private GameObject electricityParticles;
    [SerializeField] private GameObject fireParticles;
    [SerializeField] private GameObject waterParticles;

    [SerializeField] private EnemyAI_Base selectedEnemy;
    [SerializeField] private Behavior heldBehavior;
    [SerializeField] private Sprite emptySlot;
    [SerializeField] private Image stolenScriptSlot => GameObject.FindGameObjectWithTag("Canvas").transform.Find("ScriptSlot/HeldScript").GetComponent<Image>();
    [SerializeField] private EnemyManager enemyManager => GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

    [SerializeField] private Player_CombatMachine combatMachine => GetComponent<Player_CombatMachine>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scriptReturnTimer = scriptReturnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (northButton.action.WasPressedThisFrame())
        {
            if (selectedEnemy != null)
            {
                if (selectedEnemy.behaviorActive)
                {
                    scriptStealing = true;
                    StealScript();
                }
            }
            else if (!scriptStealing)
            {
                returningScript = true;
            }
        }
        else if (northButton.action.WasReleasedThisFrame())
        {
            scriptStealing = false;
            returningScript = false;
        }

        if (returningScript)
        {
            scriptReturnTimer -= Time.deltaTime;
            if (scriptReturnTimer <= 0)
            {
                ReturnScript();
                returningScript = false;
            }
        }
        else
        {
            scriptReturnTimer = scriptReturnTime;
        }

        UpdateEnemySelection();
    }

    public void StealScript()
    {
        heldBehavior = selectedEnemy.heldBehavior;
        enemyManager.UpdateEnemyBehaviors(heldBehavior);
        ApplyScriptEffects();
        selectedEnemy = null;
    }

    public void ReturnScript()
    {
        heldBehavior = null;
        enemyManager.UpdateEnemyBehaviors();
        ApplyScriptEffects();
    }

    public void UpdateEnemySelection()
    {
        if (selectedEnemy == null || Vector3.Distance(transform.position, selectedEnemy.transform.position) > scriptStealRange)
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

    public void ApplyScriptEffects()
    {
        if (heldBehavior != null)
        {
            scriptEffectColor = selectedEnemy.heldBehavior.color;

            if (heldBehavior.behaviorName == "electric")
            {
                electricityParticles.SetActive(true);
                fireParticles.SetActive(false);
                waterParticles.SetActive(false);
            }
            else if (heldBehavior.behaviorName == "fire")
            {
                electricityParticles.SetActive(false);
                fireParticles.SetActive(true);
                waterParticles.SetActive(false);
            }
            else if (heldBehavior.behaviorName == "water")
            {
                electricityParticles.SetActive(false);
                fireParticles.SetActive(false);
                waterParticles.SetActive(true);
            }
            else
            {
                Debug.Log("Something went wrong with held behavior.");
            }
        }
        else
        {
            electricityParticles.SetActive(false);
            fireParticles.SetActive(false);
            waterParticles.SetActive(false);
            scriptEffectColor = Color.white;
        }
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (heldBehavior != null)
        {
            stolenScriptSlot.sprite = heldBehavior.behavioricon;
        }
        else
        {
            stolenScriptSlot.sprite = emptySlot;
        }
        stolenScriptSlot.transform.parent.GetComponent<Image>().color = scriptEffectColor;
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
