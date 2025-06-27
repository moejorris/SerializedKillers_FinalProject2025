using UnityEngine;

public class CutRope : MonoBehaviour, IDamageable
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

    public void TakeDamage(float damage = 0, Player_ScriptSteal scriptSteal = null)
    {
        if (scriptSteal != null && scriptSteal.HeldBehavior != null && scriptSteal.HeldBehavior.behaviorName == requiredBehaviorName)
        {
            anim.SetTrigger("Cut"); // Trigger the cut animation
            Debug.Log("Rope cut with behavior: " + requiredBehaviorName);
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
        else
        {
            string heldBehaviorName = "none";
            if (scriptSteal != null && scriptSteal.HeldBehavior != null) heldBehaviorName = scriptSteal.HeldBehavior.behaviorName;
            Debug.Log("Rope cutting failed. Required behavior: " + requiredBehaviorName + ", but held behavior: " + heldBehaviorName);
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