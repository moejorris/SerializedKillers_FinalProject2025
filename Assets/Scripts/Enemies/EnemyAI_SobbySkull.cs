using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Splines;
using System.Collections;

public class EnemyAI_SobbySkull : EnemyAI_Base
{
    private GameObject waterOrb => skull.Find("WaterOrb").gameObject;
    private Animator skullAnimator => skull.transform.Find("RealSkull/AnimatedSkull").GetComponent<Animator>();
    private Transform skull => transform;
    private NavMeshAgent rollingNavMeshAgent;

    [Header("Flight Movement")]
    public bool showDuckObjects = false;
    [SerializeField] private string movementState = "idle";
    [SerializeField] private float followTimer = 15;
    [SerializeField] private bool followingPlayer = false;
    [SerializeField] private bool ducking = false;
    private float doorframeDist = 2;
    [SerializeField] private float flyingSpeed = 5;

    [SerializeField] private float divingSpeed = 5;
    [SerializeField] private float divingTurnSpeed = 1;

    [SerializeField] private float flyingTurnSpeed = 5;
    [SerializeField] private float flyHeight = 4;
    [SerializeField] private List<Vector3> duckPositions = new List<Vector3>();

    [SerializeField] private float newMaxVelocity;
    [SerializeField] private float maxVelocityLerpSpeed = 5;

    [Header("Rolling Movement")]
    [SerializeField] private float gravityLevel = 1;
    [SerializeField] private float rollSpeed = 1;

    [Header("Sobby Skull Combat")]
    [SerializeField] private float attackCooldown = 5;
    public bool selfDestructing = false;
    private float attackTimer = 0;
    private bool hitPlayer = false;
    [SerializeField] private bool attacking = false;
    private bool preparingAttack = false;
    private Vector3 attackDir = Vector3.zero;
    [SerializeField] private GameObject explosionParticle;

    [SerializeField] private GameObject resistantPrefab;

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
        navMeshAgent = skull.parent.Find("RollingAgent").GetComponent<NavMeshAgent>();
        //rollingNavMeshAgent = transform.Find("RollingAgent").GetComponent<NavMeshAgent>();
        rigidBody = skull.parent.Find("Skull").GetComponent<Rigidbody>();
        healthBar = skull.parent.Find("Canvas/Bar/Health").GetComponent<RectTransform>();
        whiteHealthBar = skull.parent.Find("Canvas/Bar/White").GetComponent<RectTransform>();
        selectedIcon = skull.parent.Find("Canvas/SelectedIcon").gameObject;

        newMaxVelocity = rigidBody.maxLinearVelocity;
        maxVelocityLerpSpeed = 15;

        StartRolling();
        StartCoroutine("DoorCheckTimer");

        if (PlayerController.instance.ScriptSteal.heldBehavior != null && PlayerController.instance.ScriptSteal.heldBehavior == heldBehavior) DeactivateBehavior();
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(skull.position, 2.5f);
    }

    public override void Update()
    {
        if (PlayerNearby() || PlayerInLineOfSight())
        {
            followTimer = 15;
            followingPlayer = true;
        }
        else if (followingPlayer)
        {
            followTimer -= Time.deltaTime;
            if (followTimer <= 0) followingPlayer = false;
        }

        if (attacking) // DOING DIVE ATTACK
        {
            if (!preparingAttack) // ACTUAL ATTACK -- retains direction, flies towards player and slowly rotates upwards in order for the consistent transform.forward to make it a dive down and back up
            {
                if (Vector3.Distance(skull.position, playerTarget.position) <= 1.5f) // when the attack gets close, it attempts a melee hit. This can only be attempted once per dive
                {
                    MeleeHitCheck();
                }

                if (FloorDistance() > 1.3f) // Dives at a normal speed unless it gets close to the floor. Then rotates quicker and propels from ground some in order to not crash into the floor
                {
                    divingTurnSpeed = 1;
                }
                else
                {
                    divingTurnSpeed = 50;
                    rigidBody.AddForce(Vector3.up * 15, ForceMode.Acceleration);
                }

                attackDir.y = Mathf.Lerp(attackDir.y, 14f, Time.deltaTime * divingTurnSpeed);
                skull.rotation = Quaternion.Slerp(skull.rotation, Quaternion.LookRotation(attackDir, Vector3.up), Time.deltaTime * flyingTurnSpeed * 1.5f); // always flies to the player
            }
            else // PREPARING -- rotates in place more or less to face the player
            {
                attackDir = playerTarget.position - skull.position;
                skull.rotation = Quaternion.Slerp(skull.rotation, Quaternion.LookRotation(attackDir, Vector3.up), Time.deltaTime * (flyingTurnSpeed / 2)); // always flies to the player
            }
        }
        else
        {
            if (attackTimer > 0) attackTimer -= Time.deltaTime;

            if (movementState == "flying")
            {

                Vector3 TARGET = skull.position;

                if (!ducking) // if the player is flying normally, it goes directly to the player
                {
                    TARGET = playerTarget.transform.position;
                    TARGET.y = Mathf.Lerp(skull.position.y, playerTarget.position.y + flyHeight, flyingTurnSpeed * (1 + Vector3.Distance(playerTarget.position, skull.position))); // makes sure to set the offset, also faster if player higher

                    if (duckPositions.Count > 0)
                    {
                        ducking = true;
                    }
                }
                else // otherwise, the player flies down to a ducked position
                {
                    if (duckPositions.Count > 0) // checks if there are any positions in the array
                    {
                        TARGET = new Vector3(duckPositions[0].x, duckPositions[0].y + 1f, duckPositions[0].z);
                        //TARGET.y += flyHeight;

                        if (Vector3.Distance(skull.position, duckPositions[0]) <= 3)
                        {
                            duckPositions.RemoveAt(0);
                        }
                    }
                    else
                    {
                        ducking = false;
                    }
                }

                skull.rotation = Quaternion.Slerp(skull.rotation, Quaternion.LookRotation(TARGET - skull.position, Vector3.up), Time.deltaTime * flyingTurnSpeed); // always flies to the player

                if (Vector3.Distance(playerTarget.position, skull.position) < 7f && attackTimer <= 0 && PlayerInLineOfSight() && !selfDestructing && !Invincible())
                {
                    BeginDiveAttack();
                }
            }
            else if (movementState == "rolling")
            {

            }
        }

        LerpMaxVelocity();
        base.Update();
    }
    private void FixedUpdate()
    {
        if (!attacking)
        {
            if (movementState == "flying")
            {
                rigidBody.angularVelocity = Vector3.zero;

                if (Vector3.Distance(playerTarget.position, skull.position) < 7f) // close, makes it slow down
                {
                    newMaxVelocity = 1f;
                    maxVelocityLerpSpeed = 5;
                    rigidBody.linearVelocity = Vector3.Lerp(rigidBody.linearVelocity, Vector3.zero, Time.deltaTime * 10);
                }
                else
                {
                    newMaxVelocity = 5;
                }
                rigidBody.AddForce(skull.forward * flyingSpeed, ForceMode.VelocityChange);
            }
            else
            {
                //Debug.Log("Rolling Now!");
                if (Vector3.Distance(navMeshAgent.transform.position, skull.position) > 7 && (Vector3.Distance(navMeshAgent.transform.position, playerTarget.position) < Vector3.Distance(skull.position, playerTarget.position)))
                {
                    navMeshAgent.isStopped = true;
                }
                else
                {
                    navMeshAgent.destination = playerTarget.position;
                    navMeshAgent.isStopped = false;
                }

                if (Vector3.Distance(playerTarget.position, waterOrb.transform.position) < 1.7f)
                {
                    // knockback due to coll.
                }

                if (IsGrounded())
                {
                    // Debug.Log("Grounded!");
                    newMaxVelocity = 3;
                    rigidBody.maxLinearVelocity = 3;
                    rigidBody.AddForce((navMeshAgent.transform.position - skull.position).normalized * rollSpeed, ForceMode.VelocityChange);
                }
                else
                {
                    Debug.Log("Not Grounded!");
                    newMaxVelocity = 15;
                    rigidBody.maxLinearVelocity = 15;
                    rigidBody.AddForce(Physics.gravity * gravityLevel, ForceMode.Acceleration);
                }
            }
        }
        else
        {
            if (movementState == "flying")
            {
                if (Vector3.Distance(skull.position, playerTarget.position) > 3)
                {
                    rigidBody.AddForce(skull.forward * (divingSpeed + 2), ForceMode.VelocityChange);
                }
                else
                {
                    rigidBody.AddForce(skull.forward * divingSpeed, ForceMode.VelocityChange);
                }
            }
        }
    }

    public void MeleeHitCheck()
    {
        RaycastHit[] hits = Physics.SphereCastAll(skull.position, 1.5f, skull.forward, 1, playerLayer);

        //Debug.Log(hits.Length);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == PlayerController.instance.Collider && !hitPlayer)
            {
                //Debug.Log("Player Hit!");
                hitPlayer = true;
                PlayerController.instance.Health.TakeDamage(5);
                break;
            }
        }
    }

    public void EnterSelfDestructMode()
    {
        selfDestructing = true;
        hitPlayer = false;
        flyHeight = 1;
        StopCoroutine("SelfDestructTimer");
        StartCoroutine("SelfDestructTimer");
    }

    IEnumerator SelfDestructTimer()
    {
        //Debug.Log("SELF DESTRUCT MODE ACTIVATED!");

        for (int i = 0; i < 3; i++)
        {
            // Debug.Log("Self Destructing in: " + (3-i));
            yield return new WaitForSeconds(1);
        }

        Explode();
    }

    public void Explode()
    {
        Instantiate(explosionParticle, transform.position, Quaternion.identity);
        RaycastHit[] hits = Physics.SphereCastAll(skull.position, 2f, skull.forward, 0.1f, playerLayer);

        //Debug.Log(hits.Length);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.parent != null && hit.transform.parent.CompareTag("Player") && !hitPlayer)
            {
                hitPlayer = true;
                PlayerController.instance.Health.TakeDamage(8);
                break;
            }
        }

        Die();
    }

    IEnumerator DoorCheckTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (PlayerInLineOfSight() || PlayerNearby() || duckPositions.Count != 0)
            {
                CheckForOverhang();
            }
            SpacialAwareness();
        }
    }

    public void StartRolling()
    {
        attacking = false;
        waterOrb.SetActive(true);
        rigidBody.useGravity = true;
        movementState = "rolling";
        skullAnimator.Play("Fly-Idle", 0, 0);
        navMeshAgent.transform.position = skull.position;
    }

    public void StartFlying()
    {
        attackTimer = 2;
        waterOrb.SetActive(false);
        rigidBody.useGravity = false;
        movementState = "flying";
        skullAnimator.Play("Idle-Fly", 0, 0);
    }

    public void BeginDiveAttack()
    {
        attackTimer = attackCooldown;
        attacking = true;
        preparingAttack = true;
        newMaxVelocity = 0.3f;
        rigidBody.linearVelocity = Vector3.zero;
        hitPlayer = false;
        StartCoroutine("DiveAttack");
    }

    IEnumerator DiveAttack()
    {
        skullAnimator.Play("Attack", 0, 0);
        // Debug.Log("Performing attack!");
        yield return new WaitForSeconds(1);
        preparingAttack = false;
        newMaxVelocity = 15;
        rigidBody.maxLinearVelocity = 15;
        rigidBody.AddForce(skull.forward * 50 * Vector3.Distance(skull.position, playerTarget.position) * 5, ForceMode.Impulse);
        yield return new WaitForSeconds(1);
        attacking = false;
    }


    public void LerpMaxVelocity()
    {
        rigidBody.maxLinearVelocity = Mathf.Lerp(rigidBody.maxLinearVelocity, newMaxVelocity, Time.deltaTime * maxVelocityLerpSpeed);
    }

    public float FloorDistance()
    {
        Ray raycast = new Ray();
        raycast.origin = skull.position;
        raycast.direction = Vector3.down;
        RaycastHit hit;

        if (Physics.Raycast(raycast, out hit, 5, obstacleLayer))
        {
            return Vector3.Distance(skull.position, hit.point);
        }
        else
        {
            return 6;
        }
    }

    public void CheckForOverhang()
    {
        Ray raycast = new Ray();
        raycast.origin = playerTarget.transform.position;
        raycast.direction = Vector3.up;
        RaycastHit hit;

        if (Physics.Raycast(raycast, out hit, 15, obstacleLayer))
        {
            if (Vector3.Distance(hit.point, playerTarget.transform.position) > doorframeDist)
            {
                if (duckPositions.Count <= 0)
                {
                    duckPositions.Add(playerTarget.transform.position);
                    // if (showDuckObjects) Debug.Log(hit.transform.gameObject.name);
                }
                else
                {
                    if (Vector3.Distance(duckPositions[duckPositions.Count - 1], playerTarget.transform.position) > 0.5f)
                    {
                        duckPositions.Add(playerTarget.transform.position);
                        // if (showDuckObjects) Debug.Log(hit.transform.gameObject.name);
                    }
                }
            }
            else if (!PlayerNearby() && !PlayerInLineOfSight() && followingPlayer) // TRYING TO MAKE IT ADD POSITIONS WHEN THE PLAYER IS NOT SEEN
            {
                if (duckPositions.Count <= 0)
                {
                    duckPositions.Add(playerTarget.transform.position);
                    // if (showDuckObjects) Debug.Log(hit.transform.gameObject.name);
                }
                else
                {
                    if (Vector3.Distance(duckPositions[duckPositions.Count - 1], playerTarget.transform.position) > 3)
                    {
                        duckPositions.Add(playerTarget.transform.position);
                        // if (showDuckObjects) Debug.Log(hit.transform.gameObject.name);
                    }
                }
            }
        }

        if (duckPositions.Count > 0 && !attacking)
        {
            Ray raycast2 = new Ray();
            raycast2.origin = skull.position;
            raycast2.direction = playerTarget.position - skull.position;
            RaycastHit hit2;


            if (Physics.Raycast(raycast2, out hit2, 25, ~0, QueryTriggerInteraction.Collide))
            {
                //Debug.Log(hit2.transform.gameObject.name + " and also hit!");

                if (hit2.transform.gameObject.name == "PlayerController")
                {
                    if (Vector3.Angle(skull.forward, playerTarget.position - skull.position) > 90) // they're basically behind the player, no need to go through a door when the player has come back through
                    {
                        duckPositions.Clear();
                    }
                }
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        if (!healthBar || !whiteHealthBar || Invincible()) return; // in case no thing exists


        if (behaviorActive)
        {
            damage /= 3;
            int random = Random.Range(0, 2);
            if (random == 0)
            {
                skull.Find("WaterKnockback").GetComponent<Animation>().Play();
                KnockPlayerBack(Random.Range(35, 45));
            }
        }
        else if (!PlayerController.instance.MovementMachine.isGrounded)
        {
            //Stunned()
        }

        if (PlayerController.instance.ScriptSteal.BehaviorActive() && PlayerController.instance.ScriptSteal.GetHeldBehavior() == heldBehavior.weakness) damage *= 2f;

        if (!PlayerController.instance.ScriptSteal.BehaviorActive())
        {
            PlayerController.instance.Mana.GainMana(manaPerHit);
        }

        health -= damage;

        StopCoroutine("MaterialFade");
        StartCoroutine("MaterialFade");

        UpdateHealth();

        if (health <= maxHealth / 4 && !selfDestructing) EnterSelfDestructMode();

        if (health <= 0)
        {
            Die();
        }
    }

    void Stunned()
    {
        //stun enemy for x seconds while the player attacks it mid-air
    }

    public void KnockPlayerBack(float force)
    {
        Vector3 dir = (playerTarget.transform.position - transform.position).normalized;
        dir.y = 0.2f;
        dir *= force;
        PlayerController.instance.ForceHandler.AddForce(dir, ForceMode.VelocityChange);
    }

    public override void Die()
    {
        PlayerController.instance.Mana.GainMana(manaOnDeath);
        Destroy(skull.parent.gameObject);
    }

    public void SpacialAwareness()
    {
        RaycastHit hit;
        if (Physics.SphereCast(skull.position, 1.5f, skull.forward, out hit, 0, obstacleLayer))
        {
            rigidBody.AddForce((hit.point - skull.position).normalized * -15, ForceMode.Impulse);
        }
    }


    public bool PlayerNearby()
    {
        if (Vector3.Distance(playerTarget.position, skull.position) <= blindFollowRange) // within the "listen range"
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool PlayerInLineOfSight()
    {
        if (Vector3.Distance(playerTarget.position, skull.position) <= visionConeLength) // at least within sight range
        {
            // Debug.Log("Player is within cone length.");
            if (Physics.Raycast(skull.position, playerTarget.position - skull.position, out RaycastHit rayHit, (Vector3.Distance(playerTarget.position, skull.position) - 3), obstacleLayer)) // checks if player behind things?
            {
                // Debug.Log("Something is in the way.");
                return false;
            }
            else
            {
                Vector3 directionToTarget = (playerTarget.position - skull.position).normalized;
                if (Vector3.Angle(skull.forward, directionToTarget) < visionConeWidth / 2)
                {
                    // Debug.Log("IN CONE!");
                    return true;
                }
                else
                {
                    // Debug.Log("NOT IN CONE!");
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
        if (Vector3.Distance(playerTarget.position, skull.position) < attackRange)
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
        StartRolling();
    }

    public bool IsGrounded()
    {
        if (Physics.CheckSphere(new Vector3(waterOrb.transform.position.x, waterOrb.transform.position.y - 0.9f, waterOrb.transform.position.z), 0.3f, obstacleLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void DeactivateBehavior() // fly mode
    {
        base.DeactivateBehavior();
        StartFlying();
    }

    public override bool Invincible()
    {
        if (Physics.CheckSphere(skull.position, 1.5f, gateLayer, QueryTriggerInteraction.Collide)) return true;
        else return false;
    }
}
