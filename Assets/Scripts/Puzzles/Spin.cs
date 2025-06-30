using UnityEngine;

public class Spin : MonoBehaviour, IDamageable
{
    [SerializeField] private string requiredBehaviorName;
    private RotatePuzzle rotatePuzzle;

    void Start()
    {
        rotatePuzzle = GetComponentInChildren<RotatePuzzle>();
    }

    public void TakeDamage(float damage = 0, Player_ScriptSteal scriptSteal = null)
    {
        if (scriptSteal != null && scriptSteal.GetHeldHebavior() != null && scriptSteal.GetHeldHebavior().behaviorName == requiredBehaviorName)
        {
            rotatePuzzle.RotateThisPuzzle();
        }
        else
        {
            string heldBehaviorName = "none";
            if (scriptSteal != null && scriptSteal.GetHeldHebavior() != null) heldBehaviorName = scriptSteal.GetHeldHebavior().behaviorName;
            Debug.Log("Tile Spin Failed. Required behavior: " + requiredBehaviorName + ", but held behavior: " + heldBehaviorName);
        }
    }
}
