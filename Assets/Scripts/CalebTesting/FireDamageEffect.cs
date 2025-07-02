using UnityEngine;
using System.Collections;

public class FireDamageEffect : MonoBehaviour, IElemental
{
    private Player_HealthComponent playerHealth => GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController").GetComponent<Player_HealthComponent>();
    private Player_ScriptSteal scriptSteal => GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController").GetComponent<Player_ScriptSteal>();

    public bool fireActive = false;

    public float fireLifetime = 5;
    [SerializeField] private float fireDamage = 1;
    [SerializeField] private float timeBetweenDamage = 1;
    [SerializeField] Behavior heldBehavior;
    public float rekindleTimer = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if (fireActive)
        {
            if (rekindleTimer > 0)
            {
                rekindleTimer -= Time.deltaTime;
            }
            else
            {
                fireLifetime -= Time.deltaTime;
                if (fireLifetime <= 0)
                {
                    StopFire();
                }
            }
        }
    }

    public IEnumerator FireTimer()
    {
        while (fireActive)
        {
            if (rekindleTimer > 0) yield return new WaitForSeconds(0.5f);
            else yield return new WaitForSeconds(timeBetweenDamage);

            if (scriptSteal.GetHeldHebavior() != null && scriptSteal.GetHeldHebavior() == heldBehavior) Debug.Log("Something!");
            else playerHealth.TakeDamage(fireDamage, scriptSteal); 
        }
    }

    public void StartFire()
    {
        rekindleTimer = 0.3f;
        fireLifetime = 5;

        if (fireActive) return;

        fireActive = true;
        GetComponent<ParticleSystem>().Play();
        StopCoroutine("FireTimer");
        StartCoroutine("FireTimer");
    }

    public void StopFire()
    {
        fireActive = false;
        GetComponent<ParticleSystem>().Stop();
        StopCoroutine("FireTimer");
        fireLifetime = 0;
    }

    public void InteractElement(Behavior behavior)
    {
        if (!behavior) return;
        if (behavior == heldBehavior.weakness)
        {
            StopFire(); // this essentially causes the fire to go out
        }
    }
}
