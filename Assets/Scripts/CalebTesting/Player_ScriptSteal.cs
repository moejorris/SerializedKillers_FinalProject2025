using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Player_ScriptSteal : MonoBehaviour
{
    public Color scriptEffectColor = Color.white;
    //public Behavior HeldBehavior => heldBehavior;

    [SerializeField] private float scriptStealRange = 10;
    [SerializeField] private InputActionReference northButton; // Pressing Y
    [SerializeField] private PlayerInput playerInput;

    private bool scriptStealing = false;
    private bool returningScript = false;
    private float scriptReturnTimer = 2f;
    public float scriptReturnTime = 1;

    [SerializeField] private ParticleSystem electricityParticles;
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private ParticleSystem waterParticles;

    [SerializeField] private EnemyAI_Base selectedEnemy;
    [SerializeField] public Behavior heldBehavior;
    [SerializeField] private Sprite emptySlot;
    [SerializeField] private Image stolenScriptSlot => GameObject.FindGameObjectWithTag("Canvas").transform.Find("HUD/HeldScript/Icon").GetComponent<Image>();
    [SerializeField] private Image stolenScriptAnimation => GameObject.FindGameObjectWithTag("Canvas").transform.Find("HUD/HeldScript/Animation").GetComponent<Image>();
    private int stolenScriptAnimationInt = 0;
    [SerializeField] private EnemyManager enemyManager;

    //[SerializeField] private GameObject onScreenControls;
    private GameObject[] onScreenControlButtons;


    [Header("Status Effects")]
    [SerializeField] private FireDamageEffect fireStatusEffect;

    public List<BoxCollider> fireHazardColliders;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager")?.GetComponent<EnemyManager>();

        if (enemyManager == null) enabled = false;

        scriptReturnTimer = scriptReturnTime;
        StartCoroutine("UpdateTimer");
        StartCoroutine("ScriptAnimationTimer");

        // initial get
        GameObject[] hazards = GameObject.FindGameObjectsWithTag("FireHazard");
        foreach (GameObject hazard in hazards)
        {
            BoxCollider[] colliders = hazard.GetComponents<BoxCollider>();
            foreach (BoxCollider collider in colliders) 
            {
                if (collider.isTrigger) continue;
                fireHazardColliders.Add(collider);
            }
        }

        if (GameObject.FindGameObjectWithTag("Canvas").transform.Find("OnScreenControls") != null)
        {
            onScreenControlButtons = GetChildren(GameObject.FindGameObjectWithTag("Canvas").transform.Find("OnScreenControls").gameObject);
            UpdateOnScreenControls();
        }

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

            if (!scriptStealing)
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

        //UpdateEnemySelection();
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

    IEnumerator UpdateTimer()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            UpdateEnemySelection();
            yield return new WaitForSeconds(0.1f);
        }
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
        if (heldBehavior != null && PlayerController.instance.Mana.scriptActive)
        {
            scriptEffectColor = heldBehavior.color;

            if (heldBehavior.behaviorName == "electric")
            {
                electricityParticles.Play();
                fireParticles.Stop();
                waterParticles.Stop();
                FireHazardToggle(true);
            }
            else if (heldBehavior.behaviorName == "fire")
            {
                electricityParticles.Stop();
                fireParticles.Play();
                waterParticles.Stop();
                FireHazardToggle(false);
            }
            else if (heldBehavior.behaviorName == "water")
            {
                electricityParticles.Stop();
                fireParticles.Stop();
                waterParticles.Play();
                FireHazardToggle(true);
            }
            else
            {
                Debug.Log("Something went wrong with held behavior.");
            }
        }
        else
        {
            electricityParticles.Stop();
            fireParticles.Stop();
            waterParticles.Stop();
            FireHazardToggle(true);
            scriptEffectColor = Color.white;
        }
        UpdateUI();
    }

    public void FireHazardToggle(bool toggle)
    {
        foreach (Collider collider in fireHazardColliders)
        {
            collider.enabled = toggle;
        }
    }

    public void UpdateUI()
    {
        stolenScriptSlot.transform.parent.GetComponent<Animation>().Play();
        if (heldBehavior != null)
        {

            if (PlayerController.instance.Mana.scriptActive)
            {
                stolenScriptAnimation.enabled = true;
                stolenScriptSlot.sprite = heldBehavior.activatedBehaviorIcon;

                stolenScriptAnimationInt = 0;
                stolenScriptAnimation.sprite = heldBehavior.animation[0];
            }
            else
            {
                stolenScriptAnimation.enabled = false;
                stolenScriptSlot.sprite = heldBehavior.deactivatedBehaviorIcon;
            }
        }
        else
        {
            stolenScriptAnimation.enabled = false;
            stolenScriptSlot.sprite = emptySlot;
        }



        //if (heldBehavior != null)
        //{
        //    if (stolenScriptSlot.sprite != heldBehavior.deactivatedBehaviorIcon)
        //    {
        //        stolenScriptSlot.transform.parent.GetComponent<Animation>().Play();
        //        stolenScriptSlot.sprite = heldBehavior.deactivatedBehaviorIcon; // doesn't do bloop every time
        //    }
        //}
        //else
        //{
        //    stolenScriptSlot.sprite = emptySlot;
        //}
        //stolenScriptSlot.transform.parent.GetComponent<Image>().color = scriptEffectColor;
    }

    IEnumerator ScriptAnimationTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (heldBehavior != null)
            {
                stolenScriptAnimationInt++;
                if (stolenScriptAnimationInt > 3) stolenScriptAnimationInt = 0;
                stolenScriptAnimation.sprite = heldBehavior.animation[stolenScriptAnimationInt];
            }
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

    public Behavior GetHeldBehavior()
    {
        if (heldBehavior == null) return null;
        else return heldBehavior;
    }

    public bool BehaviorActive()
    {
        if (heldBehavior != null && PlayerController.instance.Mana.scriptActive) return true;
        else return false;
    }

    public void ApplyStatusEffect(Behavior behavior)
    {
        if (behavior == null) return;

        if (behavior.behaviorName == "fire")
        {
            fireStatusEffect.StartFire();
        }
        else if (behavior.behaviorName == "water")
        {
            fireStatusEffect.StopFire();
        }
    }

    public void UpdateOnScreenControls()
    {
        //GameObject activeScript
        if (heldBehavior != null)
        {
            GameObject.FindGameObjectWithTag("Canvas").transform.Find("OnScreenControls/ActivateScripts").gameObject.SetActive(true);
        }
        else
        {
            GameObject.FindGameObjectWithTag("Canvas").transform.Find("OnScreenControls/ActivateScripts").gameObject.SetActive(false);
        }

            foreach (GameObject button in onScreenControlButtons)
            {
                button.transform.Find("Keyboard").gameObject.SetActive(InputIsKeyboard());
                button.transform.Find("Controller").gameObject.SetActive(!InputIsKeyboard());
            }
    }

    public GameObject[] GetChildren(GameObject parent)
    {
        int childCount = parent.transform.childCount;
        GameObject[] children = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            children[i] = parent.transform.GetChild(i).gameObject;
        }

        return children;
    }
}
