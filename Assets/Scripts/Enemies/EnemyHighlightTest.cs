using UnityEngine;

public class EnemyHighlightTest : MonoBehaviour
{
    public ScriptStealMenu UIStuff;
    public Behavior heldBehavior;
    private bool delayedExit = false;

    private void OnMouseEnter()
    {
        if (UIStuff.selectedEnemy == null)
        {
            transform.parent.gameObject.layer = 6;
            //UIStuff.selectedEnemy = this;
            UIStuff.centerSlot.AddBehavior(heldBehavior);
        }
        delayedExit = false;
    }

    private void OnMouseExit()
    {
        if (UIStuff.selectedEnemy == this && UIStuff.menuOpen)
        {
            delayedExit = true;
        }
        else
        {
            if (UIStuff.selectedEnemy == this)
            {
                UIStuff.selectedEnemy = null;
                UIStuff.centerSlot.RemoveBehavior();
            }
            transform.parent.gameObject.layer = 0;
        }
    }

    private void Update()
    {
        if (delayedExit && !UIStuff.menuOpen)
        {
            delayedExit = false;
            transform.parent.gameObject.layer = 0;
            UIStuff.selectedEnemy = null;
            UIStuff.centerSlot.RemoveBehavior();
        }
    }
}
