using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private Animator anim;

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
            anim.SetBool("Open", true);
        }
    }
}
