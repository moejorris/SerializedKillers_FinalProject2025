using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI_Base : MonoBehaviour, ITargetable, IDamageable
{
    [Header("Navigation")]
    public Transform playerTarget;
    public float attackRange;
    //public float followRange;
    public NavMeshAgent navMeshAgent;
    //private Animator animator;
    //[SerializeField] private float targetDistance;
    //[SerializeField] private float circlingRotationSpeed = 1;

    //public float radius = 10f;
    //public float angleSpeed = 1f; // Degrees per second
    //public Vector3 center = Vector3.zero;

    //private float currentAngle = 0f;
    //public bool circlePlayer = false;

    //[SerializeField] private bool flyingEnemy = false;
    //[SerializeField] private LayerMask flightDetectionLayer;

    [Header("Held Behavior")]
    public Behavior heldBehavior;
    public bool behaviorActive = true;
    private bool delayedExit = false;
    [SerializeField] private ScriptStealMenu scriptStealMenu;
    public Image selectedIcon;

    [Header("Health")]
    public RectTransform healthBar;
    public RectTransform whiteHealthBar;
    public float maxHealth = 20f;
    public float health = 20f;
    private float healthSpeedMult = 1;

    public Renderer[] meshes;
    public List<Material> materialList;

    [Header("Highlighting")]
    [SerializeField] private GameObject[] highlightableMeshes;

    [Header("Targeting")]
    public float TargetScore { get; set;}

    public virtual void Start()
    {
        foreach (Renderer mesh in meshes)
        {
            foreach (Material material in mesh.materials)
            {
                materialList.Add(material);
                Debug.Log("added " + material);
            }
        }

        navMeshAgent = GetComponent<NavMeshAgent>();

        if (playerTarget == null)
        {
            playerTarget = GameObject.FindGameObjectWithTag("PlayerMesh").transform;
        }
        scriptStealMenu = GameObject.FindGameObjectWithTag("Canvas").transform.Find("ScriptStealMenu").GetComponent<ScriptStealMenu>();

        UpdateHealth();
    }

    public virtual void Update()
    {
        if (whiteHealthBar.localScale.x > healthBar.localScale.x)
        {
            float lerpScale = Mathf.Lerp(whiteHealthBar.localScale.x, healthBar.localScale.x, healthSpeedMult);
            Vector3 whiteBarScale = whiteHealthBar.localScale;
            whiteBarScale.x = lerpScale;
            whiteHealthBar.localScale = whiteBarScale;
            healthSpeedMult += (Time.deltaTime / 25f);
        }
    }

    IEnumerator MaterialFade()
    {
        float percent = 0;
        for (int i = 0; i < 5; i++)
        {
            foreach (Material material in materialList)
            {
                material.SetFloat("_Percentage", percent);
            }
            if (i == 0) yield return new WaitForSeconds(0.04f);
            percent += 0.25f;
            yield return new WaitForSeconds(0.03f);
        }
    }

    public void UpdateHealth()
    {
        if (!healthBar || !whiteHealthBar) return;

        healthSpeedMult = 0;
        Vector3 scale = healthBar.localScale;
        scale.x = (health / maxHealth);
        healthBar.localScale = scale;
    }

    public virtual void TakeDamage(float damage)
    {
        if (!healthBar || !whiteHealthBar) return; // in case no thing exists

        health -= damage;

        StopCoroutine("MaterialFade");
        StartCoroutine("MaterialFade");

        UpdateHealth();
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public virtual void GainHealth(float amount)
    {
        health += amount;
        if (health >= maxHealth)
        {
            health = maxHealth;
        }
        UpdateHealth();
    }

    public virtual void ActivateBehavior()
    {
        behaviorActive = true;
    }

    public virtual void DeactivateBehavior()
    {
        behaviorActive = false;
    }

    public virtual void HighlightEnemy()
    {
        foreach (GameObject highlightPiece in highlightableMeshes) // highlights the pieces in the array
        {
            highlightPiece.layer = 6;
        }
        selectedIcon.enabled = true;
    }

    public virtual void UnHighlightEnemy()
    {
        foreach (GameObject highlightPiece in highlightableMeshes) // unhighlights the pieces in the array
        {
            highlightPiece.layer = 0;
        }
        selectedIcon.enabled = false;
    }
}
