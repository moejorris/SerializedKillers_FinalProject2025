using UnityEngine;

public class BreakableObject : MonoBehaviour, ITargetable, IDamageable
{
    [Range(0, 100)]
    [SerializeField] private int itemDropChance = 25;
    [SerializeField] private GameObject heldItem;
    [SerializeField] private Behavior requiredBehavior;

    public float TargetScore { get; set;}
    public float TargetScoreWeight { get => 0.2f; }

    public virtual void TakeDamage(float damage, Player_ScriptSteal scriptSteal)
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
