using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines.Interpolators;


[RequireComponent(typeof(Light))]
public class EnemyAI_SpiteBulb : EnemyAI_Base
{
    [Header("Bulb General")]
    public float idleActivateRange;
    [SerializeField] private string movementState = "idle";
    private bool healing = false;
    private GameObject bulbHead;
    [SerializeField] private bool headCanTurn = false;
    public float timeBeforeLosingTarget = 3;
    private float followTimer = 0;

    [Header("Bulb Wandering")]
    [SerializeField] private float wanderDistance = 5;
    [SerializeField] private float randomWanderLocations = 3;
    private bool wandering = false;
    [SerializeField] private bool wander_headTurning = false;
    private bool searchAnimationOccuring = false;
    private Vector3 wanderHome;
    [SerializeField] private Vector3 wanderLookDirection;

    [Header("Bulb Combat")]
    [SerializeField] private string attackState = "neutral";
    [SerializeField] private float aggressionLevel = 3;
    [SerializeField] private bool canAttack = false;
    [SerializeField] private float meleeAttackCooldown = 5;
    [SerializeField] private float shockwaveAttackCooldown = 7;
    [SerializeField] private float laserAttackCooldown = 9;
    [SerializeField] private float attackCooldownTimer = 0;

    [SerializeField] private float meleeAttackMaxRange = 1;
    [SerializeField] private float shockwaveAttackMaxRange = 2.5f;
    [SerializeField] private float laserAttackMaxRange = 6;

    [Header("Bulb Laser Attack")]
    private bool laser_inProgress = false;
    [SerializeField] private Transform laser_firePosition;
    [SerializeField] private GameObject laserRedsight;
    [SerializeField] private LayerMask laser_targetLayers;
    public float laserFollowSpeed = 10;


    [Header("Lighting")]
    [SerializeField] private Animator lightAnimator;

    public bool testBool = false;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        bulbHead = transform.Find("Head").gameObject;
        laser_firePosition = transform.Find("Head/LaserFirePosition");
    }
    // Update is called once per frame
    void Update()
    {
        if (movementState == "idle")
        {
            if (!healing)
            {
                healing = true;
                StartCoroutine(HealTimer());
            }

            if (PlayerInAwakeRange())
            {
                healing = false;
                AttackCooldown(1); // just so it doesn't immediately attack?
                movementState = "pursue";
                headCanTurn = true;
            }

            SetHeadTarget(transform.forward, 5);

            //Vector3 direction = transform.forward;
            //Quaternion rotation = Quaternion.Lerp(bulbHead.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5);
            //bulbHead.transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
        }
        else if (movementState == "wandering")
        {
            if (PlayerInFollowRange())
            {
                movementState = "pursue";
                // Debug.Log("Wander stopped!");
                StopCoroutine("WanderTimer");
                searchAnimationOccuring = false; // may need to be removed? Also, animation could impact this bool.
                wandering = false;
                wander_headTurning = false;
                headCanTurn = true;
            }
            else
            {
                if (!wandering)
                {
                    wandering = true;
                    //Debug.Log("Started wander timer");
                    StartCoroutine("WanderTimer");
                }
                else
                {
                    if (wander_headTurning)
                    {
                        //Quaternion rotation = Quaternion.Lerp(bulbHead.transform.rotation, wanderLookDirection, Time.deltaTime * 7);
                        //bulbHead.transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
                        SetHeadTarget(wanderLookDirection - bulbHead.transform.position, 7);
                    }
                    else
                    {
                        SetHeadTarget(navMeshAgent.destination - bulbHead.transform.position, 15);
                        //Vector3 direction = navMeshAgent.destination - bulbHead.transform.position;
                        //Quaternion rotation = Quaternion.Lerp(bulbHead.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 15);
                        //bulbHead.transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
                    }
                }
            }
        }
        else if (movementState == "pursue")
        {
            if (PlayerInFollowRange()) // if the enemy is attacking, for instance, it should still target player.
            {
                navMeshAgent.destination = playerTarget.position;
                followTimer = 0;
            }
            else if (followTimer < timeBeforeLosingTarget)
            {
                navMeshAgent.destination = playerTarget.position;
                followTimer += Time.deltaTime;
            }
            else
            {
                movementState = "wandering";
                headCanTurn = true;
            }

            if (PlayerInAttackRange())
            {
                if (attackState == "neutral" && canAttack)
                {
                    canAttack = false;
                    PerformAttack();
                }
            }

            SetHeadTarget(playerTarget.position - bulbHead.transform.position, 15);
        }

        if (laser_inProgress)
        {
            UpdateLaserPosition();
        }
    }

    public void PerformAttack()
    {
        bool attackLaser = true;
        if (AttackRange() == "melee" && !attackLaser)
        {
            attackState = "melee";
            StartCoroutine(MeleeAttack());
        }
        else if (AttackRange() == "shockwave" && !attackLaser)
        {
            attackState = "shockwave";
            StartCoroutine(ShockwaveAttack());
        }
        else// if (AttackRange() == "laser")
        {
            attackState = "laser";
            StartCoroutine(LaserAttack());
        }
    }

    public void UpdateLaserPosition()
    {
        Vector3 currentPosition = laserRedsight.GetComponent<LineRenderer>().GetPosition(1);

        laserRedsight.GetComponent<LineRenderer>().SetPosition(0, laser_firePosition.position);

        Vector3 nextPosition = laser_firePosition.transform.forward * 10;

        if (Physics.Raycast(laser_firePosition.position, laser_firePosition.forward, out RaycastHit hit, 100f, laser_targetLayers))
        {

            nextPosition = hit.point;
        }
        else
        {
            nextPosition = laser_firePosition.forward * 10;
            nextPosition.y = laser_firePosition.position.y;
        }

        laserRedsight.GetComponent<LineRenderer>().SetPosition(1, Vector3.Lerp(currentPosition, nextPosition, laserFollowSpeed));
    }

    public void AttackCooldown(float time)
    {
        canAttack = false;
        StopCoroutine("AttackTimer");
        attackCooldownTimer = time;
        StartCoroutine("AttackTimer");
    } // call directly, looks cleaner than stopping and starting.

    IEnumerator MeleeAttack()
    {
        Debug.Log("Moving in to attack...");
        navMeshAgent.stoppingDistance = 2;
        yield return new WaitUntil(() => navMeshAgent.remainingDistance <= 2.3f); // EDIT LATER
        navMeshAgent.isStopped = true;

        Debug.Log("Performing Attack...");
        yield return new WaitForSeconds(2);
        Debug.Log("Attack Complete!");

        navMeshAgent.stoppingDistance = 3.5f;
        navMeshAgent.isStopped = false;
        attackState = "neutral";

        AttackCooldown(meleeAttackCooldown);
    }

    IEnumerator ShockwaveAttack()
    {
        navMeshAgent.isStopped = true;

        Debug.Log("Stopping to charge...");
        yield return new WaitForSeconds(5);
        Debug.Log("KABOOM!");
        yield return new WaitForSeconds(1);

        navMeshAgent.isStopped = false;
        attackState = "neutral";

        AttackCooldown(shockwaveAttackCooldown);
    }

    IEnumerator LaserAttack()
    {
        navMeshAgent.isStopped = true;

        Debug.Log("Charging Laser...");
        laserRedsight.SetActive(true);
        laser_inProgress = true;
        yield return new WaitForSeconds(5);

        Debug.Log("No longer turning head...");
        laser_inProgress = false;
        headCanTurn = false;
        yield return new WaitForSeconds(2);

        Debug.Log("LASER FIRED BOOOOOM");
        laserRedsight.SetActive(false);
        transform.Find("Head/Capsule").gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        transform.Find("Head/Capsule").gameObject.SetActive(false);

        headCanTurn = true;
        navMeshAgent.isStopped = false;
        attackState = "neutral";

        AttackCooldown(laserAttackCooldown);
    }

    IEnumerator AttackTimer()
    {
        Debug.Log("Attack timer started.");

        yield return new WaitForSeconds(attackCooldownTimer);

        canAttack = true;

        Debug.Log("Attack timer completed.");
    } // called by the AttackCooldown function.

    IEnumerator WanderTimer()
    {
        yield return new WaitUntil(() => navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance + 0.1f); // wait for them to get into position

        wanderHome = navMeshAgent.destination;

        randomWanderLocations = Random.Range(2, 4);

        for (int i = 0; i < randomWanderLocations; i++)
        {
            Vector3 potentialLocation = Vector3.zero;
            bool walkable = false;

            while(!walkable)
            {
                potentialLocation = new Vector3(wanderHome.x + Random.Range(-wanderDistance, wanderDistance), wanderHome.y, wanderHome.z + Random.Range(-wanderDistance, wanderDistance));
                NavMeshPath path = new NavMeshPath();
                navMeshAgent.CalculatePath(potentialLocation, path);
                if(path.status == NavMeshPathStatus.PathComplete) // cycles through this to check if the location is actually viable
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


            wander_headTurning = true; // causes the actual rotation of the head to begin (found in update)

            for (int _i = 0; _i < 5; _i++) // does head turn thingy 3 times :)
            {
                Vector3 lookLocation = new Vector3(wanderHome.x + Random.Range(-wanderDistance, wanderDistance), wanderHome.y, wanderHome.z + Random.Range(-wanderDistance, wanderDistance));
                wanderLookDirection = lookLocation;
                yield return new WaitForSeconds(1);
            }

            wander_headTurning = false;
        }

        yield return new WaitForSeconds(1); // little buffer at the end

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

    IEnumerator HealTimer()
    {
        while (healing)
        {
            yield return new WaitForSeconds(2);
            //Debug.Log("Player healing");
            GainHealth(5);
        }
    }

    public bool PlayerInAwakeRange()
    {
        if (Vector3.Distance(playerTarget.position, transform.position) < idleActivateRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool PlayerInFollowRange()
    {
        if (Vector3.Distance(playerTarget.position, transform.position) < followRange)
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

    public string AttackRange()
    {
        if (Vector3.Distance(playerTarget.position, transform.position) > laserAttackMaxRange)
        {
            return "null";
        }
        else if (Vector3.Distance(playerTarget.position, transform.position) > shockwaveAttackMaxRange)
        {
            return "laser";
        }
        else if (Vector3.Distance(playerTarget.position, transform.position) > meleeAttackMaxRange)
        {
            return "shockwave";
        }
        else
        {
            return "melee";
        }
    }

    public void SetHeadTarget(Vector3 pos, float speed = 5)
    {
        if (headCanTurn)
        {
            Quaternion rotation = Quaternion.Lerp(bulbHead.transform.rotation, Quaternion.LookRotation(pos, Vector3.up), Time.deltaTime * speed);
            //Debug.Log("rotating towards: " + rotation);
            bulbHead.transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
        }
    }
}
