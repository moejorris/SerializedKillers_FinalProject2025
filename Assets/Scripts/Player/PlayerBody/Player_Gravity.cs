using UnityEngine;

public class Player_Gravity : MonoBehaviour, IPlayerMover
{
    Player_MovementMachine _machine => GetComponent<Player_MovementMachine>();
    [SerializeField] float acceleration = -10f;
    [SerializeField] float gravityScale = 1f;
    [SerializeField] float gravityOnGround = -5f;
    [SerializeField] float _currentGravity;

    bool _groundedPreviousFrame; //if the character controller was grounded in the previous frame relative to this script;

    public float GravityAcceleration { get => acceleration * gravityScale; }
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
            Debug.Log("JustLanded");
            _currentGravity = gravityOnGround;
        }
        else if (JustLeftGround()) //if the player just left the ground
        {
            // _currentGravity = 0f;
        }
        else if(!_machine.isGrounded)
        {
            _currentGravity += acceleration * gravityScale * _machine.DeltaTime;
        }

        _groundedPreviousFrame = _machine.isGrounded;

        return Vector3.up * _currentGravity;
    }


    //Better Named Shortcuts
    //Consider making these public for animator use?
    bool JustLanded() => _machine.isGrounded && !_groundedPreviousFrame;
    bool JustLeftGround() => !_machine.isGrounded && _groundedPreviousFrame;

    //External Function Calls
    public void AddVerticalForce(float y)
    {
        if (_machine.isGrounded) _currentGravity = 0;

        _currentGravity += y;
    }
}
