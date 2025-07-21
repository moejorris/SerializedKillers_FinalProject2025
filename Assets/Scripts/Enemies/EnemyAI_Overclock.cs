using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI_Overclock : EnemyAI_Base
{
    [Range(-1f, 1f)]
    [SerializeField] private float rotateThing;
    [Header("Overclock General")]
    //public float idleActivateRange;
    public string movementState = "wandering";
    [SerializeField] private bool bodyCanTurn = false;
    public float followGracePeriod = 1;
    private float followTimer = 0;
    [SerializeField] private float heatLevel = 0;

    [SerializeField] private Material redBodyColor;
    [SerializeField] private Material blueBodyColor;
    [SerializeField] private ParticleSystem[] flameParticles;
    [SerializeField] private ParticleSystem[] smokeParticles;
    [SerializeField] private Animator bodyAnimator => transform.Find("OverclockBody").GetComponent<Animator>();

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
    private bool hitPlayer = false;

    [SerializeField] private float attackCooldownTimer = 0;
    [SerializeField] private float fireDashDist = 2;

    [SerializeField] private GameObject firePrefab;
    [SerializeField] private LayerMask fireMask;

    [Range(0f, 100f)]
    [SerializeField] private float fireDashChance;

    [Header("Overclock Dash Attack")]
    [SerializeField] private bool spawningFlames = false;
    [SerializeField] private float dashSpeed = 1f;
    [SerializeField] private float regularWalkSpeed = 1f;

    [SerializeField] private float dashAttackLength = 2;
    private float dashAttackTimer = 0;

    [Header("Overclock Flamethrower Attack")]
    [SerializeField] private ParticleSystem flamethrowerParticles;
    [SerializeField] private float flamethrowerDistance;
    [SerializeField] private float flamethrowerArc;

    [SerializeField] private float arcSweepSpeed;
    [SerializeField] private Vector3 flamethrowerPosition;
    [SerializeField] private float flamethrowerParticleDur = 7;
    private float flameTimer;

    [Header("Overclock Script Change Behavior")]
    [SerializeField] private float stalkingFollowDistance = 15f;
    [SerializeField] private Transform[] iceSpikes;
    [SerializeField] Transform iceFieldTransform;
    [SerializeField] bool playerIsSlowedInIceField = true;
    [SerializeField] bool playerIsSlowedByIceAttack = true;
    [SerializeField] float playerIceAttackSlowDuration = 5f;
    [SerializeField] float iceFieldPlayerSpeedMultiplier = 0.25f;

    [Header("SFX")]
    [SerializeField] private SoundEffectSO sfx_foosteps;
    [SerializeField] private SoundEffectSO sfx_runsteps;
    [SerializeField] private SoundEffectSO sfx_flamethrowerPrep;
    [SerializeField] private SoundEffectSO sfx_flamethrowerFire;
    [SerializeField] private SoundEffectSO sfx_dashPrep;
    [SerializeField] private SoundEffectSO sfx_dashCharge;
    [SerializeField] private SoundEffectSO sfx_damage;
    [SerializeField] private SoundEffectSO sfx_iceDamage;
    [SerializeField] private SoundEffectSO sfx_iceStrike;

    [SerializeField] private SoundEffectSO sfx_iceStrikeLow;

    private bool spikesChangingSize = false;


    public bool testBool = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        healthBar = transform.Find("Canvas/Bar").GetComponent<RectTransform>();
        selectedIcon = transform.Find("Canvas/SelectedIcon").gameObject;
        healthBar = transform.Find("Canvas/Bar/Health").GetComponent<RectTransform>();
        whiteHealthBar = transform.Find("Canvas/Bar/White").GetComponent<RectTransform>();

        ToggleParticles(flameParticles, true);

        if (PlayerController.instance.ScriptSteal.heldBehavior != null && PlayerController.instance.ScriptSteal.heldBehavior == heldBehavior) DeactivateBehavior();
        else ActivateBehavior();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + (transform.forward.normalized * 0.1f), 2.8f);
    }

    // Update is called once per frame
    public override void Update()
    {
        if (navMeshAgent.velocity.magnitude > 0.1f)
        {
            bodyAnimator.SetBool("Walking", true);
        }
        else
        {
            bodyAnimator.SetBool("Walking", false);
        }

        if (!behaviorActive)
        {
            if (Vector3.Distance(PlayerController.instance.transform.position, transform.position) < (iceFieldTransform.localScale.x * 0.5f) && PlayerController.instance.MovementMachine.MovementMultiplier == 1 && playerIsSlowedInIceField)
            {
                PlayerController.instance.MovementMachine.SetMovementMultiplier(iceFieldPlayerSpeedMultiplier);
            }
            else if (PlayerController.instance.MovementMachine.MovementMultiplier != 1)
            {
                PlayerController.instance.MovementMachine.RemoveMovementMultiplier();
            }
        }

        if (!preparingAttack)
            {
                if (movementState == "wandering")
                {
                    if (PlayerVisible()) // either the player gets into range
                    {
                        movementState = "pursue";
                        StopCoroutine("WanderTimer");
                        wandering = false;
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

                        }
                    }
                }
                else if (movementState == "pursue")
                {
                    navMeshAgent.stoppingDistance = 3.5f; // MIGHT NEED TO REMOVE?
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
                if (attackState == "dash") // called during the attack
                {
                    if (PlayerInCone())
                    {
                        navMeshAgent.destination = playerTarget.position;
                        TurnBodyTowards(playerTarget.position - transform.position, 4f);
                    }
                    else if (PlayerVisible())
                    {
                        TurnBodyTowards(playerTarget.position - transform.position, 8);
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

                    if (spawningFlames) // starts in the coroutine. this makes it go forward until either the dashLengthTimer runs out or it gets close to the player
                    {
                        //transform.position += (transform.forward.normalized * Time.deltaTime) * dashSpeed;
                        SpawnFire(transform.position);

                        dashAttackTimer -= Time.deltaTime;
                        if (dashAttackTimer <= 0 || navMeshAgent.remainingDistance < 1.5f)
                        {
                            spawningFlames = false;
                        }
                    }
                }
                else if (attackState == "flamethrower")
                {
                    if (PlayerVisible())
                    {
                        navMeshAgent.destination = playerTarget.position;
                    }

                    if (spawningFlames)
                    {
                        flameTimer += Time.deltaTime * 100;
                        //Debug.Log(flameTimer);

                        Vector3 direction = transform.forward;
                        Vector3 axis = Vector3.up;
                        Quaternion rotationAxis = Quaternion.AngleAxis(flameTimer, axis);
                        Vector3 rotatedDirectionalAxis = (rotationAxis * direction).normalized;
                        rotatedDirectionalAxis.y = transform.position.y;
                        rotatedDirectionalAxis *= flamethrowerDistance;

                        flamethrowerPosition = transform.position + rotatedDirectionalAxis;

                        Debug.DrawRay(transform.position, rotatedDirectionalAxis, Color.red);

                        if (flameTimer > flamethrowerArc / 2)
                        {
                            flameTimer = -flamethrowerArc / 2;
                        }

                        SpawnFire(flamethrowerPosition, 5);
                    }

                    if (!attackOccuring)
                    {
                        if (attackPrepTimer <= 0 || navMeshAgent.remainingDistance < 0.5f) // attacks if in range or after a few seconds of running if not
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
        if (Invincible())
        {
            AttackCooldown(1);
            return;
        }

        preparingAttack = true;
        hitPlayer = false;

        if (behaviorActive)
        {
            if (PlayerDistance() > fireDashDist)
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
        else
        {
            EnterIceAttack();
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
    public void ExitFlamethrowerAttack(bool increaseMeter = true)
    {
        AttackCooldown(flamethrowerAttackCooldown);
        bodyAnimator.Play("Idle", 0, 0);
        navMeshAgent.stoppingDistance = 3.5f;
        navMeshAgent.isStopped = false;
        spawningFlames = false;
        attackState = "neutral";
        flamethrowerParticles.Stop();
        navMeshAgent.speed = 3;
        preparingAttack = false;
        attackOccuring = false;
        if (increaseMeter) IncreaseHeatMeter(40);
    }

    IEnumerator FlamethrowerAttack()
    {
        PlaySound(sfx_flamethrowerPrep, true);
        bodyAnimator.Play("SweepATK_Start", 0, 0);
        attackOccuring = true;
        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(1.5f);
        PlaySound(sfx_flamethrowerFire, true);
        bodyAnimator.Play("SweepATK_Sweeping", 0, 0);

        flamethrowerParticles.Play();
        spawningFlames = true;
        yield return new WaitForSeconds(4.8f);
        spawningFlames = false;

        ExitFlamethrowerAttack();
    }

    public void EnterDashAttack()
    {
        attackPrepTimer = 1;
        navMeshAgent.stoppingDistance = 1;

        bodyAnimator.Play("Charge_Start", 0, 0);

        PlaySound(sfx_dashPrep, true);
        attackState = "dash";
        navMeshAgent.isStopped = true;
    }

    public void ExitDashAttack(bool increaseMeter = true)
    {
        navMeshAgent.speed = regularWalkSpeed;
        navMeshAgent.stoppingDistance = 3.5f;

        AttackCooldown(dashAttackCooldown);
        navMeshAgent.stoppingDistance = 3.5f;
        navMeshAgent.isStopped = false;
        attackState = "neutral";
        bodyAnimator.Play("Idle", 0, 0);
        navMeshAgent.speed = 3;
        preparingAttack = false;
        attackOccuring = false;
        spawningFlames = false;
        if (increaseMeter) IncreaseHeatMeter(40);
    }

    IEnumerator DashAttack()
    {
        attackOccuring = true;

        dashAttackTimer = dashAttackLength;
        spawningFlames = true;
        PlaySound(sfx_dashCharge, true);
        bodyAnimator.Play("Charging", 0, 0);
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = dashSpeed;

        yield return new WaitUntil(() => !spawningFlames);

        navMeshAgent.velocity = transform.forward.normalized;
        navMeshAgent.isStopped = true;
        bodyAnimator.Play("Charge_Stop", 0, 0);

        yield return new WaitForSeconds(1.2f);

        PlaySound(sfx_iceStrikeLow);
        DashHitCheck();

        yield return new WaitForSeconds(0.5f);

        ExitDashAttack();
    }

    public void EnterIceAttack()
    {
        preparingAttack = true;
        attackOccuring = true;
        navMeshAgent.velocity = transform.forward.normalized;
        navMeshAgent.isStopped = true;
        attackState = "spike";
        StopCoroutine("IceAttack");
        StartCoroutine("IceAttack");
    }

    IEnumerator IceAttack()
    {
        yield return new WaitUntil(() => !spikesChangingSize);

        PlaySound(sfx_iceStrike);
        bodyAnimator.Play("ScriptStolen", 0, 0);

        yield return new WaitForSeconds(1);

        float iceScale = 0.8f;

        while (iceScale < 2.8f)
        {
            foreach (Transform iceSpike in iceSpikes)
            {
                iceSpike.localScale = (Vector3.one * iceScale);
            }
            iceScale += 0.1f;
            yield return new WaitForSeconds(0.0005f);
        }

        IceHitCheck();

        while (iceScale > 2f)
        {
            foreach (Transform iceSpike in iceSpikes)
            {
                iceSpike.localScale = (Vector3.one * iceScale);
            }
            iceScale -= 0.1f;
            yield return new WaitForSeconds(0.005f);
        }

        bodyAnimator.Play("ScriptStolenReverse", 0, 0);
        yield return new WaitForSeconds(0.5f);

        while (iceScale > 0.8f)
        {
            foreach (Transform iceSpike in iceSpikes)
            {
                iceSpike.localScale = (Vector3.one * iceScale);
            }
            iceScale -= 0.1f;
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(0.3f);

        AttackCooldown(2);
        ExitIceAttack();
    }

    public void ExitIceAttack()
    {
        preparingAttack = false;
        attackOccuring = false;
        navMeshAgent.isStopped = false;
        attackState = "neutral";
        StopCoroutine("IceAttack");

        Vector3 correctScale = Vector3.one * 0.8f;
        foreach (Transform iceSpike in iceSpikes)
        {
            iceSpike.localScale = correctScale;
        }

        bodyAnimator.Play("Idle", 0, 0);
    }

    public void DashHitCheck()
    {
        Debug.Log("HIT CHECK!");

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1.8f, transform.forward, 2.5f, playerLayer);

        //Debug.Log(hits.Length);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.transform.gameObject.name);
            if (hit.collider == PlayerController.instance.Collider && !hitPlayer)
            {
                //Debug.Log("Player Hit!");
                hitPlayer = true;
                PlayerController.instance.Health.TakeDamage(4);
                Vector3 dir = transform.forward.normalized;
                dir.y = 0.5f;
                PlayerController.instance.ForceHandler.AddForce(dir * 15, ForceMode.VelocityChange);
                PlayerController.instance.ScriptSteal.ApplyStatusEffect(heldBehavior);
                break;
            }
        }
    }

    public void IceHitCheck()
    {
        Debug.Log("ICE CHECK!");

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 3.5f, transform.forward, 0.1f, playerLayer);

        //Debug.Log(hits.Length);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.transform.gameObject.name);
            if (hit.collider == PlayerController.instance.Collider && !hitPlayer)
            {
                //Debug.Log("Player Hit!");
                hitPlayer = true;

                if (playerIsSlowedByIceAttack)
                {
                    PlayerController.instance.MovementMachine.SetMovementMultiplier(playerIceAttackSlowDuration);
                    PlayerController.instance.MovementMachine.Invoke("RemoveMovementMultiplier", 3f);
                }

                PlayerController.instance.Health.TakeDamage(4);
                Vector3 dir = transform.forward.normalized;
                dir.y = 0.5f;
                PlayerController.instance.ForceHandler.AddForce(dir * 5, ForceMode.VelocityChange);
                PlayerController.instance.ScriptSteal.ApplyStatusEffect(heldBehavior.weakness);
                break;
            }
        }
    }

    public void SpawnFire(Vector3 location, float fireDur = 3f)
    {
        location.y = transform.position.y + 2;
        RaycastHit[] fireCheck = Physics.SphereCastAll(location, 0.5f, Vector3.down, 20, fireMask);
        if (fireCheck.Length <= 0)
        {
            Ray raycast = new Ray();
            raycast.origin = location;
            raycast.direction = Vector3.down;

            Vector3 spawnPoint = location;
            Physics.Raycast(raycast, out RaycastHit hit2, 20, obstacleLayer);
            spawnPoint.y = hit2.point.y + 0.2f;

            if (Mathf.Abs(spawnPoint.y - location.y) > 4f) return; // stops from placing fire too high or too low?

            FireHazard fire = Instantiate(firePrefab, spawnPoint, Quaternion.identity).GetComponent<FireHazard>();
            fire.fireDuration = fireDur;
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
        bodyAnimator.Play("ScriptStolen", 0, 0);
        ToggleParticles(smokeParticles, true);
        ToggleParticles(flameParticles, false);
        movementState = "cooldown";
        while (heatLevel > 0)
        {
            yield return new WaitForSeconds(0.03f);
            heatLevel -= 1;
        }

        bodyAnimator.Play("ScriptStolenReverse", 0, 0);
        yield return new WaitForSeconds(0.8f);
        heatLevel = 0;
        bodyAnimator.Play("Idle", 0, 0);
        movementState = "wandering";
        ToggleParticles(smokeParticles, false);
        ToggleParticles(flameParticles, true);
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

    IEnumerator SpikesAppear()
    {
        if (iceSpikes.Length > 0)
        {
            spikesChangingSize = true;

            float iceScale = iceSpikes[0].localScale.x;

            if (iceScale < 0.8)
            {
                while (iceScale < 0.8f)
                {
                    foreach (Transform iceSpike in iceSpikes)
                    {
                        iceSpike.localScale = (Vector3.one * iceScale);
                    }
                    iceScale += 0.1f;
                    yield return new WaitForSeconds(0.05f);
                }
            }
            else if (iceScale > 0.8)
            {
                while (iceScale > 0.8f)
                {
                    foreach (Transform iceSpike in iceSpikes)
                    {
                        iceSpike.localScale = (Vector3.one * iceScale);
                    }
                    iceScale -= 0.1f;
                    yield return new WaitForSeconds(0.05f);
                }
            }

            spikesChangingSize = false;
        }
    }

    IEnumerator SpikesDisappear()
    {
        if (iceSpikes.Length > 0)
        {
            spikesChangingSize = true;

            float iceScale = iceSpikes[0].localScale.x;


            while (iceScale > 0f)
            {
                foreach (Transform iceSpike in iceSpikes)
                {
                    iceSpike.localScale = (Vector3.one * iceScale);
                }
                iceScale -= 0.1f;
                yield return new WaitForSeconds(0.05f);
            }

            spikesChangingSize = false;
        }
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

    public void ToggleParticles(ParticleSystem[] particles, bool on)
    {
        if (on)
        {
            foreach (ParticleSystem particle in particles)
            {
                particle.Play();
            }
        }
        else
        {
            foreach (ParticleSystem particle in particles)
            {
                particle.Stop();
            }
        }
    }

    public bool PlayerInCone()
    {
        if (Vector3.Distance(playerTarget.position, transform.position) <= visionConeLength) // at least within sight range
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
            }
        }

        return false;
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
        ExitIceAttack();
        base.ActivateBehavior();
        navMeshAgent.speed = 3;
        transform.Find("IceSphere").gameObject.SetActive(false);

        StopCoroutine("SpikesAppear");
        StopCoroutine("SpikesDisappear");
        StartCoroutine("SpikesDisappear");

        ToggleParticles(flameParticles, true);
    }

    public override void TakeDamage(float damage)
    {
        if (!healthBar || !whiteHealthBar || Invincible()) return; // in case no thing exists

        if ((PlayerController.instance.ScriptSteal.BehaviorActive() && PlayerController.instance.ScriptSteal.GetHeldBehavior() == heldBehavior.weakness) ||
            (PlayerController.instance.ScriptSteal.GetHeldBehavior() == heldBehavior && !behaviorActive && PlayerController.instance.ScriptSteal.BehaviorActive())) damage *= 2f;

        if (movementState == "cooldown") damage *= 1.5f;

        health -= damage;

        if (behaviorActive) PlaySound(sfx_damage);
        else PlaySound(sfx_iceDamage);

        if (!PlayerController.instance.ScriptSteal.BehaviorActive())
        {
            PlayerController.instance.Mana.GainMana(manaPerHit);
        }

        StopCoroutine("MaterialFade");
        StartCoroutine("MaterialFade");

        UpdateHealth();

        if (health <= 0)
        {
            Die();
        }
    }

    public override void DeactivateBehavior()
    {
        ExitDashAttack(false);
        ExitFlamethrowerAttack(false);
        heatLevel = 0;
        ToggleParticles(flameParticles, false);
        base.DeactivateBehavior();
        AttackCooldown(1);
        navMeshAgent.speed = 2.5f;
        navMeshAgent.stoppingDistance = 3.5f;
        transform.Find("IceSphere").gameObject.SetActive(true);

        StopCoroutine("SpikesDisappear");
        StopCoroutine("SpikesAppear");
        StartCoroutine("SpikesAppear");

        ToggleParticles(flameParticles, false);
    }
    public override void PlaySound(SoundEffectSO clip, bool fireExclusive = false)
    {
        SoundManager.instance.PlaySoundEffectOnObject(clip, transform);
    }
    public void PlayFootstep()
    {
        if (attackOccuring) PlaySound(sfx_runsteps);
        else PlaySound(sfx_foosteps);
    }
}
