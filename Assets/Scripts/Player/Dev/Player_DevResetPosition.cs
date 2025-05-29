using UnityEngine;

public class Player_DevResetPosition : MonoBehaviour
{
    CharacterController controller;
    [SerializeField] KeyCode respawnKey = KeyCode.R;

    Vector3 respawnPoint;
    Vector3 respawnDirection;
    void Awake() => controller = GetComponent<CharacterController>();

    void Start()
    {
        respawnPoint = transform.position;
        respawnDirection = transform.forward;
    }

    void Update()
    {
        if (Input.GetKeyDown(respawnKey))
        {
            controller.enabled = false;

            transform.position = respawnPoint;
            GetComponent<Player_MovementMachine>().SetForwardDirection(respawnDirection);

            controller.enabled = true;
        }
    }
}
