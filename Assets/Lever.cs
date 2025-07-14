using UnityEngine;

public class Lever : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator[] gatesToOpen;
    [SerializeField] private Collider[] invincibleAreas;
    private Animator leverAnimator => GetComponent<Animator>();
    private bool canInteract = true;

    public void TakeDamage(float damage = 0)
    {
        if (canInteract)
        {
            canInteract = false;

            leverAnimator.SetBool("On", !leverAnimator.GetBool("On"));

            foreach (Animator animator in gatesToOpen)
            {
                animator.SetBool("Open", leverAnimator.GetBool("On"));
            }

            foreach (Collider col in invincibleAreas)
            {
                col.gameObject.SetActive(!leverAnimator.GetBool("On"));
            }

            Invoke("ResetInteract", 1);
        }
    }

    public void ResetInteract()
    {
        canInteract = true;
    }
}
