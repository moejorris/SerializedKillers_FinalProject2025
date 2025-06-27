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

    public void TakeDamage(float damage = 0, Player_ScriptSteal scriptSteal = null)
    {
        if (scriptSteal != null && scriptSteal.HeldBehavior != null && scriptSteal.HeldBehavior.behaviorName == requiredBehaviorName)
        {
            anim.SetTrigger("Activate"); // Trigger the activation animation
            Debug.Log("Puzzle activated with behavior: " + requiredBehaviorName);

            if (particleEffect != null)
            {
                ParticleSystem ps = particleEffect.GetComponent<ParticleSystem>();
                if (ps != null) ps.Stop();
            }

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
        else
        {
            string heldBehaviorName = "none";
            if (scriptSteal != null && scriptSteal.HeldBehavior != null) heldBehaviorName = scriptSteal.HeldBehavior.behaviorName;
            Debug.Log("Puzzle activation failed. Required behavior: " + requiredBehaviorName + ", but held behavior: " + heldBehaviorName);
        }
    }
}