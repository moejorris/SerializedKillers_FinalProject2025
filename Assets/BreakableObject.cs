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
            Vector3 spawnPos = transform.position;
            spawnPos.y += 1;
            int num = Random.Range(1, 101);
            if (num <= itemDropChance)
            {
                Rigidbody rb = Instantiate(heldItem, spawnPos, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(new Vector3(Random.Range(-100f, 100), 100, Random.Range(-100f, 100f)));
            }
        }

        Destroy(gameObject);
    }
}
