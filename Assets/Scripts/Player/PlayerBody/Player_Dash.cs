using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Dash : MonoBehaviour, IPlayerMover
{
    Player_MovementMachine _machine;
    Player_Walk walk;
    Player_Rotate rotate;
    [SerializeField] InputActionReference dashInput;
    [SerializeField] InputActionReference walkInput;
    [SerializeField] float _distance = 4f;
    [SerializeField] float _travelTime = 0.25f;
    Vector3 _force;
    bool _isDashing = false;

    void Awake()
    {
        _machine = GetComponent<Player_MovementMachine>();
        walk = GetComponent<Player_Walk>();
        rotate = GetComponent<Player_Rotate>();
    }
    void OnEnable() => _machine.AddMover(this); //Add itself to the movement machine!
    void OnDisable() => _machine.RemoveMover(this); //remove itself from the movement machine when no longer active!

    void Update()
    {
        if (!_isDashing && dashInput.action.WasPerformedThisFrame())
        {
            StartCoroutine("Dash");
        }
    }

    public Vector3 UpdateForce()
    {
        return _force;
    }

    IEnumerator Dash() //Fix distance not being reflected accurately in game!!!
    {
        _isDashing = true;

        _machine.SetForwardDirection(IntendedMoveDirection()); //Snaps player to the direction they are trying to dash in so they dash in the correct direction

        Vector3 destination = transform.position + _machine.ForwardDirection.normalized * _distance;
        Vector3 startPosition = transform.position;

        destination.y = 0;
        startPosition.y = 0;

        UpdateOtherMoveComponents(); //disables walking and turning. TODO: Disable jumping once that's done

        float t = 0;

        while (t <= _travelTime)
        {
            _force = Vector3.Lerp(startPosition, destination, t / _travelTime) - startPosition; // - startPosition;

            _force = Vector3.ProjectOnPlane(_force, _machine.GroundInformation.normal);

            yield return new WaitForSeconds(_machine.DeltaTime);

            t += _machine.DeltaTime;
        }

        _force = Vector3.zero;

        _isDashing = false;

        UpdateOtherMoveComponents(); //re-enables walking and turning
    }

    void UpdateOtherMoveComponents()
    {
        walk.enabled = !_isDashing;
        rotate.enabled = !_isDashing;
    }

    Vector3 IntendedMoveDirection()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);

        Vector2 moveInput = walkInput.action.ReadValue<Vector2>();
        return moveInput.x * camRight.normalized + moveInput.y * camForward.normalized;
    }

}
