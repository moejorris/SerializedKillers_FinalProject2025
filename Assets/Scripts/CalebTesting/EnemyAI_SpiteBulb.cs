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

    [Header("Bulb Wandering")]
    [SerializeField] private float wanderDistance = 5;
    [SerializeField] private float randomWanderLocations = 3;
    private bool wandering = false;
    private bool searchAnimationOccuring = false;
    private Vector3 wanderHome;
    private Quaternion wanderLookDirection;

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


    [Header("Lighting")]
    [SerializeField] private Animator lightAnimator;

    public bool testBool = false;
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        bulbHead = transform.Find("Head").gameObject;
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
            }

            Vector3 direction = transform.forward;
            Quaternion rotation = Quaternion.Lerp(bulbHead.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5);
            bulbHead.transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
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
                    //Vector3 direction = wanderHome - bulbHead.transform.position;
                    Quaternion rotation = Quaternion.Lerp(bulbHead.transform.rotation, wanderLookDirection, Time.deltaTime * 5);
                    bulbHead.transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
                }
            }
        }
        else if (movementState == "pursue")
        {
            if (PlayerInFollowRange() || attackState != "neutral") // if the enemy is attacking, for instance, it should still target player.
            {
                navMeshAgent.destination = playerTarget.position;
            }
            else
            {
                movementState = "wandering";
            }

            if (PlayerInAttackRange())
            {
                if (attackState == "neutral" && canAttack)
                {
                    canAttack = false;
                    //PerformAttack();
                }
            }

            Vector3 direction = playerTarget.position - bulbHead.transform.position;
            Quaternion rotation = Quaternion.Lerp(bulbHead.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 15);
            bulbHead.transform.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
        }
    }

    public void PerformAttack()
    {
        if (AttackRange() == "melee")
        {
            attackState = "melee";
            StartCoroutine(MeleeAttack());
        }
        else if (AttackRange() == "shockwave")
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
        yield return new WaitForSeconds(10);
        Debug.Log("No longer turning head...");
        yield return new WaitForSeconds(2);
        Debug.Log("LASER FIRED BOOOOOM");
        yield return new WaitForSeconds(1);

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
        Debug.Log("Wander time started from beginning!");

        yield return new WaitForSeconds(2); // wait to make sure they get to the last known position
        wanderHome = navMeshAgent.destination;

        for (int i = 0; i < randomWanderLocations; i++)
        {
            navMeshAgent.destination = new Vector3(wanderHome.x + Random.Range(-wanderDistance, wanderDistance), wanderHome.y, wanderHome.z + Random.Range(-wanderDistance, wanderDistance)); // picks spot to walk to
            //Debug.Log("New destination is: " + navMeshAgent.destination);
            yield return new WaitUntil(() => navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance + 0.1f); // wait until they get close

            float yDir = Random.Range(0, 360);
            float yChange = Random.Range(45, 90);
            for (int _i = 0; _i < 3; _i++) // does head turn thingy 3 times :)
            {
                yChange += Random.Range(10, 15);
                yChange *= -1;
                wanderLookDirection = Quaternion.Euler(0, yDir + yChange, 0);
                yield return new WaitForSeconds(1);
            }

            //BeginSpotSearch(); // head look animation
            //yield return new WaitForSeconds(0.3f); // buffer in case needed?
            //yield return new WaitUntil(() => !searchAnimationOccuring); // waits until animation is complete
        }

        yield return new WaitForSeconds(1); // little buffer at the end

        Debug.Log("Wander time finished");

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

}
