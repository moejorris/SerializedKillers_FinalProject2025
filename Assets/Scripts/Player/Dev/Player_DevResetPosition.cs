using UnityEngine;

public class Player_DevResetPosition : MonoBehaviour
{
    [SerializeField] KeyCode respawnKey = KeyCode.R;

    Vector3 respawnPoint;
    Vector3 respawnDirection;

    void Start()
    {
        respawnPoint = transform.position;
        respawnDirection = transform.forward;
    }

    void Update()
    {
        if (Input.GetKeyDown(respawnKey))
        {
            PlayerController.instance.CharacterController.enabled = false;

            transform.position = respawnPoint;
            PlayerController.instance.MovementMachine.SetForwardDirection(respawnDirection);

            PlayerController.instance.CharacterController.enabled = true;
        }
    }
}
