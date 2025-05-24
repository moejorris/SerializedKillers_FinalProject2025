using UnityEngine;

public class EnemyHighlightTest : MonoBehaviour
{
    public ScriptStealMenu UIStuff;

    private void OnMouseEnter()
    {
        transform.parent.gameObject.layer = 6;
        UIStuff.currentSelectedEnemySprite = transform.parent.GetComponent<EnemyAbilityTest>().heldSprite;
    }

    private void OnMouseExit()
    {
        transform.parent.gameObject.layer = 0;
        UIStuff.currentSelectedEnemySprite = null;
    }
}
