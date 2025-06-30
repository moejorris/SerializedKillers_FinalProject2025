using UnityEngine;

public class Joe_MovingPlatformTriggerCollider : MonoBehaviour
{
    Joe_MovingPlatform platformMover => GetComponentInParent<Joe_MovingPlatform>();

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player_MovementMachine>() && platformMover != null)
        {
            platformMover.StartMoving();
        }   
    }
}
