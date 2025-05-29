using UnityEngine;

public class Player_Gravity : MonoBehaviour, IPlayerMover
{
    Player_MovementMachine _machine;
    [SerializeField] float acceleration = -10f;
    [SerializeField] float gravityScale = 1f;
    [SerializeField] float gravityOnGround = -5f;
    float _currentGravity;

    bool _groundedPreviousFrame; //if the character controller was grounded in the previous frame;

    public Vector3 Force { get; }

    void Awake() => _machine = GetComponent<Player_MovementMachine>();
    void OnEnable()
    {
        _machine.AddMover(this); //Add itself to the movement machine!
        _currentGravity = 0f;
    }  
    void OnDisable() => _machine.RemoveMover(this); //remove itself from the movement machine when no longer active!

    public Vector3 UpdateForce() //update gravity
    {
        if (JustLanded()) //if the player just landed
        {
            _currentGravity = gravityOnGround;
        }
        else if (JustLeftGround()) //if the player just left the ground
        {
            _currentGravity = 0f;
        }
        else
        {
            _currentGravity += acceleration * gravityScale * _machine.DeltaTime;
        }

        return Vector3.up * _currentGravity;
    }


    //Better Named Shortcuts
    //Consider making these public for animator use?
    bool JustLanded() => _machine.isGrounded && !_groundedPreviousFrame;
    bool JustLeftGround() => !_machine.isGrounded && _groundedPreviousFrame;
}
