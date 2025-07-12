using UnityEngine;

public class ElectricWater : MonoBehaviour
{
    [Header("Electric Water Settings")]
    [Tooltip("Interval between shocks in seconds")]
    [SerializeField] private float shockInterval = 2f;
    private float shockTimer = 0f;
    [Tooltip("Damage dealt to player on shock")]
    [SerializeField] private float damage = 5f;

    private bool playerHasElectric = false;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var playerController = PlayerController.instance;
            if (playerController != null && other == playerController.Collider)
            {
                var scriptSteal = playerController.ScriptSteal;
                bool hasElectricActive = scriptSteal.GetHeldBehavior() != null && scriptSteal.GetHeldBehavior().behaviorName == "electric" && scriptSteal.BehaviorActive();
                playerHasElectric = hasElectricActive;

                if (playerHasElectric) return;

                shockTimer += Time.deltaTime;
                if (shockTimer >= shockInterval)
                {
                    playerController.Health.TakeDamage(damage);
                    shockTimer = 0f;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            shockTimer = shockInterval;
        }
    }

    void OnTriggerExit(Collider other)
    {
        var playerController = PlayerController.instance;
        if (playerController != null && other == playerController.Collider)
        {
            shockTimer = 0f;
            playerHasElectric = false;
        }
    }
}
