using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines.Interpolators;


[RequireComponent(typeof(Light))]
public class EnemyAI_SpiteBulb : EnemyAI_Base
{
    [Header("Bulb General")]
    //public float idleActivateRange;
    [SerializeField] private string movementState = "idle";
    [SerializeField] private MeshRenderer newLightBulbHead;
    private bool healing = false;
    private GameObject bulbHead;
    [SerializeField] private bool headCanTurn = false;
    public float followGracePeriod = 1;
    private float followTimer = 0;
    private Animator bulbBodyAnimator;

    private bool standingUp = false;

    [SerializeField] private Material litBulbColor;
    [SerializeField] private Material unlitBulbColor;

    [Header("Bulb Vision")]
    [SerializeField] private float idleAlertRange;
    [SerializeField] private float blindFollowRange;
    [SerializeField] private float visionConeLength;
    [SerializeField] private float visionConeWidth;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Bulb Wandering")]
    [SerializeField] private float wanderDistance = 5;
    [SerializeField] private float randomWanderLocations = 3;
    private bool wandering = false;
    [SerializeField] private bool wander_headTurning = false;
    private bool searchAnimationOccuring = false;
    private Vector3 wanderHome;
    [SerializeField] private Vector3 wanderLookDirection;

    [Header("Bulb Combat")]
    [SerializeField] private bool preparingAttack = false;
    [SerializeField] private bool attackOccuring = false;
    [SerializeField] private string attackState = "neutral";
    //[SerializeField] private float aggressionLevel = 3;
    [SerializeField] private bool canAttack = false;
    [SerializeField] private float meleeAttackCooldown = 5;
    [SerializeField] private float shockwaveAttackCooldown = 7;
    
    [SerializeField] private float attackCooldownTimer = 0;
    [SerializeField] private float longRangeAttackDis = 10;

    [Range(0f, 100f)]
    [SerializeField] private float shockwaveAttackChance;

    [Header("Bulb Attack Melee")]
    private float gapCloseTimer = 2.3f;

    [Header("Bulb Laser Attack")]
    [SerializeField] private Transform laser_firePosition;
    private bool laser_inProgress = false;
    [SerializeField] private LayerMask laser_targetLayers;
    [SerializeField] private LineRenderer laser_lineRenderer;
    [SerializeField] private GameObject laser_endSphere;
    [SerializeField] private float laserAttackCooldown = 9;

    [Header("Bulb Scared Behavior")]
    [SerializeField] private float scaredFollowRange = 15f;
    [SerializeField] private float scaredAttackTimer = 2f;
    [SerializeField] private float teleportTimer = 8f;
    [SerializeField] private float teleportRange = 10f;
    [SerializeField] private float teleportExplosionDamage = 3f;
    [SerializeField] private GameObject teleportExplosion;
    [SerializeField] private GameObject teleportLocationIndicator;

    [Header("Bulb Circling")]
    [SerializeField] private float circleSpeed = 10;
    [SerializeField] private float circleRadius = 5;
    [SerializeField] private float circlingRange = 8;
    [SerializeField] private bool circlingPlayer = false;
    private float currentAngle = 0;


    [Header("Bulb Shockwave Attack")]
    //private bool shockwave_inProgress = false;
    [SerializeField] private Animator shockwaveAnimator => transform.Find("ShockwaveAttack").GetComponent<Animator>();


    [Header("Lighting")]
    [SerializeField] private Animator lightAnimator;

    public bool testBool = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        bulbHead = transform.Find("Head").gameObject;
        laser_firePosition = transform.Find("Head/LaserFirePosition");
        laser_lineRenderer = transform.Find("Head/LaserRenderer").GetComponent<LineRenderer>();
        laser_endSphere = transform.Find("Head/LaserEndPosSphere").gameObject;
        healthBar = transform.Find("Canvas/Bar").GetComponent<RectTransform>();
        bulbBodyAnimator = transform.Find("NewBody").GetComponent<Animator>();
        bulbBodyAnimator.Play("Bulb_Sleep", 0, 50);
    }
    // Update is called once per frame
    void Update()
    {
        if (navMeshAgent.velocity.magnitude > 0.1f)
        {
            bulbBodyAnimator.SetBool("Walking", true);
        }
        else
        {
            bulbBodyAnimator.SetBool("Walking", false);
        }

        if (!preparingAttack)
        {
            if (movementState == "idle")
            {
                if (!healing)
                {
                    healing = true;
                    StartCoroutine("HealTimer");
                }
                
                if (standingUp)
                {
                    //Debug.Log(bulbBodyAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
                    if (bulbBodyAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "BulbActivate_A")
                    {
                        headCanTurn = true;
                        movementState = "wandering";
                        standingUp = false;
                    }
                }

                if ((PlayerInAwakeRange() || !behaviorActive) && !standingUp)
                {
                    standingUp = true;
                    healing = false;
                    StopCoroutine("HealTimer");
                    bulbBodyAnimator.SetBool("Awake", true);
                }

                SetHeadTarget(transform.forward, 5);
            }
            else if (movementState == "wandering")
            {
                if (PlayerVisible()) // either the player gets into range
                {
                    movementState = "pursue";
                    StopCoroutine("WanderTimer");
                    searchAnimationOccuring = false;
                    wandering = false;
                    wander_headTurning = false;
                    headCanTurn = true;
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
                        if (wander_headTurning) // wandering is occuring already, so head turns to the look direction from wander coroutine
                        {
                            SetHeadTarget(wanderLookDirection - bulbHead.transform.position, 7);
                        }
                        else // in between looking around, looks towards it's destination (the new spot it will walk)
                        {
                            SetHeadTarget(navMeshAgent.destination - bulbHead.transform.position, 15);
                        }
                    }
                }
            }
            else if (movementState == "pursue")
            {
                if (PlayerVisible())
                {
                    if (behaviorActive)
                    {
                        navMeshAgent.destination = playerTarget.position;
                    }
                    else // backs awway when scared
                    {
                        if (Vector3.Distance(playerTarget.position, transform.position) < scaredFollowRange) // way too close!!! gets away
                        {
                            Vector3 newPos = transform.position + ((transform.position - playerTarget.position).normalized * (scaredFollowRange - Vector3.Distance(playerTarget.position, transform.position)));
                            navMeshAgent.destination = newPos;
                        }
                        else if (Vector3.Distance(playerTarget.position, transform.position) > scaredFollowRange + 5f) // too far !!! follows closer.
                        {
                            navMeshAgent.destination = playerTarget.position;
                        }
                    }

                    if ((PlayerInAttackRange() && attackCooldownTimer <= 0 && behaviorActive) || (attackCooldownTimer <= 0 && scaredAttackTimer <= 0))
                    {
                        PerformAttack();
                    }

                    followTimer = 0;

                    if (!behaviorActive && Vector3.Distance(playerTarget.position, transform.position) < 3.5f) // if player gets close too long!
                    {
                        Debug.Log("PLAYER IN SCARED RANGE !!!");
                        scaredAttackTimer -= Time.deltaTime;
                    }
                    else
                    {
                        scaredAttackTimer = 2;
                    }

                    SetHeadTarget(playerTarget.position - bulbHead.transform.position, 15);
                }
                else if (followTimer < followGracePeriod)
                {
                    navMeshAgent.destination = playerTarget.position;
                    followTimer += Time.deltaTime;

                    SetHeadTarget(playerTarget.position - bulbHead.transform.position, 15);
                }
                else
                {
                    movementState = "wandering";
                }
            }

            if (!behaviorActive)
            {
                teleportTimer -= Time.deltaTime;
                if (teleportTimer <= 0)
                {
                    teleportTimer = Random.Range(8f, 16f);
                    StartCoroutine("RandomTeleport");
                }
            }
        }
        else
        {
            if (attackState == "melee") // basically, makes the guy run to the player for a few seconds
            {
                if (PlayerVisible())
                {
                    navMeshAgent.destination = playerTarget.position;
                    SetHeadTarget(playerTarget.position - bulbHead.transform.position, 15);
                }

                if (!attackOccuring)
                {
                    if (gapCloseTimer <= 0 || navMeshAgent.remainingDistance < 0.6f) // attacks if in range or after a few seconds of running if not
                    {
                        StopCoroutine("MeleeAttack");
                        StartCoroutine("MeleeAttack");
                    }
                    else
                    {
                        gapCloseTimer -= Time.deltaTime;
                    }
                }
            }
            else if (attackState == "shockwave")
            {
                if (PlayerVisible())
                {
                    navMeshAgent.destination = playerTarget.position;
                    SetHeadTarget(playerTarget.position - bulbHead.transform.position, 15);
                }

                if (!attackOccuring)
                {
                    if (gapCloseTimer <= 0 || navMeshAgent.remainingDistance < 0.6f) // attacks if in range or after a few seconds of running if not
                    {
                        StopCoroutine("ShockwaveAttack");
                        StartCoroutine("ShockwaveAttack");
                    }
                    else
                    {
                        gapCloseTimer -= Time.deltaTime;
                    }
                }
            }
            else if (attackState == "laser")
            {
                if (attackOccuring)
                {
                    if (laser_inProgress)
                    {
                        SetHeadTarget(playerTarget.position - bulbHead.transform.position, 15);
                    }
                }
                else
                {
                    if (PlayerVisible())
                    {
                        if (Vector3.Distance(playerTarget.position, transform.position) < longRangeAttackDis) // way too close!!! gets away
                        {
                            Vector3 newPos = transform.position + ((transform.position - playerTarget.position).normalized * (longRangeAttackDis - Vector3.Distance(playerTarget.position, transform.position)));
                            navMeshAgent.destination = newPos;
                        }
                        else if (Vector3.Distance(playerTarget.position, transform.position) > longRangeAttackDis + 5f) // too far !!! follows closer.
                        {
                            navMeshAgent.destination = playerTarget.position;
                        }
                    }

                    if (gapCloseTimer <= 0 || navMeshAgent.remainingDistance < 0.3f) // attacks if in range or after a few seconds of running if not
                    {
                        StopCoroutine("LaserAttack");
                        StartCoroutine("LaserAttack");
                    }
                    else
                    {
                        gapCloseTimer -= Time.deltaTime;
                    }
                }
            }

            if (!PlayerVisible()) // in case the player runs away while an attack was occuring
            {

            }
        }

        if (testBool)
        {
            //if (Vector3.Distance(transform.position, playerTarget.position) < circlingRange)
            //{
            //    if (!circlingPlayer)
            //    {
            //        circlingPlayer = true;
            //        currentAngle = Random.Range(0f, 360f);
            //        navMeshAgent.speed = 1;
            //        navMeshAgent.stoppingDistance = 0;
            //    }
            //    else
            //    {
            //        CirclePlayer();
            //    }
            //}
            //else
            //{
            //    circlingPlayer = false;
            //    navMeshAgent.speed = 3.5f;
            //    navMeshAgent.stoppingDistance = 3.5f;
            //    navMeshAgent.destination = playerTarget.position;
            //}
        }
    }

    private void FixedUpdate()
    {
        if (laser_inProgress)
        {
            UpdateLaserPosition();
        }
    }

    IEnumerator RandomTeleport()
    {
        bool teleportSpotFound = false;
        Vector3 teleportPosition = transform.position;
        int failureAttempts = 0;
        while (!teleportSpotFound)
        {
            failureAttempts++;
            Debug.Log("Attempt Number " + failureAttempts + "!");
            teleportPosition = new Vector3(Random.Range(-teleportRange, teleportRange), Random.Range(-teleportRange, teleportRange), Random.Range(-teleportRange, teleportRange));
            NavMeshHit hit;
            if (NavMesh.FindClosestEdge(teleportPosition, out hit, NavMesh.AllAreas))
            {
                Debug.Log("HIT FOUND!");
                teleportPosition = hit.position;
                teleportSpotFound = true;
            }
            if (failureAttempts == 10)
            {
                Debug.Log("FAILURE!");
                teleportSpotFound = true;
            }
        }

        Instantiate(teleportLocationIndicator, teleportPosition, Quaternion.identity);
        yield return new WaitForSeconds(3);

        //navMeshAgent.updatePosition = false;
        transform.position = teleportPosition;
        //navMeshAgent.updatePosition = true;
        teleportExplosion.SetActive(true);
    }

    public void CirclePlayer()
    {
        currentAngle += circleSpeed * Time.deltaTime;
        currentAngle %= 360; // Keep angle within 0-360 degrees

        // Calculate the point on the circle
        float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * circleRadius;
        float z = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * circleRadius;
        Vector3 circlePos = new Vector3(x, 0, z);

        navMeshAgent.destination = playerTarget.position + circlePos;
        //navMeshAgent.updateRotation = false;

        //Vector3 direction = playerTarget.position - transform.position;
        //Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * circlingRotationSpeed);
        //transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);

        //if (targetDistance > attackDistance)
        //{
        //    circlePlayer = false;
        //    navMeshAgent.isStopped = true;
        //    animator.SetBool("Attack", true);
        //}

        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * circlingRotationSpeed);
    }

    public void PerformAttack()
    {
        preparingAttack = true;
        if (Vector3.Distance(playerTarget.position, transform.position) > longRangeAttackDis && behaviorActive)
        {
            EnterLaserAttack();
        }
        else
        {
            float ran = Random.Range(0.1f, 99.9f);
            if (behaviorActive && ran < shockwaveAttackChance)
            {
                EnterShockwaveAttack();
            }
            else
            {
                EnterMeleeAttack();
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

    public void EnterMeleeAttack()
    {
        gapCloseTimer = 3;
        attackState = "melee";
        navMeshAgent.stoppingDistance = 2f;
        navMeshAgent.speed = 6f;
    }

    public void ExitMeleeAttack()
    {
        AttackCooldown(meleeAttackCooldown);
        navMeshAgent.stoppingDistance = 3.5f;
        navMeshAgent.isStopped = false;
        attackState = "neutral";
        navMeshAgent.speed = 3;
        //navMeshAgent.angularSpeed = 120;
        preparingAttack = false;
        attackOccuring = false;
    }

    IEnumerator MeleeAttack()
    {
        attackOccuring = true;
        navMeshAgent.isStopped = true;
        //navMeshAgent.angularSpeed = 0;
        bulbBodyAnimator.Play("Melee_Attack");
        Debug.Log("IMAGINE A MELEE ANIMATION!");

        yield return new WaitForSeconds(3);


        ExitMeleeAttack();
    }

    public void EnterShockwaveAttack()
    {
        gapCloseTimer = 1.3f;
        attackState = "shockwave";
        navMeshAgent.stoppingDistance = 2f;
        navMeshAgent.speed = 6f;
    }

    public void ExitShockwaveAttack()
    {
        AttackCooldown(shockwaveAttackCooldown);
        navMeshAgent.isStopped = false;
        headCanTurn = true;
        attackState = "neutral";
        shockwaveAnimator.Play("ShockwaveTest2"); // delete maybe?
        navMeshAgent.speed = 3;
        navMeshAgent.stoppingDistance = 3.5f;
        preparingAttack = false;
        attackOccuring = false;
        StopCoroutine("ShockwaveAttack");
    }

    IEnumerator ShockwaveAttack()
    {
        attackOccuring = true;
        navMeshAgent.isStopped = true;
        headCanTurn = false;
        bulbBodyAnimator.Play("Crouch");

        shockwaveAnimator.Play("ShockwaveTest");
        yield return new WaitForSeconds(2.8f);

        bulbBodyAnimator.SetTrigger("Arise");
        yield return new WaitForSeconds(2f);

        ExitShockwaveAttack();
    }

    public void EnterLaserAttack()
    {
        gapCloseTimer = 1.5f;
        attackState = "laser";
        navMeshAgent.speed = 4f;
    }

    public void ExitLaserAttack()
    {
        AttackCooldown(laserAttackCooldown);
        headCanTurn = true;
        navMeshAgent.isStopped = false;
        attackState = "neutral";
        laser_inProgress = false;
        preparingAttack = false;
        attackOccuring = false;

        laser_lineRenderer.gameObject.SetActive(false);
        laser_endSphere.SetActive(false);
        transform.Find("Head/Capsule").gameObject.SetActive(false);
        StopCoroutine("LaserAttack");
    }

    IEnumerator LaserAttack()
    {
        attackOccuring = true;
        navMeshAgent.isStopped = true;

        Debug.Log("Charging Laser...");
        laser_lineRenderer.gameObject.SetActive(true);
        laser_endSphere.SetActive(true);
        laser_inProgress = true;
        headCanTurn = true;
        yield return new WaitForSeconds(7);

        Debug.Log("No longer turning head...");
        laser_inProgress = false;
        headCanTurn = false;
        laser_lineRenderer.gameObject.SetActive(false);
        laser_endSphere.SetActive(false);
        yield return new WaitForSeconds(0.6f);

        Debug.Log("LASER FIRED BOOOOOM");
        transform.Find("Head/Capsule").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        transform.Find("Head/Capsule").gameObject.SetActive(false);

        ExitLaserAttack();
    }

    public void UpdateLaserPosition()
    {
        laser_lineRenderer.SetPosition(0, laser_firePosition.position);

        Vector3 endPosition = laser_firePosition.position + (laser_firePosition.forward * 100);

        if (Physics.Raycast(laser_firePosition.position, laser_firePosition.forward, out RaycastHit hit, 100f, laser_targetLayers))
        {
            if (hit.transform.GetComponent<Player_Walk>() != null)
            {
                endPosition = hit.point + laser_firePosition.forward * 0.15f;
            }
            else
            {
                endPosition = hit.point;
            }
        }

        laser_lineRenderer.SetPosition(1, endPosition);
        laser_endSphere.transform.position = laser_lineRenderer.GetPosition(1);
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

        randomWanderLocations = Random.Range(2, 4);

        if (behaviorActive)
        {
            for (int i = 0; i < randomWanderLocations; i++)
            {
                Vector3 potentialLocation = Vector3.zero;
                bool walkable = false;

                while (!walkable)
                {
                    potentialLocation = new Vector3(wanderHome.x + Random.Range(-wanderDistance, wanderDistance), wanderHome.y, wanderHome.z + Random.Range(-wanderDistance, wanderDistance));
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


                wander_headTurning = true; // causes the actual rotation of the head to begin (found in update)

                for (int _i = 0; _i < 5; _i++) // does head turn thingy 3 times :)
                {
                    Vector3 lookLocation = new Vector3(transform.position.x + Random.Range(-wanderDistance, wanderDistance), transform.position.y, transform.position.z + Random.Range(-wanderDistance, wanderDistance));
                    wanderLookDirection = lookLocation;
                    yield return new WaitForSeconds(1);
                }

                wander_headTurning = false;
            }
        }
        else
        {
            while(!behaviorActive)
            {
                Vector3 potentialLocation = Vector3.zero;
                bool walkable = false;

                while (!walkable)
                {
                    potentialLocation = new Vector3(transform.position.x + Random.Range(-wanderDistance*2, wanderDistance*2), transform.position.y, transform.position.z + Random.Range(-wanderDistance*2, wanderDistance*2));
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


                wander_headTurning = true; // causes the actual rotation of the head to begin (found in update)

                for (int _i = 0; _i < 2; _i++) // does head turn thingy 3 times :)
                {
                    Vector3 lookLocation = new Vector3(transform.position.x + Random.Range(-wanderDistance, wanderDistance), transform.position.y, transform.position.z + Random.Range(-wanderDistance, wanderDistance));
                    wanderLookDirection = lookLocation;
                    yield return new WaitForSeconds(1);
                }

                wander_headTurning = false;
            }
        }

        yield return new WaitForSeconds(1); // little buffer at the end

        movementState = "idle"; // returns to idle
        bulbBodyAnimator.SetBool("Awake", false);
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
            if (Physics.Raycast(laser_firePosition.position, playerTarget.transform.position - laser_firePosition.position, out RaycastHit rayHit, (Vector3.Distance(playerTarget.position, transform.position) - 3), obstacleLayer)) // checks if player behind things?
            {
                return false;
            }
            else
            {
                Vector3 directionToTarget = (playerTarget.position - transform.position).normalized;
                if (Vector3.Angle(bulbHead.transform.forward, directionToTarget) < visionConeWidth / 2)
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

    public void SetHeadTarget(Vector3 pos, float speed = 5)
    {
        if (headCanTurn)
        {
            Quaternion rotation = Quaternion.Lerp(bulbHead.transform.rotation, Quaternion.LookRotation(pos, Vector3.up), Time.deltaTime * speed);
            //Debug.Log("rotating towards: " + rotation);
            bulbHead.transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
        }
    }

    public override void ActivateBehavior()
    {
        base.ActivateBehavior();
        Material[] newMats = newLightBulbHead.materials;
        newMats[0] = litBulbColor;
        newLightBulbHead.materials = newMats;
    }

    public override void DeactivateBehavior()
    {
        ExitLaserAttack();
        ExitShockwaveAttack();
        base.DeactivateBehavior();
        Material[] newMats = newLightBulbHead.materials;
        newMats[0] = unlitBulbColor;
        newLightBulbHead.materials = newMats;
    }
}
