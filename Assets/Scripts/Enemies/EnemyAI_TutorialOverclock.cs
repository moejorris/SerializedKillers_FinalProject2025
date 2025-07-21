using UnityEngine;

public class EnemyAI_TutorialOverclock : EnemyAI_Overclock
{
    private TutorialManager tutorialManager => GameObject.FindGameObjectWithTag("Canvas").transform.Find("Tutorial").GetComponent<TutorialManager>();

    public override void Start()
    {
        base.Start();
        AttackCooldown(2500);
    }

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

        movementState = "stop";
        
        if (!tutorialManager.isRunning && behaviorActive && tutorialManager.phase < 4) tutorialManager.StartPhaseFour();

        StopCoroutine("MaterialFade");
        StartCoroutine("MaterialFade");

        UpdateHealth();

        if (health <= 0)
        {
            Die();
        }
    }

    public override void DeactivateBehavior()
    {
        base.DeactivateBehavior();

        if (!tutorialManager.isRunning && behaviorActive && tutorialManager.phase < 5) tutorialManager.StartPhaseFive();
    }
}
