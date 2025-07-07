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


    // void OnTriggerStay(Collider other)
    // {
    //     if (other == PlayerController.instance.Collider)
    //     {
    //         if (PlayerController.instance.ScriptSteal.GetHeldBehavior() == "electric")
    //         {
    //             playerHasElectric = true;
    //         }
    //         if (!playerHasElectric) return;
    //         shockTimer += Time.deltaTime;
    //         if (shockTimer >= shockInterval)
    //         {
    //             PlayerController.instance.Health.TakeDamage(damage);
    //             shockTimer = 0f;
    //         }
    //     }
    // }

    void OnTriggerStay(Collider other)
    {
        var playerController = PlayerController.instance;
        if (playerController != null && other == playerController.Collider)
        {
            var heldBehavior = playerController.ScriptSteal.GetHeldBehavior();
            playerHasElectric = (heldBehavior != null && heldBehavior.behaviorName == "electric");

            if (playerHasElectric) return;

            shockTimer += Time.deltaTime;
            if (shockTimer >= shockInterval)
            {
                playerController.Health.TakeDamage(damage);
                shockTimer = 0f;
            }
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
