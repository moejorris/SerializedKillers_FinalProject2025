using System.Collections.Generic;
using UnityEngine;

//Joe Morris
public class Player_MovementMachine : MonoBehaviour
{

    [Header("Parameters")]
    [SerializeField] TimeStep timeStep = TimeStep.Update;

    enum GroundCheckMethod { Raycast, CapsuleCast, SphereCast }
    [SerializeField] GroundCheckMethod groundCheckMethod = GroundCheckMethod.Raycast;

    List<IPlayerMover> activeMovers = new List<IPlayerMover>();
    Vector3 _forwardDirection;
    RaycastHit _groundInfo;
    bool _grounded;
    float _movementMultiplier = 1f;


    enum TimeStep { Update, FixedUpdate }

    //Public Getters
    public Vector3 ForwardDirection { get => _forwardDirection.normalized; }
    public Vector3 RightDirection { get => GetRightVector(); }
    public RaycastHit GroundInformation { get => _groundInfo; }
    public float DeltaTime { get => currentDeltaTime(); }
    public bool isGrounded { get => _grounded; }
    public float MovementMultiplier { get => _movementMultiplier; }

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
                        movementToMake += forceToAdd * (mover.affectedByMultipliers ? _movementMultiplier : 1f);

                    }
                }
            }
        }
        else return;

        PlayerController.instance.CharacterController.Move(movementToMake * currentDeltaTime());
        CurrentMotion = movementToMake;

    }

    //Get Right Vector
    Vector3 GetRightVector()
    {
        return new Vector3(_forwardDirection.z, _forwardDirection.y, _forwardDirection.x);
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
        dir.y = 0;
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
        bool prevGrounded = _grounded;

        switch (groundCheckMethod)
        {

            case GroundCheckMethod.Raycast:
                _grounded = Physics.Raycast(transform.position, Vector3.down, out _groundInfo, 0.15f + PlayerController.instance.CharacterController.height / 2f, ~0, QueryTriggerInteraction.Ignore);
                break;

            case GroundCheckMethod.CapsuleCast:

                Vector3 heightDiff = Vector3.up * PlayerController.instance.CharacterController.height / 2f;
                Vector3 mod = PlayerController.instance.CharacterController.height * 0.95f * Vector3.up;

                _grounded = Physics.CapsuleCast
                (
                    transform.position - heightDiff + mod,
                    transform.position + heightDiff + mod,
                    PlayerController.instance.CharacterController.radius,
                    -Vector3.up,
                    out _groundInfo,
                    PlayerController.instance.CharacterController.height,
                    ~0,
                    QueryTriggerInteraction.Ignore
                );

                break;

            case GroundCheckMethod.SphereCast:

                _grounded = Physics.SphereCast(transform.position, PlayerController.instance.CharacterController.radius, Vector3.down, out _groundInfo, (PlayerController.instance.CharacterController.height / 2f) - PlayerController.instance.CharacterController.radius + 0.1f, ~0, QueryTriggerInteraction.Ignore);

                EnemyCheck();

                break;
        }

        if (prevGrounded != _grounded)
        {
            PlayerController.instance.Animation.UpdateGroundedStatus(_grounded);

            if (!_grounded)
            {
                PlayerController.instance.ChildMover.RemoveParent();
            }
        }

        if (_grounded && _groundInfo.collider.transform != PlayerController.instance.ChildMover.Parent)
        {
            PlayerController.instance.ChildMover.UpdateParent(_groundInfo.collider.transform);
        }
    }

    //Fall if standing on an Enemy
    void EnemyCheck()
    {
        if (_groundInfo.collider == null) return;

        if (_groundInfo.collider.GetComponent<EnemyAI_Base>() != null || _groundInfo.collider.transform.parent?.GetComponent<EnemyAI_Base>() != null)
        {
            _grounded = false;
            PlayerController.instance.ForceHandler.AddForce(Vector3.ProjectOnPlane((transform.position - _groundInfo.collider.transform.position).normalized, Vector3.up) * 100f, ForceMode.Acceleration);
        }
    }

    //Movement Multiplier When in Ice (or Water?)
    public void SetMovementMultiplier(float multiplier)
    {
        _movementMultiplier = multiplier;
    }

    public void RemoveMovementMultiplier()
    {
        _movementMultiplier = 1f;
    }

    void OnDrawGizmos()
    {
        if (groundCheckMethod == GroundCheckMethod.SphereCast)
        {
            if (_groundInfo.point != Vector3.zero)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_groundInfo.point, PlayerController.instance.CharacterController.radius);
            }
            else if (PlayerController.instance != null)
            {
                Gizmos.color = (Color.red + Color.white) / 2f;
                Gizmos.DrawWireSphere(transform.position - Vector3.up * PlayerController.instance.CharacterController.height / 2f, PlayerController.instance.CharacterController.radius);
            }
        }
    }
}

