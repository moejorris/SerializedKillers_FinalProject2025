using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Splines;
using System.Collections;

public class EnemyAI_SobbySkull : EnemyAI_Base
{
    [Header("Sobby Skull General")]
    public bool showDuckObjects = false;
    [SerializeField] private string movementState = "idle";
    [SerializeField] private GameObject waterOrb => skull.Find("WaterOrb").gameObject;
    [SerializeField] private Transform skull => transform;
    [SerializeField] private NavMeshAgent rollingNavMeshAgent;

    [SerializeField] private float followTimer = 15;
    [SerializeField] private bool followingPlayer = false;

    [SerializeField] private bool ducking = false;
    //[SerializeField] private Vector3 duckPosition = Vector3.zero;
    [SerializeField] private float doorframeDist = 5;
    [SerializeField] private float flyingSpeed = 5;
    [SerializeField] private float flyingTurnSpeed = 5;
    [SerializeField] private float flyHeight = 4;
    [SerializeField] private List<Vector3> duckPositions = new List<Vector3>();

    [Header("Sobby Skull Movement")]
    [SerializeField] private float gravityLevel = 1;
    [SerializeField] private float rollSpeed = 1;
    [SerializeField] private float newMaxVelocity;
    [SerializeField] private float maxVelocityLerpSpeed = 5;

    [Header("Sobby Skull Combat")]
    [SerializeField] private bool attacking = false;
    [SerializeField] private bool preparingAttack = false;
    [SerializeField] Vector3 attackDir = Vector3.zero;
    [SerializeField] bool hitPlayer = false;
    [SerializeField] private float attackCooldown = 5;
    private float attackTimer = 0;
    [SerializeField] private PlayerHealth playerHealth => GameObject.FindGameObjectWithTag("Canvas").GetComponent<PlayerHealth>();

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
        healthBar = skull.parent.Find("Canvas/Bar").GetComponent<RectTransform>();
        whiteHealthBar = skull.parent.Find("Canvas/Bar/White").GetComponent<RectTransform>();
        selectedIcon = skull.parent.Find("Canvas/SelectedIcon").GetComponent<Image>();

        newMaxVelocity = rigidBody.maxLinearVelocity;
        maxVelocityLerpSpeed = 15;

        StartRolling();
        StartCoroutine("DoorCheckTimer");

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(skull.position.x, skull.position.y - 0.8f, skull.position.z), 0.2f);
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

        if (attacking)
        {
            if (!preparingAttack)
            {
                if (Vector3.Distance(skull.position, playerTarget.position) <= 1.5f)
                {
                    MeleeHitCheck();
                }

                attackDir.y += Time.deltaTime * 20;

                skull.rotation = Quaternion.Slerp(skull.rotation, Quaternion.LookRotation(attackDir, Vector3.up), Time.deltaTime * flyingTurnSpeed * 1.5f); // always flies to the player
            }
            else
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

                if (Vector3.Distance(playerTarget.position, skull.position) < 7f && attackTimer <= 0 && PlayerInLineOfSight())
                {
                    BeginDiveAttack();
                }
            }
            else if (movementState == "rolling")
            {
                
            }
        }

        LerpMaxVelocity();
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

                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TARGET - transform.position, Vector3.up), Time.deltaTime * flyingTurnSpeed); // always flies to the player

                rigidBody.AddForce(skull.forward * flyingSpeed, ForceMode.VelocityChange);
            }
            else
            {
                rigidBody.AddForce(Physics.gravity * gravityLevel, ForceMode.Acceleration);

                if (IsGrounded())
                {
                    newMaxVelocity = 3;
                    rigidBody.maxLinearVelocity = 3;
                    rigidBody.AddForce((playerTarget.position - skull.position).normalized * rollSpeed, ForceMode.VelocityChange);
                }
                else
                {
                    newMaxVelocity = 15;
                    rigidBody.maxLinearVelocity = 15;
                }
            }
        }
        else
        {
            if (movementState == "flying")
            {
                rigidBody.AddForce(skull.forward * flyingSpeed, ForceMode.VelocityChange);
            }
        }
    }

    public void MeleeHitCheck()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1, transform.forward, 1, playerLayer);

        //Debug.Log(hits.Length);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.parent != null && hit.transform.parent.CompareTag("Player") && !hitPlayer)
            {
                Debug.Log("Player Hit!");
                hitPlayer = true;
                playerHealth.TakeDamage(5);
                break;
            }
        }
    }

    IEnumerator DoorCheckTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            CheckForOverhang();
            SpacialAwareness();
        }
    }

    public void StartRolling()
    {
        attacking = false;
        waterOrb.SetActive(true);
        rigidBody.useGravity = true;
        movementState = "rolling";
    }

    public void StartFlying()
    {
        attackTimer = 2;
        waterOrb.SetActive(false);
        rigidBody.useGravity = false;
        movementState = "flying";
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
        Debug.Log("Performing attack!");
        yield return new WaitForSeconds(1);
        preparingAttack = false;
        newMaxVelocity = 15;
        rigidBody.maxLinearVelocity = 15;
        rigidBody.AddForce(skull.forward * 50 * Vector3.Distance(skull.position, playerTarget.position) * 3, ForceMode.Impulse);
        yield return new WaitForSeconds(1);
        attacking = false;
    }

    public void LerpMaxVelocity()
    {
        rigidBody.maxLinearVelocity = Mathf.Lerp(rigidBody.maxLinearVelocity, newMaxVelocity, Time.deltaTime * maxVelocityLerpSpeed);
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
                    if (showDuckObjects) Debug.Log(hit.transform.gameObject.name);
                }
                else
                {
                    if (Vector3.Distance(duckPositions[duckPositions.Count-1], playerTarget.transform.position) > 0.5f)
                    {
                        duckPositions.Add(playerTarget.transform.position);
                        if (showDuckObjects) Debug.Log(hit.transform.gameObject.name);
                    }
                }
            }
            else if (!PlayerNearby() && !PlayerInLineOfSight() && followingPlayer) // TRYING TO MAKE IT ADD POSITIONS WHEN THE PLAYER IS NOT SEEN
            {
                if (duckPositions.Count <= 0)
                {
                    duckPositions.Add(playerTarget.transform.position);
                    if (showDuckObjects) Debug.Log(hit.transform.gameObject.name);
                }
                else
                {
                    if (Vector3.Distance(duckPositions[duckPositions.Count - 1], playerTarget.transform.position) > 3)
                    {
                        duckPositions.Add(playerTarget.transform.position);
                        if (showDuckObjects) Debug.Log(hit.transform.gameObject.name);
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
                    if (Vector3.Angle(skull.forward, playerTarget.position - skull.position) > 70) // they're basically behind the player, no need to go through a door when the player has come back through
                    {
                        duckPositions.Clear();
                    }
                }
            }
        }
    }

    public override void TakeDamage(float damage, Player_ScriptSteal scriptSteal)
    {
        if (!healthBar || !whiteHealthBar) return; // in case no thing exists

        health -= damage;

        //StopCoroutine("MaterialFade");
        //StartCoroutine("MaterialFade");

        UpdateHealth();
        if (health <= 0)
        {
            Destroy(transform.parent.gameObject);
        }
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
            Debug.Log("Player is within cone length.");
            if (Physics.Raycast(skull.position, playerTarget.position - skull.position, out RaycastHit rayHit, (Vector3.Distance(playerTarget.position, skull.position) - 3), obstacleLayer)) // checks if player behind things?
            {
                Debug.Log("Something is in the way.");
                return false;
            }
            else
            {
                Vector3 directionToTarget = (playerTarget.position - skull.position).normalized;
                if (Vector3.Angle(skull.forward, directionToTarget) < visionConeWidth / 2)
                {
                    Debug.Log("IN CONE!");
                    return true;
                }
                else
                {
                    Debug.Log("NOT IN CONE!");
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
        if (Physics.CheckSphere(new Vector3(skull.position.x, skull.position.y - 0.8f, skull.position.z), 0.2f, obstacleLayer))
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
}
