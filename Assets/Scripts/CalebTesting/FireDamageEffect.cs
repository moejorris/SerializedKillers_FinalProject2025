using UnityEngine;
using System.Collections;

public class FireDamageEffect : MonoBehaviour, IElemental
{

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

            if (PlayerController.instance.ScriptSteal.BehaviorActive() && PlayerController.instance.ScriptSteal.GetHeldHebavior() == heldBehavior) Debug.Log("Something!");
            else PlayerController.instance.Health.TakeDamage(fireDamage); 
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
