using System;
using UnityEngine;

//Joe Morris
public class Player_Gravity : MonoBehaviour, IPlayerMover
{
    [SerializeField] float acceleration = -10f;
    [SerializeField] float gravityScale = 1f;
    [SerializeField] float _currentGravity;
    public bool affectedByMultipliers = false;

    bool _groundedPreviousFrame; //if the character controller was grounded in the previous frame relative to this script;

    public float GravityAcceleration { get => acceleration * gravityScale; }
    public float CurrentGravity { get => _currentGravity;}
    public Action PlayerJustLanded; //jump script listens to this and when it goes off it resets the amount of jumps

    void OnEnable()
    {
        PlayerController.instance.MovementMachine.AddMover(this); //Add itself to the movement machine!
        _currentGravity = 0f;
    }
    void OnDisable() => PlayerController.instance.MovementMachine.RemoveMover(this); //remove itself from the movement machine when no longer active!

    public Vector3 UpdateForce() //update gravity
    {
        if (JustLanded()) //if the player just landed
        {
            PlayerJustLanded();
            _currentGravity = 0;
        }
        else if (!PlayerController.instance.MovementMachine.isGrounded) //enemies will be set as the ground, but Grounded is still false so we don't want to apply gravity if the player isn't falling
        {
            _currentGravity += acceleration * gravityScale * PlayerController.instance.MovementMachine.DeltaTime;
        }

        _groundedPreviousFrame = PlayerController.instance.MovementMachine.isGrounded;

        return Vector3.up * _currentGravity;
    }


    //Better Named Shortcuts
    //Consider making these public for animator use?
    public bool JustLanded()
    {
        return PlayerController.instance.MovementMachine.isGrounded && !_groundedPreviousFrame;
    }
    public bool JustLeftGround() => !PlayerController.instance.MovementMachine.isGrounded && _groundedPreviousFrame;

    //External Function Calls
    public void AddVerticalForce(float y)
    {
        if (PlayerController.instance.MovementMachine.isGrounded) _currentGravity = 0;

        _currentGravity += y;
    }

    public void OverrideVerticalForce(float y)
    {
        _currentGravity = y;
    }
}
