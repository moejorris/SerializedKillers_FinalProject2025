using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private SoundEffectSO doorOpenSFX;

    void Awake()
    {
        anim = GetComponent<Animator>();
        // anim.speed = 0.50f; // Set animator speed to 0.25 to slow down the animation
    }

    void OnTriggerEnter(Collider other)
    {
        var playerController = PlayerController.instance;
        if (playerController != null && other == playerController.Collider)
        {
            if (doorOpenSFX != null && !anim.GetBool("Open")) SoundManager.instance.PlaySoundEffectOnObject(doorOpenSFX, anim.transform);

            anim.SetBool("Open", true);
        }
    }
}
