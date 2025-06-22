using UnityEngine;

public class FireHazard : MonoBehaviour
{
    [SerializeField] private Behavior heldBehavior; // just to tell which type of element(?)
    [SerializeField] private GameObject fireEffect;
    private bool fireActive = true;
    private ParticleSystem particles;
    [SerializeField] private int fps;
    private float timeElapsed = 0;
    private float displayTime = 0;

    private void Start()
    {
        particles = transform.Find("Particle System").GetComponent<ParticleSystem>();
    }

    public void PutOutFire()
    {
        fireActive = false;
        particles.Stop();
    }

    private void LateUpdate()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > particles.main.duration + 1)
        {
            Destroy(gameObject);
        }

        if ((timeElapsed - displayTime) > 1f / fps)
        {
            displayTime = timeElapsed;
            particles.Simulate(0.15f, true, false, false);
            particles.Pause();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (fireActive)
        {
            Debug.Log(other.gameObject.name);
            if (other.transform.parent != null && other.transform.parent.GetComponent<EnemyAI_Base>() != null)
            {
                if (other.transform.parent.GetComponent<EnemyAI_Base>().heldBehavior.behaviorName == "water")
                {
                    PutOutFire();
                }
            }
            else if (other.transform.parent != null && other.transform.parent.gameObject.CompareTag("Player"))
            {
                if (other.transform.parent.Find("Meshes").childCount < 3)
                {
                    GameObject effect = Instantiate(fireEffect, other.transform.parent.Find("Meshes"));
                    effect.transform.position = other.transform.parent.Find("Meshes").position;
                }
                else
                {
                    FireDamageEffect damageEffect = other.transform.parent.Find("Meshes").GetComponentInChildren<FireDamageEffect>();
                    if (damageEffect != null)
                    {
                        damageEffect.fireLifetime = 4;
                    }
                    // deals more damage since entering fire while on fire?
                }
            }
        }
    }
}
