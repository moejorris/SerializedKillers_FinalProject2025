using UnityEngine;

public class Joe_MovingPlatformTriggerCollider : MonoBehaviour
{
    Joe_MovingPlatform platformMover => GetComponentInParent<Joe_MovingPlatform>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && platformMover != null)
        {
            platformMover.StartMoving();
        }   
    }
}
