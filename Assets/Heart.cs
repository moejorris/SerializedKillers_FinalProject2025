using UnityEngine;

public class Heart : MonoBehaviour
{
    public float healAmount = 4;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("Canvas").GetComponent<PlayerHealth>().HealDamage(healAmount);
            Destroy(gameObject);
        }
    }
}
