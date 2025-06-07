using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Jump : MonoBehaviour
{
    [SerializeField] InputActionReference jumpInput;
    Player_MovementMachine _machine => GetComponent<Player_MovementMachine>();
    Player_ForceHandler _forceHandler => GetComponent<Player_ForceHandler>();
    Player_Gravity _gravity => GetComponent<Player_Gravity>();

    enum JumpMode { Force, Height };
    bool useFloorNormal = false;
    [SerializeField] JumpMode _jumpMode = JumpMode.Height;
    [SerializeField] float _value = 2;

    void Update()
    {
        if (jumpInput.action.WasPressedThisFrame() && _machine.isGrounded)
        {
            Vector3 direction = useFloorNormal ? _machine.GroundInformation.normal : Vector3.up;

            float forceMagnitude = 0;

            if (_jumpMode == JumpMode.Height)
            {
                forceMagnitude = Mathf.Sqrt(-2f * _gravity.GravityAcceleration * _value);
            }
            else
            {
                forceMagnitude = _value;
            }

            _forceHandler.AddForce(direction * forceMagnitude, ForceMode.VelocityChange);
        }

        //Add gravity scalar upon jump input release? (player jumps less high depending on how long they hold the jump button)
    }
}
