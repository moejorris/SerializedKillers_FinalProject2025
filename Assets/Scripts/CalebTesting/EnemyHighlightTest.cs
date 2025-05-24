using UnityEngine;

public class EnemyHighlightTest : MonoBehaviour
{
    public ScriptStealMenu UIStuff;
    public Behavior heldBehavior;

    private void OnMouseEnter()
    {
        transform.parent.gameObject.layer = 6;
        UIStuff.menu_heldBehaviorSlot.AddBehavior(heldBehavior);
    }

    private void OnMouseExit()
    {
        transform.parent.gameObject.layer = 0;
        UIStuff.menu_heldBehaviorSlot.RemoveBehavior();
    }
}
