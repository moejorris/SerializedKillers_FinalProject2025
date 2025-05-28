using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI_Base : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Transform target;
    [SerializeField] private float attackDistance;
    private NavMeshAgent navMeshAgent;
    //private Animator animator;
    [SerializeField] private float targetDistance;

    [Header("Held Behavior")]
    public Behavior heldBehavior;
    public bool behaviorActive = true;
    private bool delayedExit = false;
    [SerializeField] private ScriptStealMenu scriptStealMenu;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
        }
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        targetDistance = Vector3.Distance(navMeshAgent.transform.position, target.position);
        if (targetDistance < attackDistance)
        {
            navMeshAgent.isStopped = true;
            //animator.SetBool("Attack", true);
        }
        else
        {
            navMeshAgent.isStopped = false;
            //animator.SetBool("Attack", false);
            navMeshAgent.destination = target.position;
        }

        if (delayedExit && !scriptStealMenu.menuOpen)
        {
            delayedExit = false;
            transform.Find("Capsule").gameObject.layer = 0;
            scriptStealMenu.selectedEnemy = null;
            scriptStealMenu.centerSlot.RemoveBehavior();
        }
    }


    public void SelectEnemy()
    {
        transform.Find("Capsule").gameObject.layer = 6;
        scriptStealMenu.selectedEnemy = this;
        scriptStealMenu.centerSlot.AddBehavior(heldBehavior);
    }

    public void DeselectEnemy()
    {
        scriptStealMenu.selectedEnemy = null;
        scriptStealMenu.centerSlot.RemoveBehavior();
        transform.Find("Capsule").gameObject.layer = 0;
    }


    //private void OnMouseEnter()
    //{
    //    if (scriptStealMenu.selectedEnemy == null && behaviorActive)
    //    {
    //        transform.Find("Capsule").gameObject.layer = 6;
    //        scriptStealMenu.selectedEnemy = this;
    //        scriptStealMenu.centerSlot.AddBehavior(heldBehavior);
    //    }
    //    delayedExit = false;
    //}

    //private void OnMouseExit()
    //{
    //    if (scriptStealMenu.selectedEnemy == this && scriptStealMenu.menuOpen)
    //    {
    //        delayedExit = true;
    //    }
    //    else
    //    {
    //        if (scriptStealMenu.selectedEnemy == this)
    //        {
    //            scriptStealMenu.selectedEnemy = null;
    //            scriptStealMenu.centerSlot.RemoveBehavior();
    //        }
    //        transform.Find("Capsule").gameObject.layer = 0;
    //    }
    //}
}
