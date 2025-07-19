using UnityEngine;

public class Lever : MonoBehaviour, IDamageable
{
    [SerializeField] private Animator[] gatesToOpen;
    [SerializeField] private Collider[] invincibleAreas;
    private Animator leverAnimator => GetComponent<Animator>();
    private bool canInteract = true;
    private InteractPopup interactPopup;
    [SerializeField] private bool startDisabled = false;
    private void Start()
    {
        if (transform.Find("LeverPopup") != null)
        {
            interactPopup = transform.Find("LeverPopup").GetComponent<InteractPopup>();
        }

        if (startDisabled)
        {
            canInteract = false;
            if (interactPopup != null) interactPopup.active = false;
        }
    }

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

    public void EnableLever()
    {
        canInteract = true;
        if (interactPopup != null) interactPopup.active = true;
    }

    public void ResetInteract()
    {
        canInteract = true;
    }
}
