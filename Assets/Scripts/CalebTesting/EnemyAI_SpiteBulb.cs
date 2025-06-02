using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines.Interpolators;


[RequireComponent(typeof(Light))]
public class EnemyAI_SpiteBulb : EnemyAI_Base
{
    [Header("Bulb Specific")]
    public float idleActivateRange;
    [SerializeField] private string movementState = "idle";
    private bool healing = false;

    [Header("Bulb Wandering")]
    [SerializeField] private float wanderDistance = 5;
    [SerializeField] private float randomWanderLocations = 3;
    [SerializeField] private float wanderTime = 15;
    private bool wandering = false;
    private bool searching = false;

    [Header("Bulb Combat")]
    [SerializeField] private string attackState = "melee";
    [SerializeField] private float meleeAttackCooldown = 5;
    [SerializeField] private float shockwaveAttackCooldown = 7;
    [SerializeField] private float laserAttackCooldown = 9;

    [Header("Lighting")]

    [SerializeField] private Animator lightAnimator;
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
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
                movementState = "pursue";
            }
        }
        else if (movementState == "wandering")
        {
            if (PlayerInFollowRange())
            {
                movementState = "pursue";
                StopCoroutine(WanderTimer());
                searching = false; // may need to be removed? Also, animation could impact this bool.
                wandering = false;
            }
            else
            {
                if (!wandering)
                {
                    wandering = true;
                    StartCoroutine(WanderTimer());
                }
            }
        }
        else if (movementState == "pursue")
        {
            if (PlayerInFollowRange())
            {
                navMeshAgent.destination = playerTarget.position;
            }
            else
            {
                movementState = "wandering";
            }

            if (PlayerInAttackRange())
            {

            }



        }
    }

    IEnumerator WanderTimer()
    {
        Debug.Log("wander timer started");
        yield return new WaitForSeconds(2); // wait to make sure they get to the last known position
        Vector3 home = navMeshAgent.destination;

        for (int i = 0; i < randomWanderLocations; i++)
        {
            navMeshAgent.destination = new Vector3(home.x + Random.Range(-wanderDistance, wanderDistance), home.y, home.z + Random.Range(-wanderDistance, wanderDistance)); // picks spot to walk to
            Debug.Log("New destination is: " + navMeshAgent.destination);
            yield return new WaitUntil(() => navMeshAgent.remainingDistance < 0.2f); // wait until they get close
            BeginSpotSearch(); // head look animation
            //yield return new WaitForSeconds(0.3f); // buffer in case needed?
            yield return new WaitUntil(() => !searching); // waits until animation is complete
        }

        yield return new WaitForSeconds(1); // little buffer at the end

        movementState = "idle"; // returns to idle
    }

    public void BeginSpotSearch()
    {
        searching = true;
        Invoke("FinishSpotSearch", 3); // TEMPPPP!!
    }

    public void FinishSpotSearch()
    {
        searching = false;
    }

    IEnumerator HealTimer()
    {
        while (healing)
        {
            yield return new WaitForSeconds(2);
            Debug.Log("Player healing");
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

}
