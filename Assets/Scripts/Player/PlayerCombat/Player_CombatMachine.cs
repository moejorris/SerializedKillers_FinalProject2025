using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


//Joe Morris
//SGD FP '25

public class Player_CombatMachine : MonoBehaviour
{
    Player_ScriptSteal scriptStealMenu => GetComponent<Player_ScriptSteal>();
    Player_MovementMachine _machine => GetComponent<Player_MovementMachine>();
    Player_RootMotion _rootMotion => GetComponent<Player_RootMotion>();
    Player_ForceHandler _forceHandler => GetComponent<Player_ForceHandler>();
    Player_Rotate _rotation => GetComponent<Player_Rotate>();
    Player_ScriptSteal _scriptSteal => GetComponent<Player_ScriptSteal>();


    [SerializeField] AnimatorOverrideController animatorOverride;
    int currentAnim = 1;

    [SerializeField] Animator animator;
    [SerializeField] List<PlayerAttackSO> defaultAttacks = new List<PlayerAttackSO>();

    [SerializeField] float comboResetTime = 0.3f;
    [SerializeField] int currentComboID = 0;

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

    //Local Stuff
    bool isAttacking = false;
    bool attackQueued = false;

    PlayerAttackSO currentAttack;

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

        //TODO:
        //Also standard targeting method
        //Get target (somehow)
        //Rotate player towards it (not immediate snap)
        //Make cam look at it... look at avg position of player and target (player.position + target.position / 2f)

        isAttacking = true;
        attackQueued = false;

        currentAttack = defaultAttacks[currentComboID];

        HandleAnimation();


        float attackTime = currentAttack.animation.length / currentAttack.animationSpeed; // find a way to get the speed of the state and divide the length by that...

        StartCoroutine(HitCheck());
        HandleMovement();
        HandleRotation();
        HandleMoveToTarget();

        animator.speed = 0;

        //Wait for dash to finish/player is close to enemy to start attack

        animator.speed = currentAttack.animationSpeed;

        HandleSound();
        HandleParticle();

        CancelInvoke("AttackIsDone");
        Invoke("AttackIsDone", attackTime * 0.75f);


        CancelInvoke("ResetCombo");
        Invoke("ResetCombo", comboResetTime + attackTime);

        GetNextAttack();
    }

    void HandleMoveToTarget()
    {
        //TODO: Create Score/Weight System that factors both input direction and closeness
        //TODO: Move player over time

        if (!isBatman) return;
        if (!_machine.isGrounded) return;

        Vector3 inputDir = IntendedMoveDirection();

        if (inputDir.magnitude < 0.5f) inputDir = _machine.ForwardDirection;

        Collider[] checkSphereColliders = Physics.OverlapSphere(transform.position + inputDir * (maxLeapDistance / 2f), maxLeapDistance / 2f, ~0, QueryTriggerInteraction.Ignore);

        List<ITargetable> potentialTargets = new();

        foreach (Collider collider in checkSphereColliders)
        {
            ITargetable target = collider.GetComponent<ITargetable>();

            if (target == null) continue;

            float dist = Vector3.Distance(transform.position, target.transform.position);

            if (dist > maxLeapDistance) continue;

            if (dist > checkRadius && walkInput.action.ReadValue<Vector2>().magnitude == 0) continue;

            RaycastHit hit;

            if (!Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, dist + 2, ~0, QueryTriggerInteraction.Ignore) || hit.collider != collider) continue;

            potentialTargets.Add(target);
        }

        // Debug.Log("Arkham Combat: Found " + potentialTargets.Count + " potential targets.");

        if (potentialTargets.Count < 1) return;

        ITargetable selectedTarget = null;

        foreach (ITargetable target in potentialTargets)
        {
            target.TargetScore = 0;

            float testDot = Vector3.Dot(target.transform.position - transform.position, inputDir);

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

        // foreach (ITargetable target in potentialTargets)
        // {
        //     if (target.TargetScore == highScore)
        //     {
        //         Handles.color = Color.green;
        //     }
        //     else if (target.TargetScore >= highScore / 2f)
        //     {
        //         Handles.color = Color.red;
        //     }
        //     else
        //     {
        //         Handles.color = Color.black;
        //     }

        //     // Handles.DrawSolidDisc(target.transform.position + Vector3.up, Vector3.up, 1f);
        // }

        if (selectedTarget == null || highScore == 0) return;

        _rotation.enabled = false;

        Debug.Log("Arkham Combat: Found Most In Line Target: " + selectedTarget.transform.name);

        float targetDistance = Vector3.Distance(selectedTarget.transform.position, transform.position);

        if (targetDistance > minDashDistance)
        {
            _machine.DisableAllMovers(GetComponent<Player_Dash>());
            GetComponent<Player_Dash>().ExternalDash(selectedTarget.transform.position);
        }
        else
        {
            Vector3 newDir = selectedTarget.transform.position - transform.position;
            newDir.y = 0;

            _machine.SetForwardDirection(newDir);
            _machine.DisableAllMovers(_forceHandler);
            _forceHandler.AddForce(_machine.ForwardDirection * Vector3.Distance(transform.position, selectedTarget.transform.position - _machine.ForwardDirection * checkRadius) * 5f, ForceMode.VelocityChange);        
        }
    }

    void HandleParticle()
    {
        if (currentAttack.particleEffect)
        {
            ParticleSystem.MainModule particle = Instantiate(currentAttack.particleEffect, animator.transform).GetComponent<ParticleSystem>().main;
            if (_scriptSteal)
            {
                particle.startColor = _scriptSteal.scriptEffectColor;
            }
            else
            {
                particle.startColor = Color.white;
            }
        }
    }

    void HandleRotation()
    {
        if (currentAttack.lockRotation)
        {
            _rotation.enabled = false;
        }
    }

    void HandleMovement()
    {
        if (isBatman) return;

        if (currentAttack.overrideMotion)
        {
            if (currentAttack.usesRootMotion) _machine.DisableAllMovers(_rootMotion);
            else _rootMotion.enabled = false;
            _machine.DisableAllMovers(_forceHandler);
        }

        if (currentAttack.vectorForce.magnitude > 0) // only apply force if necessary
        {
            _forceHandler.AddForce(LocalizeForceVector(currentAttack.vectorForce), ForceMode.VelocityChange, Player_ForceHandler.OverrideMode.All);
        }
    }

    void HandleSound()
    {
        if (currentAttack.swingSound != null)
        {
            if (currentAttack.hitboxDelay > 0)
            {
                StopCoroutine("PlaySoundDelayed_TEMP");
                StartCoroutine(PlaySoundDelayed_TEMP(currentAttack.swingSound, currentAttack.hitboxDelay));
            }
            else PlaySound_TEMP(currentAttack.swingSound);
        }
    }

    void HandleImpactSound()
    {
        if (currentAttack.impactSound != null)
        {
            PlaySound_TEMP(currentAttack.impactSound);
        }
    }

    IEnumerator PlaySoundDelayed_TEMP(SoundEffectSO sound, float delay = 0) ///TEMP Until a proper sound manager/player is developed.
    {
        yield return new WaitForSeconds(delay);
        PlaySound_TEMP(sound);
    }

    void PlaySound_TEMP(SoundEffectSO sound)
    {
        GameObject soundObject = Instantiate(new GameObject(), transform);
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        AudioClip clip = sound.SoundEffect();

        if (sound.usesRandomPitch)
        {
            audioSource.pitch = sound.RandomPitch;
        }

        audioSource.PlayOneShot(clip);
        Destroy(soundObject, clip.length);
        // Debug.Log("Clip length = " + clip.length);
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

        _machine.EnableAllMovers();
        _rootMotion.enabled = false;
        _rotation.enabled = true;

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

    IEnumerator HitCheck()
    {
        yield return new WaitForSeconds(currentAttack.hitboxDelay);

        float completionTime = currentAttack.hitboxDuration;

        hitBoxActive = true;

        EnemyAI_Base selectedEnemy = null;

        // START OF WHAT I (THE CALEB) ADDED

        if (completionTime == 0)
        {
            Collider[] hitObjects = Physics.OverlapSphere(transform.position + _machine.ForwardDirection * distanceForwards, checkRadius, ~0, QueryTriggerInteraction.Collide);

            bool hitSomething = false;
            foreach (Collider collider in hitObjects)
            {
                if (collider.CompareTag("Player")) continue;

                IElemental elemental = collider.GetComponent<IElemental>();

                if (elemental == null || elemental.GetType() == typeof(Player_HealthComponent)) continue;

                elemental.InteractElement(scriptStealMenu.GetHeldHebavior());

                hitSomething = true;
            }
            if (hitSomething) HandleImpactSound(); // something different maybe for element interact?
        }
        else
        {
            List<IElemental> elementalsHit = new();

            float currentTime = 0;

            while (currentTime < completionTime)
            {
                Collider[] hitObjects = Physics.OverlapSphere(transform.position + _machine.ForwardDirection * distanceForwards, checkRadius, ~0, QueryTriggerInteraction.Collide);

                bool somethingHitThisFrame = false;

                foreach (Collider collider in hitObjects)
                {
                    if (collider.CompareTag("Player")) continue;

                    IElemental elemental = collider.GetComponent<IElemental>();

                    if (elemental == null || elementalsHit.Contains(elemental) || elemental.GetType() == typeof(Player_HealthComponent)) continue;


                    elemental.InteractElement(scriptStealMenu.GetHeldHebavior());
                    elementalsHit.Add(elemental);
                    somethingHitThisFrame = true;
                }

                if (somethingHitThisFrame) HandleImpactSound();

                yield return new WaitForEndOfFrame();
                currentTime += Time.deltaTime;
            }
        }

        // END OF WHAT I (THE CALEB) ADDED

        //TODO: Refactor this so both long and one frame hit checks use a func since they use much of the same code
        if (completionTime == 0)
        {
            //check hitbox one frame

            Collider[] hitObjects = Physics.OverlapSphere(transform.position + _machine.ForwardDirection * distanceForwards, checkRadius, ~0, QueryTriggerInteraction.Ignore);

            bool hitSomething = false;
            foreach (Collider collider in hitObjects)
            {
                if (collider.CompareTag("Player")) continue;

                IDamageable damageable = collider.GetComponent<IDamageable>();

                if (damageable == null || damageable.GetType() == typeof(Player_HealthComponent)) continue;

                selectedEnemy = collider.gameObject.GetComponent<EnemyAI_Base>();

                damageable.TakeDamage(currentAttack.damage, _scriptSteal);
                hitSomething = true;
            }
            if (hitSomething) HandleImpactSound();
        }
        else
        {
            List<IDamageable> damageablesHit = new();

            float currentTime = 0;

            while (currentTime < completionTime)
            {
                Collider[] hitObjects = Physics.OverlapSphere(transform.position + _machine.ForwardDirection * distanceForwards, checkRadius, ~0, QueryTriggerInteraction.Ignore);

                bool somethingHitThisFrame = false;

                foreach (Collider collider in hitObjects)
                {
                    if (collider.CompareTag("Player")) continue;

                    IDamageable damageable = collider.GetComponent<IDamageable>();

                    if (damageable == null || damageablesHit.Contains(damageable) || damageable.GetType() == typeof(Player_HealthComponent)) continue;

                    selectedEnemy = collider.gameObject.GetComponent<EnemyAI_Base>();


                    damageable.TakeDamage(currentAttack.damage, _scriptSteal);
                    damageablesHit.Add(damageable);
                    somethingHitThisFrame = true;
                }

                if (somethingHitThisFrame) HandleImpactSound();

                yield return new WaitForEndOfFrame();
                currentTime += Time.deltaTime;
            }
        }

        if (selectedEnemy) _scriptSteal.ChangeSelectedEnemy(selectedEnemy);

        hitBoxActive = false;
    }
    void OnDrawGizmos()
    {
        if (!showHitboxCollider) return;

        if ((hitBoxActive && onlyShowHitboxWhenEnabled) || !onlyShowHitboxWhenEnabled)
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
    
    Vector3 IntendedMoveDirection()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);

        Vector2 moveInput = walkInput.action.ReadValue<Vector2>();

        return moveInput.x * camRight.normalized + moveInput.y * camForward.normalized;
    }
}
