using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Joe Morris
//SGD FP '25

public class Player_CombatMachine : MonoBehaviour
{
    [SerializeField] AnimatorOverrideController animatorOverride;
    int currentAnim = 1;

    [SerializeField] Animator animator;
    [SerializeField] List<PlayerAttackSO> defaultAttacks = new List<PlayerAttackSO>();

    [SerializeField] float comboResetTime = 0.3f;
    [SerializeField] int currentComboID = 0;

    [SerializeField] InputActionReference attackInput;

    bool isAttacking = false;
    bool attackQueued = false;

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

        //AnimatorOverrideController override = GetCurrentState.Tag == 1 ? animatorOverride1 : animatorOverride2;
        //use override for new controller
        //override.animation = attack.animation


        animatorOverride["AttackPlaceholder" + currentAnim] = attack.animation;
        animator.runtimeAnimatorController = animatorOverride;


        // animator.SetTrigger("Attack");
        animator.CrossFade("Attack"+currentAnim, 0.1f);

        if (attack.usesRootMotion)
        {
            GetComponent<Player_MovementMachine>().DisableAllMovers(GetComponent<Player_RootMotion>());
            CancelInvoke("ReEnableMovers");
            Invoke("ReEnableMovers", attack.animation.length * (currentComboID == defaultAttacks.Count -1 ? 0.9f :0.7f));
        }

        CancelInvoke("ResetCombo");
        Invoke("ResetCombo", comboResetTime + attack.animation.length);

        GetNextAttack();
    }

    void ReEnableMovers()
    {
        GetComponent<Player_MovementMachine>().EnableAllMovers();
        isAttacking = false;
    }

    void GetNextAttack()
    {
        if (currentComboID < defaultAttacks.Count - 1)
        {
            currentComboID++;
        }
        else ResetCombo();

        currentAnim = currentAnim == 1 ? 2 : 1;

        // currentComboID = Mathf.Clamp(currentComboID, 0, defaultAttacks.Count); //Don't clamp to Count - 1 because then the combo would never reset
    }

    void ResetCombo()
    {
        currentComboID = 0;
        currentAnim = 1;
    }

    bool OutOfRangeCheck()
    {
        // Debug.Log(currentComboID);

        if (currentComboID < 0) return true;
        if (currentComboID >= defaultAttacks.Count) return true;

        return false;
    }
}
