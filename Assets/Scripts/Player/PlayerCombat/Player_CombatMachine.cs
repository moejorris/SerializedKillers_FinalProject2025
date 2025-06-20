using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

//Joe Morris
//SGD FP '25

public class Player_CombatMachine : MonoBehaviour
{
    ScriptStealMenu scriptStealMenu => GameObject.FindGameObjectWithTag("Canvas").transform.Find("ScriptStealMenu").GetComponent<ScriptStealMenu>();
    Player_MovementMachine _machine => GetComponent<Player_MovementMachine>();
    Player_RootMotion _rootMotion => GetComponent<Player_RootMotion>();
    Player_ForceHandler _forceHandler => GetComponent<Player_ForceHandler>();
    Player_Rotate _rotation => GetComponent<Player_Rotate>();
    [SerializeField] AnimatorOverrideController animatorOverride;
    int currentAnim = 1;

    [SerializeField] Animator animator;
    [SerializeField] List<PlayerAttackSO> defaultAttacks = new List<PlayerAttackSO>();

    [SerializeField] float comboResetTime = 0.3f;
    [SerializeField] int currentComboID = 0;

    [SerializeField] InputActionReference attackInput;


    [Header("Damage Collision")]
    [SerializeField] bool showHitboxCollider;
    [SerializeField] float distanceForwards = 1f;
    [SerializeField] float checkRadius = 1f;

    bool isAttacking = false;
    bool attackQueued = false;

    [Header("Cosmetic")]
    [SerializeField] TrailRenderer trailRenderer;

    void Update()
    {
        if (attackInput.action.WasPressedThisFrame())
        {
            if (isAttacking)
            {
                attackQueued = true;
            }
            else
            {
                Attack();
            }
        }
        else if (!isAttacking && attackQueued)
        {
            Attack();
        }
    }

    void Attack()
    {
        if (OutOfRangeCheck()) return;

        isAttacking = true;
        attackQueued = false;

        PlayerAttackSO attack = defaultAttacks[currentComboID];

        //TODO:
        //Arkham Method:
        //Check to find possible victims
        //iterate through them to find most likely intended to be attacked by player by getting joystick dir relative to cam and closest of them
        //snap player to look at that enemy
        //if movement force is greater than the distance from player to chosen enemy, limit force so the player doesn't move if not necessary.
        //Consider just using the distance as the move force so the player gets close enough on it's own???

        //TODO:
        //Also standard targeting method
        //Get target (somehow)
        //Rotate player towards it (not immediate snap)
        //Make cam look at it... look at avg position of player and target (player.position + target.position / 2f)

        animatorOverride["AttackPlaceholder" + currentAnim] = attack.animation;
        animator.runtimeAnimatorController = animatorOverride;

        animator.CrossFade("Attack" + currentAnim, 0.025f);
        animator.speed = attack.animationSpeed;
        float animationLength = attack.animation.length / attack.animationSpeed; // find a way to get the speed of the state and divide the length by that...

        if (trailRenderer)
        {
            trailRenderer.emitting = true;

            CancelInvoke("DisableTrail");
            Invoke("DisableTrail", animationLength * 0.5f);
        }


        CancelInvoke("AttackIsDone");
        Invoke("AttackIsDone", animationLength * 0.75f);

        HitCheck(); //add delay to this because right now it's as soon as you press the button

        if (attack.overrideMotion)
        {
            if (attack.usesRootMotion) _machine.DisableAllMovers(_rootMotion);
            else _rootMotion.enabled = false;
            _machine.DisableAllMovers(_forceHandler);
            _rotation.enabled = false;
        }
        if (attack.vectorForce.magnitude > 0) // only apply force if necessary
        {
            _forceHandler.AddForce(LocalizeForceVector(attack.vectorForce), ForceMode.VelocityChange, Player_ForceHandler.OverrideMode.All);        
        }

        CancelInvoke("ResetCombo");
        Invoke("ResetCombo", comboResetTime + animationLength);

        GetNextAttack();
    }

    void DisableTrail()
    {
        trailRenderer.emitting = false;
    }

    void AttackIsDone()
    {
        isAttacking = false;
        animator.speed = 1;

        _machine.EnableAllMovers();
        _rootMotion.enabled = false;
        _rotation.enabled = true;

        if (trailRenderer) trailRenderer.emitting = false;
    }

    void GetNextAttack()
    {
        if (currentComboID < defaultAttacks.Count - 1)
        {
            currentComboID++;
        }
        else ResetCombo();

        //toggle between using anim states 1 and 2 for transitionary purposes as opposed to using one anim state for attacks
        currentAnim = currentAnim == 1 ? 2 : 1;
    }

    void ResetCombo()
    {
        currentComboID = 0;
        currentAnim = 1;
    }

    void HitCheck() // Caleb was here O_O //This needs a major refactor
    {
        Collider[] hitObjects = Physics.OverlapSphere(transform.position + _machine.ForwardDirection * distanceForwards, checkRadius, ~0, QueryTriggerInteraction.Ignore);
        Vector3 hitPositions = new Vector3();
        float validEnemies = 0;

        //TODO Create List of Enemy movement controllers

        EnemyAI_Base selectedEnemy = null;

        foreach (Collider collider in hitObjects)
        {
            if (collider.CompareTag("Player") || !collider.gameObject.GetComponent<EnemyAI_Base>()) continue;

            validEnemies++;
            hitPositions += collider.transform.position;
            collider.gameObject.GetComponent<EnemyAI_Base>().TakeDamage(defaultAttacks[currentComboID].damage); //TODO apply script effect to enemy when damaging
            Debug.Log(collider.gameObject.name + " took " + defaultAttacks[currentComboID].damage + " damage!");
            selectedEnemy = collider.gameObject.GetComponent<EnemyAI_Base>();

            //TODO add enemy to List of movement controllers
        }

        //TODO foreach enemy movement controller, apply knockback force to them. Possibly divide knockback by list count???

        if (selectedEnemy) scriptStealMenu.UpdateSelectedEnemy(selectedEnemy);

        if (Vector3.Distance(transform.position + _machine.ForwardDirection, hitPositions / validEnemies) <= checkRadius*2f && defaultAttacks[currentComboID].overrideMotion) //this didn't work lol TODO fix this
        {
            Debug.Log("Enemy is close");
            //_forceHandler.AddForce(Vector3.zero, ForceMode.VelocityChange, Player_ForceHandler.OverrideMode.All);
            _forceHandler.ResetVelocity();
        }

        // VVV commented-out copy of the function prior to me editing in case I somehow destroy this function VVV
        if (1 > 5000)
        {
            //Collider[] hitObjects = Physics.OverlapSphere(transform.position + _machine.ForwardDirection * distanceForwards, checkRadius, ~0, QueryTriggerInteraction.Ignore);
            //Vector3 hitPositions = new Vector3();
            //float validEnemies = 0;
            //foreach (Collider collider in hitObjects)
            //{
            //    if (collider.CompareTag("Player") || !collider.gameObject.GetComponent<EnemyAI_Base>()) continue;

            //    validEnemies++;
            //    hitPositions += collider.transform.position;
            //    collider.gameObject.GetComponent<EnemyAI_Base>().TakeDamage(defaultAttacks[currentComboID].damage);
            //    Debug.Log(collider.gameObject.name + " took " + defaultAttacks[currentComboID].damage + " damage!");
            //}



            //if (Vector3.Distance(transform.position + _machine.ForwardDirection, hitPositions / validEnemies) <= checkRadius * 2f && defaultAttacks[currentComboID].overrideMotion)
            //{
            //    Debug.Log("Enemy is close");
            //    //_forceHandler.AddForce(Vector3.zero, ForceMode.VelocityChange, Player_ForceHandler.OverrideMode.All);
            //    _forceHandler.ResetVelocity();
            //}
        } 
    }

    void OnDrawGizmos()
    {
        if (isAttacking || showHitboxCollider)
        {
            Vector3 dir = _machine.ForwardDirection == Vector3.zero ? transform.forward : _machine.ForwardDirection;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + dir * distanceForwards, checkRadius);
        }
    }

    bool OutOfRangeCheck() //this might be stupid to keep in
    {
        if (currentComboID < 0) return true;
        if (currentComboID >= defaultAttacks.Count) return true;

        return false;
    }

    Vector3 LocalizeForceVector(Vector3 vector)
    {
        return _machine.ForwardDirection * vector.z + _machine.RightDirection * vector.x + Vector3.up * vector.y;
    }
}
