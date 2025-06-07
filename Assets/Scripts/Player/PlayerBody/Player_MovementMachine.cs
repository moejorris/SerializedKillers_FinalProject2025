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

    public Vector3 CurrentMotion;

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
                if (((MonoBehaviour)mover).enabled)
                {
                    movementToMake += mover.UpdateForce();
                }
            }
        }
        else return;

        controller.Move(movementToMake * currentDeltaTime());
        CurrentMotion = movementToMake;        
    }

    void OnGUI()
    {
        GUI.Label(new Rect(25, 25, 100, 20), CurrentMotion.ToString());
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
        _grounded = Physics.Raycast(transform.position, Vector3.down, out _groundInfo, 0.15f + controller.height / 2f, ~0, QueryTriggerInteraction.Ignore);
    }
}
