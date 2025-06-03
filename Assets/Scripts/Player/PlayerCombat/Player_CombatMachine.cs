using System.Collections.Generic;
using UnityEngine;

//Joe Morris
//SGD FP '25

public class Player_CombatMachine : MonoBehaviour
{
    [SerializeField] AnimatorOverrideController animatorOverride;
    [SerializeField] Animator animator;
    [SerializeField] List<PlayerAttackSO> defaultAttacks = new List<PlayerAttackSO>();

    [SerializeField] float comboResetTime = 0.3f;
    [SerializeField] int currentComboID = 0;

    bool isAttacking = false;
    bool attackQueued = false;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
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

        animatorOverride["AttackPlaceholder"] = attack.animation;
        animator.runtimeAnimatorController = animatorOverride;
        animator.Play("Attack", 0);

        if (attack.usesRootMotion)
        {
            GetComponent<Player_MovementMachine>().DisableAllMovers(GetComponent<Player_RootMotion>());
            CancelInvoke("ReEnableMovers");
            Invoke("ReEnableMovers", attack.animation.length * 0.9f);
        }

        CancelInvoke("ResetCombo");
        Invoke("ResetCombo", comboResetTime + attack.animation.length);

        GetNextAttack();
    }

    void ReEnableMovers()
    {
        GetComponent<Player_MovementMachine>().EnableAllMovers();
        animator.Play("Default");
        isAttacking = false;
    }

    void GetNextAttack()
    {
        if (currentComboID < defaultAttacks.Count - 1)
        {
            currentComboID++;
        }
        else ResetCombo();

        // currentComboID = Mathf.Clamp(currentComboID, 0, defaultAttacks.Count); //Don't clamp to Count - 1 because then the combo would never reset
    }

    void ResetCombo()
    {
        currentComboID = 0;
    }

    bool OutOfRangeCheck()
    {
        Debug.Log(currentComboID);

        if (currentComboID < 0) return true;
        if (currentComboID >= defaultAttacks.Count) return true;

        return false;
    }
}
