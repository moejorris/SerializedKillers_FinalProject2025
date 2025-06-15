using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class EnemyAI_Base : MonoBehaviour
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

    [Header("Health")]
    private RectTransform healthBar;
    public float maxHealth = 20f;
    public float health = 20f;

    [Header("Highlighting")]
    [SerializeField] private GameObject[] highlightableMeshes;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        //navMeshAgent = GetComponent<NavMeshAgent>();
        healthBar = transform.Find("Canvas/Bar").GetComponent<RectTransform>();
        scriptStealMenu = GameObject.FindGameObjectWithTag("Canvas").transform.Find("ScriptStealMenu").GetComponent<ScriptStealMenu>();
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController/Test_Bryson").gameObject.transform;

        UpdateHealth();
        //currentAngle = Random.Range(0f, 360f);
        //transform.Find("Canvas/Image").GetComponent<Image>().sprite = heldBehavior.behavioricon;
        //animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    //void Update()
    //{






    //if (flyingEnemy)
    //{
    //    navMeshAgent.enabled = false;
    //    if (!behaviorActive)
    //    {
    //        GetComponent<Rigidbody>().useGravity = false;
    //        transform.LookAt(playerTarget.transform.position);
    //        GetComponent<Rigidbody>().AddForce(transform.forward / 10);

    //        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 50f, flightDetectionLayer))
    //        {
    //            if (hit.distance > 5)
    //            {
    //                GetComponent<Rigidbody>().AddForce(Vector3.up / 3);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        GetComponent<Rigidbody>().useGravity = true;
    //    }

    //    if (delayedExit && !scriptStealMenu.menuOpen)
    //    {
    //        delayedExit = false;
    //        transform.Find("Capsule").gameObject.layer = 0;
    //        scriptStealMenu.selectedEnemy = null;
    //        scriptStealMenu.centerSlot.RemoveBehavior();
    //    }

    //    if (scriptStealMenu.selectedEnemy != this && transform.Find("Capsule").gameObject.layer == 6)
    //    {
    //        DeselectEnemy();
    //    }
    //}
    //else
    //{
    //    if (circlePlayer)
    //    {
    //        currentAngle += angleSpeed * Time.deltaTime;
    //        currentAngle %= 360; // Keep angle within 0-360 degrees

    //        // Calculate the point on the circle
    //        float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius;
    //        float z = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius;
    //        Vector3 circlePos = new Vector3(x, 0, z);

    //        navMeshAgent.destination = playerTarget.position + circlePos;
    //        navMeshAgent.updateRotation = false;

    //        Vector3 direction = playerTarget.position - transform.position;
    //        Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * circlingRotationSpeed);
    //        transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);

    //        //if (targetDistance > attackDistance)
    //        //{
    //        //    circlePlayer = false;
    //        //    navMeshAgent.isStopped = true;
    //        //    animator.SetBool("Attack", true);
    //        //}

    //        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * circlingRotationSpeed);
    //    }
    //    else
    //    {
    //        navMeshAgent.updateRotation = true;
    //        targetDistance = Vector3.Distance(navMeshAgent.transform.position, playerTarget.position);
    //        if (targetDistance < attackRange)
    //        {
    //            //circlePlayer = true;
    //            navMeshAgent.isStopped = true;
    //            //animator.SetBool("Attack", true);
    //        }
    //        else
    //        {
    //            navMeshAgent.isStopped = false;
    //            //animator.SetBool("Attack", false);
    //            if (targetDistance < followRange) // if the player is within follow range, the entity will head to that positon. otherwise, the last known position.
    //            {
    //                navMeshAgent.destination = playerTarget.position;
    //            }
    //        }
    //    }

    //    if (delayedExit && !scriptStealMenu.menuOpen)
    //    {
    //        delayedExit = false;
    //        transform.Find("Capsule").gameObject.layer = 0;
    //        scriptStealMenu.selectedEnemy = null;
    //        scriptStealMenu.centerSlot.RemoveBehavior();
    //    }

    //    if (scriptStealMenu.selectedEnemy != this && transform.Find("Capsule").gameObject.layer == 6)
    //    {
    //        DeselectEnemy();
    //    }
    //}
    // }

    public void UpdateHealth()
    {
        if (!healthBar) return;
        Vector3 scale = healthBar.localScale;
        scale.x = (health / maxHealth);
        healthBar.localScale = scale;
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
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

    public virtual void SelectEnemy()
    {
        foreach (GameObject highlightPiece in highlightableMeshes) // highlights the pieces in the array
        {
            highlightPiece.layer = 6;
        }

        scriptStealMenu.selectedEnemy = this;
        scriptStealMenu.UpdateCenterSlot();
    }

    public virtual void DeselectEnemy()
    {
        foreach (GameObject highlightPiece in highlightableMeshes) // unhighlights the pieces in the array
        {
            highlightPiece.layer = 0;
        }

        scriptStealMenu.selectedEnemy = null;
        scriptStealMenu.UpdateCenterSlot();
    }
}
