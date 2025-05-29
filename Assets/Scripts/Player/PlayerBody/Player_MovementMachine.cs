using System.Collections.Generic;
using UnityEngine;

public class Player_MovementMachine : MonoBehaviour
{
    [SerializeField] TimeStep timeStep = TimeStep.Update;
    [SerializeField] List<IPlayerMover> activeMovers = new List<IPlayerMover>();
    [SerializeField] CharacterController controller;
    Vector3 _forwardDirection;
    RaycastHit _groundInfo;
    bool _grounded;

    enum TimeStep { Update, FixedUpdate }

    //Public Getters
    public Vector3 ForwardDirection { get => _forwardDirection; }
    public RaycastHit GroundInformation { get => _groundInfo; }
    public float DeltaTime { get => currentDeltaTime(); }
    public bool isGrounded { get => _grounded; }

    //Functions
    void Awake() => controller = GetComponent<CharacterController>();

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
                movementToMake += mover.UpdateForce();
            }
        }
        else return;

        controller.Move(movementToMake * currentDeltaTime());
    }




    //External Functions
    public void AddMover(IPlayerMover mover)
    {
        activeMovers.Add(mover);
    }

    public void RemoveMover(IPlayerMover mover)
    {
        activeMovers.Remove(mover);
    }

    public void SetForwardDirection(Vector3 dir)
    {
        _forwardDirection = dir;
    }

    //Private Getters
    float currentDeltaTime()
    {
        return timeStep == TimeStep.Update ? Time.deltaTime : Time.fixedDeltaTime;
    }

    //Private Functions
    void GroundCheck()
    {
        _grounded = Physics.Raycast(transform.position, Vector3.down, out _groundInfo, 0.15f + controller.height / 2f, ~0, QueryTriggerInteraction.Ignore);
    }
}
