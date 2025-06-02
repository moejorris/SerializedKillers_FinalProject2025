using System.Collections.Generic;
using UnityEngine;

public class Player_CombatMachine : MonoBehaviour
{
    [SerializeField] AnimatorOverrideController animatorOverride;
    [SerializeField] Animator animator;
    [SerializeField] List<PlayerAttackSO> groundedAttacks = new List<PlayerAttackSO>();


    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            PlayerAttackSO attack = groundedAttacks[Random.Range(0, groundedAttacks.Count)];

            animatorOverride["AttackPlaceholder"] = attack.animation;
            animator.runtimeAnimatorController = animatorOverride;
            animator.CrossFade("Attack", 0.05f , 0);

            if (attack.usesRootMotion)
            {
                GetComponent<Player_MovementMachine>().DisableAllMovers(GetComponent<Player_RootMotion>());
                CancelInvoke("ReEnableMovers");
                Invoke("ReEnableMovers", attack.animation.length * 0.9f);
            }
        }
    }

    void ReEnableMovers()
    {
        GetComponent<Player_MovementMachine>().EnableAllMovers();
    }
}
