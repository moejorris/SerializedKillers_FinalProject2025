using UnityEngine;

public class Activate : MonoBehaviour, IDamageable
{
    private Animator anim;
    [Header("Puzzle Settings")]
    [SerializeField] private string requiredBehaviorName;
    [SerializeField] private GameObject particleEffect;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.P))
        {
            anim.SetTrigger("Activate"); // Trigger the activation animationif (particleEffect != null)
            {
                ParticleSystem ps = particleEffect.GetComponent<ParticleSystem>();
                if (ps != null) ps.Stop();
            }
            Destroy(particleEffect, 0.5f);

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }

    public void TakeDamage(float damage = 0)
    {
        if (PlayerController.instance.ScriptSteal.GetHeldHebavior() != null && PlayerController.instance.ScriptSteal.GetHeldHebavior().behaviorName == requiredBehaviorName)
        {
            anim.SetTrigger("Activate"); // Trigger the activation animation
            Debug.Log("Puzzle activated with behavior: " + requiredBehaviorName);

            if (particleEffect != null)
            {
                ParticleSystem ps = particleEffect.GetComponent<ParticleSystem>();
                if (ps != null) ps.Stop();
            }
            Destroy(particleEffect, 0.5f);

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
        else
        {
            string heldBehaviorName = "none";
            if (PlayerController.instance.ScriptSteal.GetHeldHebavior() != null) heldBehaviorName = PlayerController.instance.ScriptSteal.GetHeldHebavior().behaviorName;
            Debug.Log("Puzzle activation failed. Required behavior: " + requiredBehaviorName + ", but held behavior: " + heldBehaviorName);
        }
    }
}