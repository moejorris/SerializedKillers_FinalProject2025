using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class EnemyAI_SobbySkull : EnemyAI_Base
{
    [Header("Sobby Skull General")]
    [SerializeField] private string movementState = "rolling";
    
    [SerializeField] private GameObject waterOrb;
    [SerializeField] private Rigidbody rigidBody;

    [Header("Sobby Skull Vision")]
    [SerializeField] private float idleAlertRange;
    [SerializeField] private float blindFollowRange;
    [SerializeField] private float visionConeLength;
    [SerializeField] private float visionConeWidth;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Sobby Skull Rolling")]
    [SerializeField] private float rollFollowRange = 6;

    [Header("Sobby Skull Pathfinding")]
    [SerializeField] private Vector3 actualPos;
    public float rollCycle = 0;
    public float rollCycleDivision = 2;
    public Vector3 lagPos;
    public float yOffset;
    public float rollSpeed = 1;



    public override void Start()
    {
        base.Start();
        navMeshAgent = transform.parent.Find("NavmeshAgent").GetComponent<NavMeshAgent>();
        lagPos = transform.position;
        yOffset = Vector3.Distance(transform.position, navMeshAgent.transform.position);
        rigidBody = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (behaviorActive) // has the water script
        {
            if (movementState == "rolling_idle")
            {
                if (PlayerInRollRange())
                {
                    //StartRolling();
                    navMeshAgent.speed = 1;
                    movementState = "rolling_pursuit";
                    waterOrb.SetActive(true);
                    rigidBody.useGravity = false;
                }
                else
                {
                    // just chillin
                }
            }
            else if (movementState == "rolling_pursuit")
            {
                if (!PlayerInRollRange())
                {
                    //nothing
                }
                else
                {
                    navMeshAgent.destination = playerTarget.position;
                    RollForward();
                }
            }
        }
        else // script stolen
        {
            if (movementState == "flying_idle")
            {
                if (PlayerVisible())
                {
                    movementState = "flying_pursuit";
                }
                else
                {
                    // fly in place?
                }
            }
            else if (movementState == "flying_pursuit")
            {
                if (PlayerVisible())
                {
                    rollCycle += Mathf.PI / rollCycleDivision * Time.deltaTime;
                    rollCycle = rollCycle % (Mathf.PI * 2);

                    lagPos = Vector3.Lerp(lagPos, new Vector3(navMeshAgent.transform.position.x, lagPos.y, navMeshAgent.transform.position.z), Time.deltaTime);
                    lagPos = Vector3.Lerp(lagPos, new Vector3(lagPos.x, navMeshAgent.transform.position.y, lagPos.z), Time.deltaTime / 3);

                    transform.position = lagPos + new Vector3(0, yOffset + Mathf.Sin(rollCycle * 2) * 0.5f, 0);
                    RotateTowardsPlayer();
                }
                else
                {
                    movementState = "flying_idle";
                }
            }
        }
    }

    public void RollForward()
    {
        rigidBody.AddForce((navMeshAgent.transform.position - transform.position) * rollSpeed, ForceMode.Impulse);
    }

    public void StartFlying()
    {
        navMeshAgent.speed = 3.5f;
        movementState = "flying_idle";
        waterOrb.SetActive(false);
        rigidBody.useGravity = false;
    }

    public void StopRolling()
    {
        movementState = "rolling_idle";
    }

    public void StartRolling()
    {
        navMeshAgent.speed = 1;
        movementState = "rolling_pursuit";
        waterOrb.SetActive(true);
        rigidBody.useGravity = false;
    }

    public void RotateTowardsPlayer()
    {
        Vector3 pos = playerTarget.position - transform.position;
        Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(pos, Vector3.up), Time.deltaTime * 10);
        //Debug.Log("rotating towards: " + rotation);
        transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
    }


    public bool PlayerVisible()
    {
        if (Vector3.Distance(playerTarget.position, transform.position) <= blindFollowRange) // within the "listen range"
        {
            return true;
        }
        else if (Vector3.Distance(playerTarget.position, transform.position) <= visionConeLength) // at least within sight range
        {
            if (Physics.Raycast(transform.position, playerTarget.transform.position - transform.position, out RaycastHit rayHit, (Vector3.Distance(playerTarget.position, transform.position) - 3), obstacleLayer)) // checks if player behind things?
            {
                return false;
            }
            else
            {
                Vector3 directionToTarget = (playerTarget.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < visionConeWidth / 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else // in neither
        {
            return false;
        }
    }

    public bool PlayerInRollRange()
    {
        if (Vector3.Distance(playerTarget.position, transform.position) < rollFollowRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool PlayerInAttackRange()
    {
        if (Vector3.Distance(playerTarget.position, transform.position) < attackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void ActivateBehavior() // roll mode
    {
        base.ActivateBehavior();

        movementState = "idle";
    }

    public override void DeactivateBehavior() // fly mode
    {
        base.DeactivateBehavior();

        StartFlying();
    }
}
