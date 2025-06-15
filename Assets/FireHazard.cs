using UnityEngine;

public class FireHazard : MonoBehaviour
{
    public Behavior heldBehavior; // just to tell which type of element(?)
    private bool fireActive = true;
    private ParticleSystem particles;

    private void Start()
    {
        particles = transform.Find("Particle System").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (particles == null)
        {
            Destroy(gameObject);
        }
    }

    public void PutOutFire()
    {
        fireActive = false;
        particles.Stop();
    }

    private void OnTriggerEnter(Collider other)
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
        }
    }
}
