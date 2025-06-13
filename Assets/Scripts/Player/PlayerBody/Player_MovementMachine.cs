using System.Collections.Generic;
using UnityEngine;

public class Player_MovementMachine : MonoBehaviour
{

    [Header("Parameters")]
    CharacterController controller => GetComponent<CharacterController>();
    [SerializeField] TimeStep timeStep = TimeStep.Update;

    enum GroundCheckMethod { Raycast, CapsuleCast, SphereCast}
    [SerializeField] GroundCheckMethod groundCheckMethod = GroundCheckMethod.Raycast;

    List<IPlayerMover> activeMovers = new List<IPlayerMover>();
    Vector3 _forwardDirection;
    RaycastHit _groundInfo;
    bool _grounded;
    enum TimeStep { Update, FixedUpdate }

    //Public Getters
    public Vector3 ForwardDirection { get => _forwardDirection.normalized; }
    public RaycastHit GroundInformation { get => _groundInfo; }
    public float DeltaTime { get => currentDeltaTime(); }
    public bool isGrounded { get => _grounded; }

    public Vector3 CurrentMotion;

    //Functions

    void Start() => _forwardDirection = transform.forward;

    void FixedUpdate() => UpdateMovement(TimeStep.FixedUpdate);
    void Update() => UpdateMovement(TimeStep.Update);

    void UpdateMovement(TimeStep loopTimeStep)
    {
        if (loopTimeStep != timeStep) return; //check if this instance should run based on whether or not this is in update/fixedUpdate and the matching timestep is selected

        GroundCheck();

        Vector3 movementToMake = Vector3.zero;

        if (activeMovers.Count > 0)
        {
            foreach (IPlayerMover mover in activeMovers)
            {
                if (((MonoBehaviour)mover).enabled)
                {
                    Vector3 forceToAdd = mover.UpdateForce();

                    if (forceToAdd.magnitude > 0.05f)
                    {
                        movementToMake += forceToAdd;                        

                    }
                }
            }
        }
        else return;

        controller.Move(movementToMake * currentDeltaTime());
        CurrentMotion = movementToMake;
    }

    //External Functions
    public void AddMover(IPlayerMover mover)
    {
        if (activeMovers.Contains(mover)) return;
        activeMovers.Add(mover);
    }

    public void RemoveMover(IPlayerMover mover)
    {
        // activeMovers.Remove(mover);
    }

    public void SetForwardDirection(Vector3 dir)
    {
        _forwardDirection = dir;
    }

    public void DisableAllMovers(IPlayerMover mover1 = null, IPlayerMover mover2 = null, IPlayerMover mover3 = null) //optional parameter to exclude up to 3 movers from being disabled.
    {
        foreach (IPlayerMover playerMover in activeMovers)
        {
            if (playerMover != mover1 && playerMover != mover2 && playerMover != mover3)
            {
                ((MonoBehaviour)playerMover).enabled = false;
            }
        }
    }

    public void EnableAllMovers()
    {
        IPlayerMover[] allMovers = GetComponents<IPlayerMover>();
        foreach (IPlayerMover playerMover in allMovers)
        {
            ((MonoBehaviour)playerMover).enabled = true;
        }
    }

    //Private Getters
    float currentDeltaTime()
    {
        return timeStep == TimeStep.Update ? Time.deltaTime : Time.fixedDeltaTime;
    }

    //Private Functions
    void GroundCheck()
    {
        switch (groundCheckMethod)
        {
            case GroundCheckMethod.Raycast:
                _grounded = Physics.Raycast(transform.position, Vector3.down, out _groundInfo, 0.15f + controller.height / 2f, ~0, QueryTriggerInteraction.Ignore);
                break;

            case GroundCheckMethod.CapsuleCast:

                Vector3 heightDiff = Vector3.up * controller.height / 2f;
                Vector3 mod = controller.height * 0.95f * Vector3.up;

                _grounded = Physics.CapsuleCast
                (
                    transform.position - heightDiff + mod,
                    transform.position + heightDiff + mod,
                    controller.radius,
                    -Vector3.up,
                    out _groundInfo,
                    controller.height,
                    ~0,
                    QueryTriggerInteraction.Ignore
                );

                break;

            case GroundCheckMethod.SphereCast:

                _grounded = Physics.SphereCast(transform.position, controller.radius, Vector3.down, out _groundInfo, controller.height / 2f, ~0, QueryTriggerInteraction.Ignore);

                break;

        }
    }

    void OnDrawGizmos()
    {
        if(groundCheckMethod == GroundCheckMethod.SphereCast)
        {
            if (_groundInfo.point != Vector3.zero)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_groundInfo.point, controller.radius);
            }
            else
            {
                Gizmos.color = (Color.red + Color.white) / 2f;
                Gizmos.DrawWireSphere(transform.position - Vector3.up * controller.height / 2f, controller.radius);
            }
        }
    }
}
