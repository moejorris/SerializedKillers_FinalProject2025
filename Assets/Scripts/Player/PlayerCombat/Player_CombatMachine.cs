using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


//Joe Morris
//SGD FP '25

public class Player_CombatMachine : MonoBehaviour
{

    //References
    [SerializeField] AnimatorOverrideController animatorOverride;
    [SerializeField] Animator animator;

    [Header("Combat Parameters")]
    [SerializeField] List<PlayerAttackSO> defaultAttacks = new List<PlayerAttackSO>();
    [SerializeField] float comboResetTime = 0.3f;
    [SerializeField] InputActionReference attackInput;


    [Header("Damage Collision")]
    [SerializeField] bool showHitboxCollider;
    [SerializeField] bool onlyShowHitboxWhenEnabled = true;
    bool hitBoxActive;
    [SerializeField] float distanceForwards = 1f;
    [SerializeField] float checkRadius = 1f;

    [Header("Batman Arkham Combat Settings")]
    [SerializeField] bool isBatman = true;
    [SerializeField] InputActionReference walkInput;
    [SerializeField] float maxLeapDistance = 8f;
    [SerializeField] float minDashDistance = 4f;
    [SerializeField] float minDotProduct = 0.75f;
    float currentAerialAttack = 0;

    //Local Stuff
    bool isAttacking = false;
    bool attackQueued = false;
    int currentAnim = 1;
    PlayerAttackSO currentAttack;
    int currentComboID = 0;

    [Header("Combo System Parameters")]
    [SerializeField] int currentComboCount = 0;
    [SerializeField] float comboCountResetTime = 0.5f;
    [SerializeField] bool comboResetOnWhiff = true;
    [SerializeField] float comboManaBonusMultiplier = 0.25f;

    void Update()
    {
        if (attackInput.action.WasPressedThisFrame())
        {
            if (PlayerController.instance.MovementMachine.isGrounded)
            {
                currentAerialAttack = 0;    
            }

            if (isAttacking)
            {
                attackQueued = true;
            }
            else
            {
                StartCoroutine(Attack());
            }
        }
        else if (!isAttacking && attackQueued)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        if (OutOfRangeCheck()) yield break;

        //TODO:
        //Also standard targeting method
        //Get target (somehow)
        //Rotate player towards it (not immediate snap)
        //Make cam look at it... look at avg position of player and target (player.position + target.position / 2f)

        if (PlayerController.instance.ScriptSteal.GetHeldBehavior() != null && PlayerController.instance.ScriptSteal.GetHeldBehavior().behaviorName == "water" && PlayerController.instance.ScriptSteal.BehaviorActive()) // PUTS OUT FIRE ON PLAYER IF THEY ATTACK WITH WATER SCRIPT!
        {
            PlayerController.instance.ScriptSteal.ApplyStatusEffect(PlayerController.instance.ScriptSteal.heldBehavior);
        }

        isAttacking = true;
        attackQueued = false;

        currentAttack = defaultAttacks[currentComboID];

        HandleAnimation();


        float attackTime = currentAttack.animation.length / currentAttack.animationSpeed; // find a way to get the speed of the state and divide the length by that...

        HandleMovement();
        HandleRotation();

        if (HandleMoveToTarget())
        {
            animator.speed = 0;
            yield return new WaitForSeconds(PlayerController.instance.Dash._travelTime);
            animator.speed = currentAttack.animationSpeed;
        }
        StartCoroutine(HitCheck());


        //Wait for dash to finish/player is close to enemy to start attack


        HandleSound();
        HandleParticle();

        CancelInvoke("AttackIsDone");
        Invoke("AttackIsDone", attackTime * 0.75f);


        CancelInvoke("ResetCombo");
        Invoke("ResetCombo", comboResetTime + attackTime);

        GetNextAttack();
    }

    bool HandleMoveToTarget()
    {
        if (!isBatman) return false;
        // if (!PlayerController.instance.MovementMachine.isGrounded) return false;

        Vector3 inputDir = IntendedMoveDirection();

        if (inputDir.magnitude < 0.5f) inputDir = PlayerController.instance.MovementMachine.ForwardDirection;

        Collider[] checkSphereColliders = Physics.OverlapSphere(transform.position + inputDir * (maxLeapDistance / 2f), maxLeapDistance / 2f, ~0, QueryTriggerInteraction.Ignore);

        List<ITargetable> potentialTargets = new();

        foreach (Collider collider in checkSphereColliders)
        {
            ITargetable target = collider.GetComponent<ITargetable>();

            if (target == null) continue;

            float dist = Vector3.Distance(transform.position, target.transform.position);

            if (dist > maxLeapDistance) continue;

            if (dist > checkRadius*2f && walkInput.action.ReadValue<Vector2>().magnitude == 0) continue;

            if (target.transform.position.y > transform.position.y && collider.GetComponent<EnemyAI_SobbySkull>() != null && PlayerController.instance.MovementMachine.isGrounded)
            {
                continue;
            }

            RaycastHit hit;

            if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, dist + 2, ~0, QueryTriggerInteraction.Ignore) && hit.collider != collider) continue;

            potentialTargets.Add(target);
        }

        // Debug.Log("Arkham Combat: Found " + potentialTargets.Count + " potential targets.");

        if (potentialTargets.Count < 1) return false;

        ITargetable selectedTarget = null;

        if (potentialTargets.Count >= 2)
        {
            foreach (ITargetable target in potentialTargets)
            {
                target.TargetScore = 0;

                Vector3 yLessTargetPosition = target.transform.position;
                yLessTargetPosition.y = transform.position.y;

                float testDot = Vector3.Dot((yLessTargetPosition - transform.position).normalized, inputDir);
                // Debug.Log("Arkham Combat: Target Dot Product = " + testDot);
                if (testDot < minDotProduct) continue;

                target.TargetScore += testDot;
                target.TargetScore += 1 - (Vector3.Distance(transform.position, target.transform.position) / maxLeapDistance);
            }

            float highScore = 0;
            foreach (ITargetable target in potentialTargets) //don't need to iterate over the same list twice, I was tired when I wrote this....
            {
                if (target.TargetScore * target.TargetScoreWeight > highScore)
                {
                    highScore = target.TargetScore * target.TargetScoreWeight;
                    selectedTarget = target;
                }
            }
        }
        else selectedTarget = potentialTargets[0];

        if (selectedTarget == null) return false;

        PlayerController.instance.Rotate.enabled = false;

        // Debug.Log("Arkham Combat: Found Most In Line Target: " + selectedTarget.transform.name);

        ///DEBUG LINES
        // foreach (ITargetable target in potentialTargets)
        // {
        //     Color thisColor;
        //     if (target.TargetScore == highScore) thisColor = Color.green;
        //     else if (target.TargetScore >= highScore / 2f) thisColor = Color.blue;
        //     else thisColor = Color.red;

        //     Debug.DrawLine(transform.position, target.transform.position, thisColor, 10f);

        // }
        ///END DEBUG LINES

        float targetDistance = Vector3.Distance(selectedTarget.transform.position, transform.position);

        if (targetDistance > minDashDistance)
        {
            PlayerController.instance.MovementMachine.DisableAllMovers(PlayerController.instance.Dash);
            PlayerController.instance.Dash.ExternalDash(selectedTarget.transform.position, true);
            if (!PlayerController.instance.MovementMachine.isGrounded) currentAerialAttack = currentComboID;
            return true;
        }
        else
        {
            if (!PlayerController.instance.MovementMachine.isGrounded)
            {
                currentAerialAttack++;

                if (currentAerialAttack >= defaultAttacks.Count)
                {
                    return false;
                }
            }

            Vector3 newDir = selectedTarget.transform.position - transform.position;
            PlayerController.instance.MovementMachine.SetForwardDirection(newDir);
            PlayerController.instance.MovementMachine.DisableAllMovers(PlayerController.instance.ForceHandler);
            // PlayerController.instance.ForceHandler.AddForce(PlayerController.instance.MovementMachine.ForwardDirection * Vector3.Distance(transform.position, selectedTarget.transform.position - PlayerController.instance.MovementMachine.ForwardDirection * checkRadius) * 5f, ForceMode.VelocityChange, Player_ForceHandler.OverrideMode.All);
            PlayerController.instance.ForceHandler.AddForce(newDir, ForceMode.VelocityChange, Player_ForceHandler.OverrideMode.All);
            return false;
        }
    }

    void HandleParticle()
    {
        if (currentAttack.particleEffect)
        {
            GameObject particleGOBJ = Instantiate(currentAttack.particleEffect, animator.transform);
            ParticleSystem.MainModule particle = particleGOBJ.GetComponent<ParticleSystem>().main;

            particle.startColor = PlayerController.instance.ScriptSteal.scriptEffectColor;

            Destroy(particleGOBJ, 1f);
        }
    }

    void HandleRotation()
    {
        if (currentAttack.lockRotation)
        {
            PlayerController.instance.Rotate.enabled = false;
        }
    }

    void HandleMovement() //Old Root Motion/Manual Force Style of Movement
    {
        if (isBatman) return;

        if (currentAttack.overrideMotion)
        {
            if (currentAttack.usesRootMotion) PlayerController.instance.MovementMachine.DisableAllMovers(PlayerController.instance.RootMotion);
            else PlayerController.instance.RootMotion.enabled = false;
            PlayerController.instance.MovementMachine.DisableAllMovers(PlayerController.instance.ForceHandler);
        }

        if (currentAttack.vectorForce.magnitude > 0) // only apply force if necessary
        {
            PlayerController.instance.ForceHandler.AddForce(LocalizeForceVector(currentAttack.vectorForce), ForceMode.VelocityChange, Player_ForceHandler.OverrideMode.All);
        }
    }

    void HandleSound()
    {
        if (currentAttack.swingSound != null)
        {
            if (currentAttack.hitboxDelay > 0)
            {
                SoundManager.instance.PlaySoundEffectDelayed(currentAttack.swingSound, currentAttack.hitboxDelay);
            }
            else SoundManager.instance.PlaySoundEffect(currentAttack.swingSound);
        }
    }

    void HandleImpactSound()
    {
        if (currentAttack.impactSound != null)
        {
            SoundManager.instance.PlaySoundEffect(currentAttack.impactSound);
        }
    }

    void HandleAnimation()
    {
        animatorOverride["AttackPlaceholder" + currentAnim] = currentAttack.animation;
        animator.runtimeAnimatorController = animatorOverride;

        animator.CrossFade("Attack" + currentAnim, 0.025f);
        animator.speed = currentAttack.animationSpeed;
    }

    void AttackIsDone()
    {
        isAttacking = false;
        animator.speed = 1;

        PlayerController.instance.MovementMachine.EnableAllMovers();
        PlayerController.instance.RootMotion.enabled = false;
        PlayerController.instance.Rotate.enabled = true;

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

    void ColliderIterationBody(ref EnemyAI_Base selectedEnemy, ref List<IDamageable> damageableStorage, ref List<IElemental> elementalStorage, ref bool increaseCombo) //Refactoring all the repeat code in HitCheck(). All code referencing "elemental" or "elementals" was Caleb. Everything else was me (joe) :)
    {
        Collider[] hitObjects = Physics.OverlapSphere(transform.position + PlayerController.instance.MovementMachine.ForwardDirection * distanceForwards, checkRadius, ~0, QueryTriggerInteraction.Collide);

        bool hitSomething = false;
        foreach (Collider collider in hitObjects)
        {
            if (collider.CompareTag("Player")) continue; //this actually shouldn't fire because the player collider shouldn't be tagged due to caleb's jank requirement that only the root Player parent object is tagged as "Player" but leaving in as a fail-safe

            CheckDamageables(collider, ref damageableStorage, ref selectedEnemy, ref hitSomething);
            CheckElementals(collider, ref elementalStorage, ref hitSomething);

            if (collider.GetComponent<IComboTarget>() != null && !increaseCombo)
            {
                increaseCombo = true;
                UpdateComboCount();
            }
        }

        if (hitSomething)
        {
            if (PlayerController.instance.Mana.usageType == Player_Mana.UsageType.PerUse) PlayerController.instance.Mana.UseMana(); // uses mana after hit because internally the enemies will have checked if there was enough mana to take actual damage
            HandleImpactSound();
        }
    }

    void CheckDamageables(Collider collider, ref List<IDamageable> damageableStorage, ref EnemyAI_Base selectedEnemy, ref bool hitSomething)
    {
        IDamageable damageable = collider.GetComponent<IDamageable>();

        if (damageable == null || damageable.GetType() == typeof(Player_HealthComponent) || damageableStorage.Contains(damageable)) return;

        damageableStorage.Add(damageable);
        damageable.TakeDamage(currentAttack.damage);
        hitSomething = true;

        if (collider.gameObject.GetComponent<EnemyAI_Base>() != null && !collider.gameObject.GetComponent<EnemyAI_Base>().Invincible()) // Added the invincible check so enemies behind gates can't get targeted for script-stealing/dmg
        {
            selectedEnemy = collider.gameObject.GetComponent<EnemyAI_Base>();
        }
    }

    void CheckElementals(Collider collider, ref List<IElemental> elementalStorage, ref bool hitSomething)
    {
        IElemental elemental = collider.GetComponent<IElemental>();

        if (elemental == null || elemental.GetType() == typeof(Player_HealthComponent) || elementalStorage.Contains(elemental)) return;

        elementalStorage.Add(elemental);
        if (PlayerController.instance.ScriptSteal.BehaviorActive()) // NEEDS TO MAKE SURE THIS IS ACTIVE. If the check is done in the actual script as to whether this is active, it will check it for enemies too (BAD).
        {
            elemental.InteractElement(PlayerController.instance.ScriptSteal.GetHeldBehavior());
        }
        hitSomething = true;
    }

    IEnumerator HitCheck()
    {
        yield return new WaitForSeconds(currentAttack.hitboxDelay);

        float completionTime = currentAttack.hitboxDuration;

        hitBoxActive = true;

        EnemyAI_Base selectedEnemy = null;

        List<IDamageable> damageablesHit = new();
        List<IElemental> elementalsHit = new();

        bool increaseCombo = false;

        if (completionTime == 0)
        {
            ColliderIterationBody(ref selectedEnemy, ref damageablesHit, ref elementalsHit, ref increaseCombo);
            //check hitbox one frame - unfortunate that ref can't be set to null so there's an unnecessary extra execution happening in regards to the list but I can't complain
        }
        else
        {

            float currentTime = 0;

            while (currentTime < completionTime)
            {
                ColliderIterationBody(ref selectedEnemy, ref damageablesHit, ref elementalsHit, ref increaseCombo);

                yield return new WaitForEndOfFrame();
                currentTime += Time.deltaTime;
            }
        }
        if (selectedEnemy) PlayerController.instance.ScriptSteal.ChangeSelectedEnemy(selectedEnemy);

        hitBoxActive = false;

        if (!increaseCombo && comboResetOnWhiff) //if we attacked and we didn't hit anything combo-able, then reset the combo
        {
            CancelInvoke("ResetComboCount");
            ResetComboCount();
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 inputDir = IntendedMoveDirection();

        if (inputDir.magnitude < 0.5f && PlayerController.instance != null) inputDir = PlayerController.instance.MovementMachine.ForwardDirection;

        Gizmos.DrawWireSphere(transform.position + inputDir * (maxLeapDistance / 2f), maxLeapDistance / 2f);

        if (!showHitboxCollider) return;

        if ((hitBoxActive && onlyShowHitboxWhenEnabled) || !onlyShowHitboxWhenEnabled)
        {
            Vector3 dir = PlayerController.instance.MovementMachine.ForwardDirection == Vector3.zero ? transform.forward : PlayerController.instance.MovementMachine.ForwardDirection;

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
        return PlayerController.instance.MovementMachine.ForwardDirection * vector.z + PlayerController.instance.MovementMachine.RightDirection * vector.x + Vector3.up * vector.y;
    }

    Vector3 IntendedMoveDirection()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);

        Vector2 moveInput = walkInput.action.ReadValue<Vector2>();

        return moveInput.x * camRight.normalized + moveInput.y * camForward.normalized;
    }

    //Combo Count Functions
    void UpdateComboCount()
    {
        currentComboCount++;
        Ui_ComboCounter.instance?.UpdateComboDisplay(currentComboCount);
        CancelInvoke("ResetComboCount");
        Invoke("ResetComboCount", comboCountResetTime + currentAttack.animation.length);
    }

    void ResetComboCount()
    {
        if (currentComboCount > 0) PlayerController.instance.Mana.GainMana(currentComboCount * comboManaBonusMultiplier);

        currentComboCount = 0;
        Ui_ComboCounter.instance?.UpdateComboDisplay(0);
    }
}
