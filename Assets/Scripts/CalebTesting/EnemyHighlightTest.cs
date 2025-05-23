using UnityEngine;

public class EnemyHighlightTest : MonoBehaviour
{
    private void OnMouseEnter()
    {
        transform.parent.gameObject.layer = 6;
    }

    private void OnMouseExit()
    {
        transform.parent.gameObject.layer = 0;
    }
}
