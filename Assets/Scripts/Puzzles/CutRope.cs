using UnityEngine;

public class CutRope : MonoBehaviour, IElemental
{
    private Animator anim;
    [Header("Puzzle Settings")]
    [Tooltip("The name of the behavior required to cut the rope.")]
    [SerializeField] private string requiredBehaviorName;
    [SerializeField] private GameObject gateToOpen;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // public void TakeDamage(float damage = 0)
    // {
    //     if (PlayerController.instance.ScriptSteal.GetHeldBehavior() != null && PlayerController.instance.ScriptSteal.GetHeldBehavior() == heldBehavior && PlayerController.instance.ScriptSteal.BehaviorActive())
    //     {
    //         anim.SetTrigger("Cut"); // Trigger the cut animation
    //         Debug.Log("Rope cut with behavior: " + heldBehavior.name);
    //         Collider col = GetComponent<Collider>();
    //         if (col != null) col.enabled = false;
    //     }
    //     else
    //     {
    //         string heldBehaviorName = "none";
    //         if (PlayerController.instance.ScriptSteal.GetHeldBehavior() != null) heldBehaviorName = PlayerController.instance.ScriptSteal.GetHeldBehavior().behaviorName;
    //         Debug.Log("Rope cutting failed. Required behavior: " + heldBehavior.name + ", but held behavior: " + heldBehaviorName);
    //     }
    // }

    public void InteractElement(Behavior behavior)
    {
        if (behavior != null)
        {
            if (behavior.behaviorName.ToLower() == requiredBehaviorName.ToLower())
            {
                anim.SetTrigger("Cut"); // Trigger the cut animation
                Debug.Log("Rope cut with behavior: " + behavior.behaviorName);
                Collider col = GetComponent<Collider>();
                if (col != null) col.enabled = false;
            }
            else
            {
                string heldBehaviorName = "none";
                if (PlayerController.instance.ScriptSteal.GetHeldBehavior() != null) heldBehaviorName = PlayerController.instance.ScriptSteal.GetHeldBehavior().behaviorName;
                Debug.Log("Rope cutting failed. Required behavior: " + requiredBehaviorName + ", but held behavior: " + heldBehaviorName);
            }
        }
    }

    void LiftGate()
    {
        if (gateToOpen != null)
        {
            OpenGate gate = gateToOpen.GetComponent<OpenGate>();
            if (gate != null)
            {
                gate.LiftUp(); // Call the method to open the gate
                Debug.Log("Gate is attempting to open");
            }
            else
            {
                Debug.LogWarning("gateToOpen does not have the OpenGate component attached!");
            }
        }
    }
}