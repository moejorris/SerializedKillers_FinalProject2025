using UnityEngine;

public class FireHazard : MonoBehaviour
{
    [SerializeField] private Behavior heldBehavior; // just to tell which type of element(?)
    [SerializeField] private GameObject fireEffect;
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
            else if (other.transform.parent != null && other.transform.parent.gameObject.CompareTag("Player"))
            {
                if (other.transform.Find("Meshes").childCount < 3)
                {
                    GameObject effect = Instantiate(fireEffect, other.transform.Find("Meshes"));
                    effect.transform.position = other.transform.Find("Meshes").position;
                }
                else
                {
                    // deals more damage since entering fire while on fire?
                }
            }
        }
    }
}
