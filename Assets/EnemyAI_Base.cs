using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI_Base : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float attackDistance;
    private NavMeshAgent navMeshAgent;
    //private Animator animator;
    [SerializeField] private float targetDistance;
    [SerializeField] private float circlingRotationSpeed = 1;

    public float radius = 10f;
    public float angleSpeed = 1f; // Degrees per second
    public Vector3 center = Vector3.zero;

    private float currentAngle = 0f;

    [Header("Held Behavior")]
    public Behavior heldBehavior;
    public bool behaviorActive = true;
    private bool delayedExit = false;
    [SerializeField] private ScriptStealMenu scriptStealMenu;

    public bool circlePlayer = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (playerTarget == null)
        {
            playerTarget = GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController").gameObject.transform;
        }
        //animator = GetComponent<Animator>();
    }



    // Update is called once per frame
    void Update()
    {
        if (circlePlayer)
        {
            currentAngle += angleSpeed * Time.deltaTime;
            currentAngle %= 360; // Keep angle within 0-360 degrees

            // Calculate the point on the circle
            float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radius;
            float z = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radius;
            Vector3 circlePos = new Vector3(x, 0, z);

            navMeshAgent.destination = playerTarget.position + circlePos;
            navMeshAgent.updateRotation = false;

            Vector3 direction = playerTarget.position - transform.position;
            Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * circlingRotationSpeed);
            transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);

            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * circlingRotationSpeed);
        }
        else
        {
            navMeshAgent.updateRotation = true;
            targetDistance = Vector3.Distance(navMeshAgent.transform.position, playerTarget.position);
            if (targetDistance < attackDistance)
            {
                navMeshAgent.isStopped = true;
                //animator.SetBool("Attack", true);
            }
            else
            {
                navMeshAgent.isStopped = false;
                //animator.SetBool("Attack", false);
                navMeshAgent.destination = playerTarget.position;
            }
        }

        if (delayedExit && !scriptStealMenu.menuOpen)
        {
            delayedExit = false;
            transform.Find("Capsule").gameObject.layer = 0;
            scriptStealMenu.selectedEnemy = null;
            scriptStealMenu.centerSlot.RemoveBehavior();
        }

        if (scriptStealMenu.selectedEnemy != this && transform.Find("Capsule").gameObject.layer == 6)
        {
            DeselectEnemy();
        }
    }


    public void SelectEnemy()
    {
        transform.Find("Capsule").gameObject.layer = 6;
        scriptStealMenu.selectedEnemy = this;
        scriptStealMenu.UpdateCenterSlot();
    }

    public void DeselectEnemy()
    {
        scriptStealMenu.selectedEnemy = null;
        scriptStealMenu.UpdateCenterSlot();
        transform.Find("Capsule").gameObject.layer = 0;
    }


    //private void OnMouseEnter()
    //{
    //    if (scriptStealMenu.selectedEnemy == null && behaviorActive)
    //    {
    //        transform.Find("Capsule").gameObject.layer = 6;
    //        scriptStealMenu.selectedEnemy = this;
    //        scriptStealMenu.centerSlot.AddBehavior(heldBehavior);
    //    }
    //    delayedExit = false;
    //}

    //private void OnMouseExit()
    //{
    //    if (scriptStealMenu.selectedEnemy == this && scriptStealMenu.menuOpen)
    //    {
    //        delayedExit = true;
    //    }
    //    else
    //    {
    //        if (scriptStealMenu.selectedEnemy == this)
    //        {
    //            scriptStealMenu.selectedEnemy = null;
    //            scriptStealMenu.centerSlot.RemoveBehavior();
    //        }
    //        transform.Find("Capsule").gameObject.layer = 0;
    //    }
    //}
}
