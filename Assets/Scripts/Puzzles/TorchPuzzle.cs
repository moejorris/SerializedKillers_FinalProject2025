using UnityEngine;

public class TorchPuzzle : MonoBehaviour, IElemental
{
    [Header("Torch Settings")]
    [Tooltip("The amount of time it takes for the fire to extinguish in seconds")]
    [SerializeField] private float extinguishTime = 60f;
    private float extinguishTimer = 0;
    private bool isLit = true; // Insert Travis Scott refrence here
    public bool puzzleComplete = false;
    [SerializeField] private GameObject parent;


    private ParticleSystem fire;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fire = transform.Find("Fire").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // if puzzle is complete do nothing
        if (puzzleComplete) return;
        if (isLit)
        {
            extinguishTimer += Time.deltaTime;
            if (extinguishTimer >= extinguishTime)
            {
                extinguishTimer = 0;
                ExntinguishFire();
            }
        }
    }

    public void InteractElement(Behavior behavior)
    {
        if (behavior == null) return;
        if (behavior.behaviorName == "fire")
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        LightFire();
    }

    void LightFire()
    {
        isLit = true;
        fire.Play();
        parent.GetComponent<Activate>().AddTorch();
        Debug.Log("ITS LIT!");
    }

    void ExntinguishFire()
    {
        isLit = false;
        fire.Stop();
        parent.GetComponent<Activate>().RemoveTorch();
        Debug.Log("Fire Extinguished");
    }
}
