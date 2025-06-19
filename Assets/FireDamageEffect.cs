using UnityEngine;

public class FireDamageEffect : MonoBehaviour
{
    private PlayerHealth playerHealth => GameObject.FindGameObjectWithTag("Canvas").GetComponent<PlayerHealth>();
    [SerializeField] private float fireLifetime = 5;
    [SerializeField] private float fireDamage = 1;
    [SerializeField] private float timeBetweenDamage = 1;
    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.Stop();

        var main = ps.main;
        main.duration = fireLifetime;

        ps.Play();
    }

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
    }
}
