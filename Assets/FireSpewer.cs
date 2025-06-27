using UnityEngine;

public class FireSpewer : MonoBehaviour, IDamageable, IElemental
{
    public ParticleSystem smoke;
    [SerializeField] private Behavior heldBehavior; // just to tell which type of element(?)
    [SerializeField] private GameObject fireStatusEffect;
    private bool fireActive = true;
    private ParticleSystem fire;
    [SerializeField] private int fps;
    private float timeElapsed = 0;
    private float displayTime = 0;
    public float fireDuration = 5;
    private Player_ScriptSteal playerScriptSteal => GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController").GetComponent<Player_ScriptSteal>();

    private void Start()
    {
        fire = transform.Find("Fire").GetComponent<ParticleSystem>();
        smoke = transform.Find("Smoke").GetComponent<ParticleSystem>();
    }

    public void PutOutFire()
    {
        if (!fireActive) return;
        fireActive = false;
        fire.Stop();
        smoke.Play();
    }

    public void StartFire()
    {
        if (fireActive) return;
        fireActive = true;
        fire.Play();
        smoke.Stop();
    }

    private void LateUpdate()
    {
        timeElapsed += Time.deltaTime;

        if ((timeElapsed - displayTime) > 1f / fps && fireActive)
        {
            displayTime = timeElapsed;
            fire.Simulate(0.15f, true, false, false);
            fire.Pause();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (fireActive)
        {
            Debug.Log(other.gameObject.name);
            if (other.transform.parent != null && other.transform.parent.GetComponent<EnemyAI_Base>() != null)
            {
                if (other.transform.parent.GetComponent<EnemyAI_Base>().heldBehavior.behaviorName == "water" && other.transform.parent.GetComponent<EnemyAI_Base>().behaviorActive)
                {
                    PutOutFire();
                }
            }
            else if (other.transform.parent != null && other.transform.parent.gameObject.CompareTag("Player"))
            {
                //Vector3 dir = (other.transform.position - transform.position).normalized + Vector3.up * 0.25f;
                //GameObject.Find("Player").transform.Find("PlayerController").GetComponent<Player_ForceHandler>().AddForce(dir * 20f, ForceMode.VelocityChange);

                if (other.transform.parent.Find("Meshes").childCount < 3)
                {
                    GameObject effect = Instantiate(fireStatusEffect, other.transform.parent.Find("Meshes"));
                    effect.transform.position = other.transform.parent.Find("Meshes").position;
                }
                else
                {
                    FireDamageEffect damageEffect = other.transform.parent.Find("Meshes").GetComponentInChildren<FireDamageEffect>();
                    if (damageEffect != null)
                    {
                        damageEffect.fireLifetime = 5;
                    }
                    // deals more damage since entering fire while on fire?
                }
            }
        }
        else
        {
            Debug.Log(other.gameObject.name);
            if (other.transform.parent != null && other.transform.parent.GetComponent<EnemyAI_Base>() != null)
            {
                if (other.transform.parent.GetComponent<EnemyAI_Base>().heldBehavior.behaviorName == "fire")
                {
                    PutOutFire();
                }
            }
        }
    }

    public void TakeDamage(float damage, Player_ScriptSteal script)
    {
        if (playerScriptSteal.GetHeldHebavior())
        {
            if (playerScriptSteal.GetHeldHebavior() == heldBehavior)
            {
                PutOutFire();
            }
            else if (playerScriptSteal.GetHeldHebavior() == heldBehavior)
            {
                StartFire();
            }
        }
    }

    void IElemental.InteractElement(Behavior behavior)
    {
        if (!behavior) return;

        if (behavior == heldBehavior.weakness) PutOutFire();
        else if (behavior == heldBehavior) StartFire();
    }
}
