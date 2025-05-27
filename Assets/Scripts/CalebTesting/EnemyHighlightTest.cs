using UnityEngine;

public class EnemyHighlightTest : MonoBehaviour
{
    public ScriptStealMenu UIStuff;
    public Behavior heldBehavior;

    private void OnMouseEnter()
    {
        transform.parent.gameObject.layer = 6;
        UIStuff.centerSlot.AddBehavior(heldBehavior);
    }

    private void OnMouseExit()
    {
        transform.parent.gameObject.layer = 0;
        UIStuff.centerSlot.RemoveBehavior();
    }
}
