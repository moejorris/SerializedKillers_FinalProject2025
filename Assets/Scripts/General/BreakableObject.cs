using Unity.VisualScripting;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamageable
{
    [Range(0, 100)]
    [SerializeField] private int itemDropChance = 25;
    [SerializeField] private GameObject heldItem;
    [SerializeField] private Behavior requiredBehavior;
    [SerializeField] private int heldHeartAmount = 1;
    [SerializeField] private int health = 1;
    [SerializeField] private GameObject smokeParticle;

    [SerializeField] private GameObject requiredToBreakFirst;
    [SerializeField] private SoundEffectSO sfx_destroyed;

    public virtual void TakeDamage(float damage)
    {
        if (requiredToBreakFirst == null)
        {
            health--;

            if (transform.GetComponent<Animator>() != null) transform.GetComponent<Animator>().Play("DestructableObject", 0, 0);

            if (sfx_destroyed != null) SoundManager.instance.PlaySoundEffect(sfx_destroyed);

            if (health <= 0)
            {
                if (heldItem != null)
                {
                    Vector3 spawnPos = transform.position;
                    spawnPos.y += 1f;
                    int num = Random.Range(1, 101);
                    if (num <= itemDropChance)
                    {
                        float angle = 360 / heldHeartAmount;

                        for (int i = 0; i < heldHeartAmount; i++)
                        {
                            Rigidbody rb = Instantiate(heldItem, spawnPos, Quaternion.identity).GetComponent<Rigidbody>();

                            if (heldHeartAmount > 1)
                            {
                                Vector2 dir2D = DegreeToVector2(angle * (i + 1));
                                Vector3 dir = new Vector3(dir2D.x, 1, dir2D.y);
                                rb.AddForce(dir, ForceMode.Impulse);
                            }
                            else rb.AddForce(new Vector3(0, 1, 0), ForceMode.Impulse);
                        }
                    }
                }

                Instantiate(smokeParticle, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    Vector2 DegreeToVector2(float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        float x = Mathf.Sin(angleInRadians);
        float y = Mathf.Cos(angleInRadians);

        return new Vector2(x, y);
    }
}
