using UnityEngine;

public class EnemyAI_TutorialOverclock : EnemyAI_Overclock
{
    private TutorialManager tutorialManager => GameObject.Find("Canvas").transform.Find("TutorialManager").GetComponent<TutorialManager>();

    public override void TakeDamage(float damage)
    {
        if (!healthBar || !whiteHealthBar || Invincible()) return; // in case no thing exists

        if ((PlayerController.instance.ScriptSteal.BehaviorActive() && PlayerController.instance.ScriptSteal.GetHeldBehavior() == heldBehavior.weakness) ||
            (PlayerController.instance.ScriptSteal.GetHeldBehavior() == heldBehavior && !behaviorActive)) damage *= 2f;

        if (movementState == "cooldown") damage *= 1.5f;

        health -= damage;

        if (!PlayerController.instance.ScriptSteal.BehaviorActive())
        {
            PlayerController.instance.Mana.GainMana(manaPerHit);
        }

        //tutorialManager.StartPhaseTwo();

        StopCoroutine("MaterialFade");
        StartCoroutine("MaterialFade");

        UpdateHealth();

        if (health <= 0)
        {
            Die();
        }
    }
}
