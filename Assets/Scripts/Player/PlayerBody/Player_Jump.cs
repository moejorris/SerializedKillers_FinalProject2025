using System.Net.Security;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player_Jump : MonoBehaviour
{
    [SerializeField] InputActionReference jumpInput;
    Player_MovementMachine _machine => GetComponent<Player_MovementMachine>();
    Player_ForceHandler _forceHandler => GetComponent<Player_ForceHandler>();
    Player_Gravity _gravity => GetComponent<Player_Gravity>();

    UnityEvent onLand;

    enum JumpMode { Force, Height };
    bool useFloorNormal = false;
    [Header("Normal Jump")]
    [SerializeField] JumpMode _jumpMode = JumpMode.Height;
    [SerializeField] float _value = 2;

    [Header("Double Jump/Air Jumps")]
    [SerializeField] int _airJumps = 1;
    [SerializeField] JumpMode _airJumpMode = JumpMode.Height;
    [SerializeField] float _airJumpValue = 1f;

    [SerializeField] int _curAirJumps = 0;

    // void Start()
    // {
    //     _curAirJumps = _airJumps;

    // }

    void OnEnable()
    {
        _gravity.PlayerJustLanded += ResetAirJumps;
    }

    void OnDisable()
    {
        _gravity.PlayerJustLanded -= ResetAirJumps;
    }

    void Update()
    {
        if (jumpInput.action.WasPressedThisFrame())
        {
            if (_machine.isGrounded)
            {
                Vector3 direction = useFloorNormal ? _machine.GroundInformation.normal : Vector3.up;
                ApplyJump(_value, _jumpMode, direction);
            }
            else if (_curAirJumps > 0 && !_machine.isGrounded)
            {
                _curAirJumps -= 1;
                ApplyJump(_airJumpValue, _airJumpMode);
            }

        }

        //Add gravity scalar upon jump input release? (player jumps less high depending on how long they hold the jump button)
    }

    void ApplyJump(float value, JumpMode jumpMode = JumpMode.Height, Vector3? direction = null)
    {
        if (direction == null) direction = Vector3.up; //roundabout way to assign default val to V3.up because it is not a compile time constant

        if (jumpMode == JumpMode.Height) //if the jump mode is set to height, perform calculation to obtain the force needed to reach Height.
        {
            value = Mathf.Sqrt(-2f * _gravity.GravityAcceleration * value);
        }

        _forceHandler.AddForce(direction.Value * value, ForceMode.VelocityChange);
    }

    public void ResetAirJumps()  { Debug.Log("Reset Jumps"); _curAirJumps = _airJumps;}
}
