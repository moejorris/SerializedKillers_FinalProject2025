using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Range(0, 100)]
    [SerializeField] private int itemDropChance = 25;
    [SerializeField] private GameObject heldItem;
    [SerializeField] private Behavior requiredBehavior;

    public void DestroyObjct()
    {
        if (heldItem != null)
        {
            int num = Random.Range(1, 101);
            if (num <= itemDropChance)
            {
                Instantiate(heldItem, transform.position, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }
}
