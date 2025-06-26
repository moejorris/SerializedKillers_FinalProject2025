using UnityEngine;

public class FireDamageEffect : MonoBehaviour
{
    private PlayerHealth playerHealth => GameObject.FindGameObjectWithTag("Canvas").GetComponent<PlayerHealth>();
    public float fireLifetime = 4;
    [SerializeField] private float fireDamage = 1;
    [SerializeField] private float timeBetweenDamage = 1;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        if (playerHealth != null)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenDamage)
            {
                timer = 0;
                playerHealth.TakeDamage(fireDamage);
            }
        }

        fireLifetime -= Time.deltaTime;
        if (fireLifetime <= 1)
        {
            GetComponent<ParticleSystem>().Stop();
            if (fireLifetime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
