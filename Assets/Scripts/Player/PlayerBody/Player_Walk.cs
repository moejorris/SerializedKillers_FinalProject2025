using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Walk : MonoBehaviour, IPlayerMover
{
    [SerializeField] InputActionReference walkInput;
    [SerializeField] float speed = 7f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float deccerlation = 30f;
    [SerializeField] AnimationCurve speedUpCurve;
    Player_MovementMachine _machine;
    [SerializeField] Vector3 _walkVelocity;

    void Awake() => _machine = GetComponent<Player_MovementMachine>(); //Get reference to the player movement machine!
    void OnEnable()
    {
        _machine.AddMover(this); //Add itself to the movement machine!
        _walkVelocity = Vector3.zero;
    }

    void OnDisable() 
    {
        _machine.RemoveMover(this); //remove itself from the movement machine when no longer active!
        _walkVelocity = Vector3.zero;
    }

    public Vector3 UpdateForce()
    {
        return UpdateWalkVector();
    }

    Vector3 UpdateWalkVector()
    {
        Vector3 moveDir = IntendedMoveDirection();
        Vector3 currentWalkVector = _walkVelocity;
        float currentSpeed = currentWalkVector.magnitude;

        float lerpSpeed = currentSpeed <= speed * moveDir.magnitude ? acceleration : deccerlation;

        float newWalkSpeed = currentWalkVector.magnitude;

        if (enabled)
        {
            //Speed Change
            float walkCurveValue = Mathf.Clamp(speedUpCurve.Evaluate(GetNormalizedSpeed()), 0.1f, 1f);
            newWalkSpeed = Vector3.Slerp(currentWalkVector, moveDir * speed, walkCurveValue * _machine.DeltaTime * lerpSpeed).magnitude;
            if (newWalkSpeed < 0.1f) return Vector3.zero;

        }

        //Velocity Vector Change
        return _walkVelocity = Vector3.ProjectOnPlane(_machine.ForwardDirection, _machine.GroundInformation.normal).normalized * newWalkSpeed;
    }

    Vector3 IntendedMoveDirection()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);

        Vector2 moveInput = walkInput.action.ReadValue<Vector2>();
        return moveInput.x * camRight.normalized + moveInput.y * camForward.normalized;
    }

    public float GetMaxSpeed()
    {
        return speed;
    }

    public float GetNormalizedSpeed()
    {
        return _walkVelocity.magnitude / speed;
    }

    public float GetCurrentSpeed()
    {
        return _walkVelocity.magnitude;
    }
}
