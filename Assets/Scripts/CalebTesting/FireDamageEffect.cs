using UnityEngine;

public class FireDamageEffect : MonoBehaviour, IElemental
{
    private Player_HealthComponent playerHealth => GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController").GetComponent<Player_HealthComponent>();
    private Player_ScriptSteal scriptSteal => GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController").GetComponent<Player_ScriptSteal>();
    public float fireLifetime = 5;
    [SerializeField] private float fireDamage = 1;
    [SerializeField] private float timeBetweenDamage = 1;
    [SerializeField] Behavior heldBehavior;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        if (playerHealth != null)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenDamage && fireLifetime > 1)
            {
                timer = 0;

                if (scriptSteal.GetHeldHebavior() != null && heldBehavior == scriptSteal.GetHeldHebavior())
                {
                    Debug.Log("Held Script is there and is fire!");
                }
                else
                {
                    playerHealth.TakeDamage(fireDamage);
                }

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

    public void InteractElement(Behavior behavior)
    {
        if (!behavior) return;
        if (behavior == heldBehavior.weakness)
        {
            fireLifetime = 1; // this essentially causes the fire to go out
        }
    }
}
