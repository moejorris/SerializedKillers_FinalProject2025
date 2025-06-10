using UnityEngine;

public class EnemyAI_SobbySkull : EnemyAI_Base
{
    [Header("Sobby Skull General")]
    [SerializeField] private string movementState = "idle";
    [SerializeField] private GameObject waterOrb;

    [Header("Sobby Skull Vision")]
    [SerializeField] private float idleAlertRange;
    [SerializeField] private float blindFollowRange;
    [SerializeField] private float visionConeLength;
    [SerializeField] private float visionConeWidth;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        
    }

    // Update is called once per frame
    void Update()
    {
        navMeshAgent.destination = playerTarget.position;
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
