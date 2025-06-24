using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Splines;
using System.Collections;

public class EnemyAI_SobbySkull : EnemyAI_Base
{
    [Header("Sobby Skull General")]
    [SerializeField] private string movementState = "idle";
    [SerializeField] private GameObject waterOrb;
    [SerializeField] private NavMeshAgent rollingNavMeshAgent;
    [SerializeField] private bool followPlayer = false;   

    [SerializeField] private bool ducking = false;
    [SerializeField] private Vector3 duckPosition = Vector3.zero;
    [SerializeField] private float doorframeDist = 5;
    [SerializeField] private float flyingSpeed = 5;
    [SerializeField] private float flyingTurnSpeed = 5;
    [SerializeField] private float flyHeight = 4;
    [SerializeField] private List<Vector3> duckPositions = new List<Vector3>();
    [SerializeField] Transform skullFollowTarget;

    [Header("Sobby Skull Vision")]
    [SerializeField] private float idleAlertRange;
    [SerializeField] private float blindFollowRange;
    [SerializeField] private float visionConeLength;
    [SerializeField] private float visionConeWidth;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    //Vector3 lagPos;
    //float yOffset;
    Rigidbody rigidBody;
    //public float rollSpeed = 0.2f;
    //public float rollCycle = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        navMeshAgent = transform.parent.Find("FlyingAgent").GetComponent<NavMeshAgent>();
        rollingNavMeshAgent = transform.parent.Find("RollingAgent").GetComponent<NavMeshAgent>();
        rigidBody = transform.GetComponent<Rigidbody>();
        healthBar = transform.parent.Find("Canvas/Bar").GetComponent<RectTransform>();
        selectedIcon = transform.parent.Find("Canvas/SelectedIcon").GetComponent<Image>();

        //navMeshAgent.isStopped = true;
        movementState = "flying";
        StartCoroutine("DoorCheckTimer");
    }
    public override void Update()
    {
        if (5000 < 1)
        {
            //navMeshAgent.destination = playerTarget.position;
            //if (behaviorActive) // has the water script
            //{
            //    lagPos = transform.position;

            //    if (movementState == "rolling_idle")
            //    {

            //        if (PlayerInRollRange())
            //        {
            //            //StartRolling();
            //            navMeshAgent.speed = 1;
            //            movementState = "rolling_pursuit";
            //            waterOrb.SetActive(true);
            //            rigidBody.useGravity = true;
            //        }
            //        else
            //        {
            //            if (rigidBody.useGravity == false)
            //            {
            //                navMeshAgent.speed = 1;
            //                waterOrb.SetActive(true);
            //                rigidBody.useGravity = true;
            //            }
            //            // just chillin
            //        }
            //    }
            //    else if (movementState == "rolling_pursuit")
            //    {
            //        if (!PlayerInRollRange())
            //        {
            //            //nothing
            //        }
            //        else
            //        {
            //            navMeshAgent.destination = playerTarget.position;
            //            RollForward();
            //        }
            //    }
            //}
            //else // script stolen
            //{
            //    if (movementState == "flying_idle")
            //    {
            //        if (PlayerVisible())
            //        {
            //            movementState = "flying_pursuit";
            //        }
            //        else
            //        {
            //            //rollCycle += Mathf.PI / rollCycleDivision * Time.deltaTime;
            //            //rollCycle = rollCycle % (Mathf.PI * 2);

            //            lagPos = Vector3.Lerp(lagPos, new Vector3(navMeshAgent.transform.position.x, lagPos.y, navMeshAgent.transform.position.z), Time.deltaTime);
            //            lagPos = Vector3.Lerp(lagPos, new Vector3(lagPos.x, navMeshAgent.transform.position.y, lagPos.z), Time.deltaTime / 3);

            //            transform.position = lagPos + new Vector3(0, yOffset + Mathf.Sin(rollCycle * 2) * 0.5f, 0);

            //            // fly in place?
            //        }
            //    }
            //    else if (movementState == "flying_pursuit")
            //    {
            //        if (PlayerVisible())
            //        {
            //            //rollCycle += Mathf.PI / rollCycleDivision * Time.deltaTime;
            //            //rollCycle = rollCycle % (Mathf.PI * 2);

            //            lagPos = Vector3.Lerp(lagPos, new Vector3(navMeshAgent.transform.position.x, lagPos.y, navMeshAgent.transform.position.z), Time.deltaTime);
            //            lagPos = Vector3.Lerp(lagPos, new Vector3(lagPos.x, navMeshAgent.transform.position.y, lagPos.z), Time.deltaTime / 3);

            //            transform.position = lagPos + new Vector3(0, yOffset + Mathf.Sin(rollCycle * 2) * 0.5f, 0);

            //            navMeshAgent.destination = playerTarget.position;
            //            RotateTowardsPlayer();
            //        }
            //        else
            //        {
            //            movementState = "flying_idle";
            //        }
            //    }
            //}
        }

       // navMeshAgent.destination = playerTarget.position;

        if (followPlayer)
        {
            skullFollowTarget = playerTarget;
        }
        else
        {
            skullFollowTarget = navMeshAgent.transform;
        }

        if (movementState == "flying")
        {
            Vector3 TARGET = transform.position;

            //CheckForOverhang();

            if (!ducking)
            {
                //Vector3 navPos = navMeshAgent.transform.position;
                //navPos.y = navMeshAgent.transform.position.y + flyHeight;
                //Vector3 pos = Vector3.Slerp(transform.position, navPos, Time.deltaTime * flyingSpeed);
                //transform.position = pos;

                TARGET = skullFollowTarget.transform.position;
                TARGET.y = Mathf.Lerp(transform.position.y, skullFollowTarget.position.y + flyHeight, flyingTurnSpeed);

                if (duckPositions.Count > 0)
                {
                    ducking = true;
                }
            }
            else
            {
                if (duckPositions.Count > 0)
                {

                    TARGET = duckPositions[0];

                    //Vector3 pos = Vector3.Lerp(transform.position, duckPositions[0], Time.deltaTime * flyingSpeed);
                    //transform.position = pos;

                    if (Vector3.Distance(transform.position, duckPositions[0]) <= 3)
                    {
                        duckPositions.RemoveAt(0);
                    }
                }
                else
                {
                    ducking = false;
                }
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TARGET - transform.position, Vector3.up), Time.deltaTime * flyingTurnSpeed);
            rigidBody.linearVelocity = transform.forward * flyingSpeed;
        }
    }

    public void ConverToSplines()
    {
        
    }

    public void RotateTowardsPlayer()
    {
        Vector3 pos = playerTarget.position - transform.position;
        Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(pos, Vector3.up), Time.deltaTime * 10);
        //Debug.Log("rotating towards: " + rotation);
        transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
    }

    IEnumerator DoorCheckTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            CheckForOverhang();
        }
    }

    public void CheckForOverhang()
    {
        Ray raycast = new Ray();
        raycast.origin = skullFollowTarget.transform.position;
        raycast.direction = Vector3.up;
        RaycastHit hit;

        if (Physics.Raycast(raycast, out hit, 25, obstacleLayer))
        {
            if (Vector3.Distance(hit.point, skullFollowTarget.transform.position) > doorframeDist)
            {
                if (duckPositions.Count <= 0)
                {
                    duckPositions.Add(skullFollowTarget.transform.position);
                }
                else
                {
                    if (Vector3.Distance(duckPositions[duckPositions.Count-1], skullFollowTarget.transform.position) > 3)
                    {
                        duckPositions.Add(skullFollowTarget.transform.position);
                    }
                }
            }
        }

        if (duckPositions.Count > 0)
        {
            Ray raycast2 = new Ray();
            raycast2.origin = transform.position;
            raycast2.direction = skullFollowTarget.position - transform.position;
            RaycastHit hit2;


            if (Physics.Raycast(raycast2, out hit2, 25, ~0, QueryTriggerInteraction.Collide))
            {
                //Debug.Log(hit2.transform.gameObject.name + " and also hit!");

                if (hit2.transform.gameObject.name == "PlayerController")
                {
                    if (Vector3.Angle(transform.forward, skullFollowTarget.position - transform.position) > 90)
                    {
                        duckPositions.Clear();
                    }
                }
            }
        }
    }


    public bool PlayerVisible()
    {
        if (Vector3.Distance(playerTarget.position, navMeshAgent.transform.position) <= blindFollowRange) // within the "listen range"
        {
            return true;
        }
        else if (Vector3.Distance(playerTarget.position, navMeshAgent.transform.position) <= visionConeLength) // at least within sight range
        {
            if (Physics.Raycast(navMeshAgent.transform.position, playerTarget.transform.position - navMeshAgent.transform.position, out RaycastHit rayHit, (Vector3.Distance(playerTarget.position, navMeshAgent.transform.position) - 3), obstacleLayer)) // checks if player behind things?
            {
                return false;
            }
            else
            {
                Vector3 directionToTarget = (playerTarget.position - navMeshAgent.transform.position).normalized;
                if (Vector3.Angle(navMeshAgent.transform.forward, directionToTarget) < visionConeWidth / 2)
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
        waterOrb.SetActive(true);
        rigidBody.useGravity = true;
        movementState = "rolling_idle";
    }

    public override void DeactivateBehavior() // fly mode
    {
        base.DeactivateBehavior();
    }
}
