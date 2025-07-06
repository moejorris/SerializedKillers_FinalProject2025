using UnityEngine;

public class Spin : MonoBehaviour, IDamageable
{
    [SerializeField] private string requiredBehaviorName;
    private RotatePuzzle rotatePuzzle;

    void Start()
    {
        rotatePuzzle = GetComponentInChildren<RotatePuzzle>();
    }

    public void TakeDamage(float damage = 0)
    {
        if (PlayerController.instance.ScriptSteal.GetHeldHebavior() != null && PlayerController.instance.ScriptSteal.GetHeldHebavior().behaviorName == requiredBehaviorName)
        {
            rotatePuzzle.RotateThisPuzzle();
        }
        else
        {
            string heldBehaviorName = "none";
            if (PlayerController.instance.ScriptSteal.GetHeldHebavior() != null) heldBehaviorName = PlayerController.instance.ScriptSteal.GetHeldHebavior().behaviorName;
            Debug.Log("Tile Spin Failed. Required behavior: " + requiredBehaviorName + ", but held behavior: " + heldBehaviorName);
        }
    }
}
