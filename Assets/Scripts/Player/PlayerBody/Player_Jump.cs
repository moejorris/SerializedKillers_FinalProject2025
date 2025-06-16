using UnityEngine;
using UnityEngine.InputSystem;


//Joe Morris
public class Player_Jump : MonoBehaviour
{
    [SerializeField] InputActionReference jumpInput;
    Player_MovementMachine _machine => GetComponent<Player_MovementMachine>();
    Player_Animation _animations => GetComponent<Player_Animation>();
    Player_ForceHandler _forceHandler => GetComponent<Player_ForceHandler>();
    Player_Gravity _gravity => GetComponent<Player_Gravity>();

    enum JumpMode { Force, Height };
    bool useFloorNormal = false;

    [Header("Normal Jump")]
    [Tooltip("Whether you want to input the Height of how high the player should jump (in Unity Units) or how much upward force is applied to the player.")]
    [SerializeField] JumpMode _jumpMode = JumpMode.Height;
    [Tooltip("The value applied based on the Jump mode.")]
    [SerializeField] float _value = 2;
    [Tooltip("Whether or not the jump will be applied on top of the current velocity, or reset it.")]
    [SerializeField] bool overrideCurrentVelocity = true;

    [Header("Double Jump/Air Jumps")]
    [Tooltip("How many times the player can jump mid-air.")]
    [SerializeField] int _airJumps = 1;
    [Tooltip("Whether you want to input the Height of how high the player should jump (in Unity Units) or how much upward force is applied to the player.")]
    [SerializeField] JumpMode _airJumpMode = JumpMode.Height;
    [Tooltip("The value applied based on the Jump mode.")]
    [SerializeField] float _airJumpValue = 1f;
    [Tooltip("Whether or not the jump will be applied on top of the current velocity, or reset it.")]
    [SerializeField] bool overrideCurrentAirVelocity = true;

    [Header("GFX")]
    [SerializeField] GameObject jumpSmokeParticle;

    // [SerializeField]
    int _curAirJumps = 0;

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
                ApplyJump(_value, _jumpMode, overrideCurrentVelocity, direction);

                _animations?.PlayJumpAnimation();
            }
            else if (_curAirJumps > 0 && !_machine.isGrounded)
            {
                _curAirJumps -= 1;
                ApplyJump(_airJumpValue, _airJumpMode, overrideCurrentAirVelocity);

                _animations?.PlayAirJumpAnimation();
            }

        }

        //Add gravity scalar upon jump input release? (player jumps less high depending on how long they hold the jump button)
    }

    void ApplyJump(float value, JumpMode jumpMode = JumpMode.Height, bool overrideVelocity = false, Vector3? direction = null)
    {
        if (direction == null) direction = Vector3.up; //roundabout way to assign default val to V3.up because it is not a compile time constant

        if (jumpMode == JumpMode.Height) //if the jump mode is set to height, perform calculation to obtain the force needed to reach Height.
        {
            value = Mathf.Sqrt(-2f * _gravity.GravityAcceleration * value);
        }

        Player_ForceHandler.OverrideMode overrideMode = overrideVelocity ? Player_ForceHandler.OverrideMode.OnlyChanged : Player_ForceHandler.OverrideMode.None;

        _forceHandler.AddForce(direction.Value * value, ForceMode.VelocityChange, overrideMode);

        if (jumpSmokeParticle)
        {
            Instantiate(jumpSmokeParticle, transform.position - Vector3.up, Quaternion.Euler(Vector3.right * 90f));
        }
    }

    public void ResetAirJumps() => _curAirJumps = _airJumps;
}
