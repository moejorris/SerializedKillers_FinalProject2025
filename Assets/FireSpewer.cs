using UnityEngine;

public class FireSpewer : MonoBehaviour, IElemental
{
    public bool canBeReignited = true;
    private ParticleSystem smoke;
    [SerializeField] private Behavior heldBehavior; // just to tell which type of element(?)
    [SerializeField] private GameObject fireStatusEffect;
    private bool fireActive = true;
    private ParticleSystem fire;
    [SerializeField] private int fps;
    private float timeElapsed = 0;
    private float displayTime = 0;
    public float fireDuration = 5;
    private float enemyExtinguishTimer = 1;

    private float health = 3;
    private float knockbackCooldown = 0;
    public float knockbackStrength = 10;

    private void Start()
    {
        fire = transform.Find("Fire").GetComponent<ParticleSystem>();
        smoke = transform.Find("Smoke").GetComponent<ParticleSystem>();
    }

    public void PutOutFire()
    {
        if (!fireActive) return;

        health--;

        enemyExtinguishTimer = 1;

        Vector3 newPos = smoke.transform.localPosition;

        if (health <= 0)
        {
            fireActive = false;
            //fire.Stop();
            ParticleSystem.MainModule main = fire.main;
            main.startLifetime = 0f;
            // fire.loop = false;

            newPos.y = 0.4f;
        }
        else if (health <= 1)
        {
            ParticleSystem.MainModule main = fire.main;
            main.startLifetime = 1.3f;

            newPos.y = 1.2f;
        }
        else
        {
            ParticleSystem.MainModule main = fire.main;
            main.startLifetime = 1.7f;

            newPos.y = 2;
        }

        smoke.transform.localPosition = newPos;
        smoke.Play();
    }

    public void StartFire()
    {
        if (canBeReignited)
        {
            if (fireActive) return;

            // fire.loop = true;
            health = 3;
            ParticleSystem.MainModule main = fire.main;
            main.startLifetime = 2f;

            fireActive = true;
            fire.Play();
            smoke.Stop();
        }
    }

    private void LateUpdate()
    {
        timeElapsed += Time.deltaTime;

        //if ((timeElapsed - displayTime) > 1f / fps)
        //{
        //    displayTime = timeElapsed;
        //    fire.Simulate(0.15f, true, false, false);
        //    fire.Pause();
        //}

        if (enemyExtinguishTimer > 0)
        {
            enemyExtinguishTimer -= Time.deltaTime;
        }
    }

    private void Update()
    {
        if (knockbackCooldown > 0) knockbackCooldown -= Time.deltaTime;
        //Debug.Log(smoke.transform.localPosition.y);

        //2, 1.2, 0.4
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
                    if (enemyExtinguishTimer <= 0)
                    {
                        PutOutFire();
                    }
                }
            }
            else if (other.transform.parent != null && other.transform.parent.gameObject.CompareTag("Player"))
            {
                PlayerController.instance.ScriptSteal.ApplyStatusEffect(heldBehavior);
                
                if (knockbackCooldown <= 0)
                {
                    knockbackCooldown = 0.5f;

                    Vector3 dir = (other.transform.position - GetComponent<Collider>().ClosestPointOnBounds(other.transform.position)).normalized;
                    dir.y = 0.3f;
                    dir *= 5;
                    dir *= knockbackStrength;

                    PlayerController.instance.ForceHandler.AddForce(dir, ForceMode.VelocityChange);
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

    void IElemental.InteractElement(Behavior behavior)
    {
        if (!behavior) return;

        if (behavior == heldBehavior.weakness) PutOutFire();
        else if (behavior == heldBehavior) StartFire();
    }
}
