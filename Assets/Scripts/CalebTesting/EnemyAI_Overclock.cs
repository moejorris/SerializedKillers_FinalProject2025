using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UI;

public class EnemyAI_Overclock : EnemyAI_Base
{
    [Range(-1f, 1f)]
    [SerializeField] private float rotateThing;
    [Header("Overclock General")]
    //public float idleActivateRange;
    [SerializeField] private string movementState = "wandering";
    [SerializeField] private bool bodyCanTurn = false;
    public float followGracePeriod = 1;
    private float followTimer = 0;
    [SerializeField] private float heatLevel = 0;

    [SerializeField] private Material redBodyColor;
    [SerializeField] private Material blueBodyColor;

    [Header("Overclock Vision")]
    [SerializeField] private float idleAlertRange;
    [SerializeField] private float blindFollowRange;
    [SerializeField] private float visionConeLength;
    [SerializeField] private float visionConeWidth;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Overclock Wandering")]
    [SerializeField] private float wanderDistance = 5;
    private bool wandering = false;
    private bool searchAnimationOccuring = false;
    private Vector3 wanderHome;
    [SerializeField] private Vector3 wanderLookDirection;

    [Header("Overclock Combat")]
    [SerializeField] private bool preparingAttack = false;
    [SerializeField] private bool attackOccuring = false;
    [SerializeField] private string attackState = "neutral";
    [SerializeField] private bool canAttack = false;
    [SerializeField] private float dashAttackCooldown = 4;
    [SerializeField] private float flamethrowerAttackCooldown = 3;
    private float attackPrepTimer = 2.3f;

    [SerializeField] private float attackCooldownTimer = 0;
    [SerializeField] private float fireDashDist = 2;

    [SerializeField] private GameObject firePrefab;
    [SerializeField] private LayerMask fireMask;

    [Range(0f, 100f)]
    [SerializeField] private float fireDashChance;

    [Header("Overclock Dash Attack")]
    [SerializeField] private bool flameDashing = false;
    [SerializeField] private float dashSpeed = 1f;
    [SerializeField] private float dashAttackLength = 2;
    private float dashAttackTimer = 0;

    [Header("Overclock Script Change Behavior")]
    [SerializeField] private float stalkingFollowDistance = 15f;


    public bool testBool = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        healthBar = transform.Find("Canvas/Bar").GetComponent<RectTransform>();
        selectedIcon = transform.Find("Canvas/SelectedIcon").GetComponent<Image>();
        healthBar = transform.Find("Canvas/Bar/Health").GetComponent<RectTransform>();
        whiteHealthBar = transform.Find("Canvas/Bar/White").GetComponent<RectTransform>();
    }


    // Update is called once per frame
    public override void Update()
    {
        if (!preparingAttack)
        {
            if (movementState == "wandering")
            {
                if (PlayerVisible()) // either the player gets into range
                {
                    movementState = "pursue";
                    StopCoroutine("WanderTimer");
                    wandering = false;
                    //wander_headTurning = false;
                    //headCanTurn = true;
                    //searchAnimationOccuring = false;
                }
                else
                {
                    if (!wandering) // hasn't started the wander timer coroutine
                    {
                        wandering = true;
                        StopCoroutine("WanderTimer");
                        StartCoroutine("WanderTimer");
                    }
                    else
                    {
                        //if (wander_headTurning) // wandering is occuring already, so head turns to the look direction from wander coroutine
                        //{
                        //    SetHeadTarget(wanderLookDirection - bulbHead.transform.position, 7);
                        //}
                        //else // in between looking around, looks towards it's destination (the new spot it will walk)
                        //{
                        //    SetHeadTarget(navMeshAgent.destination - bulbHead.transform.position, 15);
                        //}
                    }
                }
            }
            else if (movementState == "pursue")
            {
                if (PlayerVisible())
                {
                    if (!behaviorActive)
                    {
                        navMeshAgent.destination = playerTarget.position; // straight toward them in ice mode

                        if (PlayerDistance() < 3 && attackCooldownTimer <= 0)
                        {
                            PerformAttack();
                        }
                    }
                    else // tries to get behind the player(?)
                    {
                        Vector3 _newDirection = Camera.main.transform.position - playerTarget.transform.position; // maybe opposite?
                        Vector3 _newPosition = playerTarget.transform.position + (_newDirection.normalized * 12);
                        _newPosition.y = playerTarget.transform.position.y;
                        navMeshAgent.destination = _newPosition;

                        if (attackCooldownTimer <= 0)
                        {
                            PerformAttack();
                        }
                    }

                    followTimer = 0;
                }
                else if (followTimer < followGracePeriod)
                {
                    navMeshAgent.destination = playerTarget.position;
                    followTimer += Time.deltaTime;
                }
                else
                {
                    movementState = "wandering";
                }
            }
            else if (movementState == "cooldown")
            {
                
            }
        }
        else
        {
            if (attackState == "dash") // basically, makes the guy run to the player for a few seconds
            {
                if (PlayerVisible())
                {
                    TurnBodyTowards(playerTarget.position - transform.position, 15);
                }

                if (!attackOccuring)
                {
                    if (attackPrepTimer <= 0) // attacks if in range or after a few seconds of running if not
                    {
                        StopCoroutine("DashAttack");
                        StartCoroutine("DashAttack");
                    }
                    else
                    {
                        attackPrepTimer -= Time.deltaTime;
                    }
                }

                if (flameDashing) // starts in the coroutine. this makes it go forward until either the dashLengthTimer runs out or it gets close to the player
                {
                    transform.position += (transform.forward.normalized * Time.deltaTime) * dashSpeed;
                    AttemptFlameTrail();

                    dashAttackTimer -= Time.deltaTime;
                    if (dashAttackTimer <= 0 || PlayerDistance() <= 1.3f)
                    {
                        flameDashing = false;
                    }
                }
            }
            else if (attackState == "flamethrower")
            {
                if (PlayerVisible())
                {
                    navMeshAgent.destination = playerTarget.position;
                }

                if (!attackOccuring)
                {
                    if (attackPrepTimer <= 0 || navMeshAgent.remainingDistance < 0.3f) // attacks if in range or after a few seconds of running if not
                    {
                        StopCoroutine("FlamethrowerAttack");
                        StartCoroutine("FlamethrowerAttack");
                    }
                    else
                    {
                        attackPrepTimer -= Time.deltaTime;
                    }
                }
            }

            if (!PlayerVisible()) // in case the player runs away while an attack was occuring
            {

            }
        }

        base.Update();
    }
    public void PerformAttack()
    {
        preparingAttack = true;
        if (PlayerDistance() > fireDashDist && behaviorActive)
        {
            EnterDashAttack();
        }
        else
        {
            float ran = Random.Range(0.1f, 99.9f);
            if (behaviorActive && ran < fireDashChance) // commits fire dash
            {
                EnterDashAttack();
            }
            else // commits not
            {
                EnterFlamethrowerAttack();
            }
        }
    }

    public void AttackCooldown(float time)
    {
        //canAttack = false;
        StopCoroutine("AttackTimer");
        attackCooldownTimer = time;
        StartCoroutine("AttackTimer");
    } // call directly, looks cleaner than stopping and starting.

    public void EnterFlamethrowerAttack()
    {
        attackPrepTimer = 2.5f;
        attackState = "flamethrower";
        navMeshAgent.isStopped = false;
    }
    public void ExitFlamethrowerAttack()
    {
        AttackCooldown(flamethrowerAttackCooldown);
        IncreaseHeatMeter(20);
        navMeshAgent.stoppingDistance = 3.5f;
        navMeshAgent.isStopped = false;
        attackState = "neutral";
        navMeshAgent.speed = 3;
        preparingAttack = false;
        attackOccuring = false;
    }

    IEnumerator FlamethrowerAttack()
    {
        attackOccuring = true;
        navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(1);

        Debug.Log("Flame Attack Animation!!!");
        AttemptFlameTrail();

        ExitFlamethrowerAttack();
    }

    public void EnterDashAttack()
    {
        attackPrepTimer = 1;
        attackState = "dash";
        navMeshAgent.isStopped = true;
    }

    public void ExitDashAttack()
    {
        AttackCooldown(dashAttackCooldown);
        IncreaseHeatMeter(30);
        navMeshAgent.stoppingDistance = 3.5f;
        navMeshAgent.isStopped = false;
        attackState = "neutral";
        navMeshAgent.speed = 3;
        preparingAttack = false;
        attackOccuring = false;
        flameDashing = false;
    }

    IEnumerator DashAttack()
    {
        attackOccuring = true;
        navMeshAgent.isStopped = true;

        dashAttackTimer = dashAttackLength;
        flameDashing = true;
        yield return new WaitUntil(() => !flameDashing);

        Debug.Log("FLAME BOOM!");

        ExitDashAttack();
    }

    public void AttemptFlameTrail()
    {
        Ray raycast = new Ray();
        raycast.origin = transform.position;
        raycast.direction = Vector3.down;

        Debug.DrawRay(transform.position, Vector3.down);

        if (!Physics.Raycast(raycast, 3, fireMask))
        {
            Vector3 spawnPoint = transform.position;
            spawnPoint.y -= 0.7f;
            Instantiate(firePrefab, spawnPoint, Quaternion.identity);
        }

    }

    public void IncreaseHeatMeter(float amount)
    {
        heatLevel += amount;
        if (heatLevel >= 100)
        {
            navMeshAgent.isStopped = true;
            StartCoroutine("CooldownMode");
        }
    }

    IEnumerator CooldownMode()
    {
        movementState = "cooldown";
        while(heatLevel > 0)
        {
            yield return new WaitForSeconds(0.05f);
            heatLevel -= 1;
        }
        heatLevel = 0;
        movementState = "wandering";
        navMeshAgent.isStopped = false;
    }

    IEnumerator AttackTimer()
    {
        Debug.Log("Attack timer started.");

        yield return new WaitForSeconds(attackCooldownTimer);

        //canAttack = true;
        attackCooldownTimer = 0;

        Debug.Log("Attack timer completed.");
    } // called by the AttackCooldown function.

    IEnumerator WanderTimer()
    {
        yield return new WaitUntil(() => navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance + 0.1f); // wait for them to get into position

        wanderHome = navMeshAgent.destination;


        while (movementState == "wandering")
        {
            Vector3 potentialLocation = Vector3.zero;
            bool walkable = false;

            while (!walkable)
            {
                potentialLocation = new Vector3(transform.position.x + Random.Range(-wanderDistance * 2, wanderDistance * 2), transform.position.y, transform.position.z + Random.Range(-wanderDistance * 2, wanderDistance * 2));
                NavMeshPath path = new NavMeshPath();
                navMeshAgent.CalculatePath(potentialLocation, path);
                if (path.status == NavMeshPathStatus.PathComplete) // cycles through this to check if the location is actually viable
                {
                    //Debug.Log("New path made. It's location is: " + potentialLocation + " and it can path there :)");
                    walkable = true;
                }
                else
                {
                    //Debug.Log("New path made. It's location is: " + potentialLocation + " and it cannot path there :(");
                }
            }

            navMeshAgent.isStopped = false;
            navMeshAgent.destination = potentialLocation; // picks spot to walk to

            float stopDistance = navMeshAgent.stoppingDistance;
            navMeshAgent.stoppingDistance = 0.1f;
            yield return new WaitUntil(() => navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance + 0.1f); // wait until they get close
            navMeshAgent.stoppingDistance = stopDistance;
            yield return new WaitForSeconds(2);
        }

        movementState = "idle"; // returns to idle
    }

    public void BeginSpotSearch()
    {
        searchAnimationOccuring = true;
        Invoke("FinishSpotSearch", 3); // TEMPPPP!!
    }

    public void FinishSpotSearch()
    {
        searchAnimationOccuring = false;
    }

    public bool PlayerInAwakeRange()
    {
        if (Vector3.Distance(playerTarget.position, transform.position) < idleAlertRange)
        {
            return true;
        }
        else
        {
            return false;
        }
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

    public float PlayerDistance()
    {
        return Vector3.Distance(playerTarget.position, transform.position);
    }

    public void TurnBodyTowards(Vector3 pos, float speed = 5)
    {
        if (bodyCanTurn)
        {
            Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(pos, Vector3.up), Time.deltaTime * speed);
            transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
        }
    }

    public override void ActivateBehavior()
    {
        base.ActivateBehavior();
        navMeshAgent.speed = 3;
        transform.Find("IceSphere").gameObject.SetActive(false);
        //Material[] newMats = bulbHead.GetComponent<MeshRenderer>().materials;
        //newMats[0] = redBodyColor;
        //bulbHead.GetComponent<MeshRenderer>().materials = newMats;
    }

    public override void DeactivateBehavior()
    {
        ExitDashAttack();
        ExitFlamethrowerAttack();
        base.DeactivateBehavior();
        navMeshAgent.speed = 1;
        transform.Find("IceSphere").gameObject.SetActive(true);
        //Material[] newMats = bulbHead.GetComponent<MeshRenderer>().materials;
        //newMats[0] = blueBodyColor;
        //bulbHead.GetComponent<MeshRenderer>().materials = newMats;
    }
}
