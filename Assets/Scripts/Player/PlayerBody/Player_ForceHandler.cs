using UnityEngine;

//Joe Morris

public class Player_ForceHandler : MonoBehaviour, IPlayerMover
{
    //if the below methods for get component works, update other scripts to use this!
    Player_Gravity _gravity => GetComponent<Player_Gravity>();
    Player_MovementMachine _machine => GetComponent<Player_MovementMachine>();

    [SerializeField] float mass = 2;
    [SerializeField] float groundedDrag = 5f;
    [SerializeField] float airDrag = 3f;

    [SerializeField] Vector3 forceCurrent;

    public enum OverrideMode {None, OnlyChanged, All}

    void OnEnable() => _machine.AddMover(this); //Add itself to the movement machine!
    void OnDisable() => _machine.RemoveMover(this); //remove itself from the movement machine when no longer active!

    public void AddForce(Vector3 forceToAdd, ForceMode forceMode = ForceMode.VelocityChange, OverrideMode overrideMode = OverrideMode.None)
    {
        switch (forceMode)
        {
            case ForceMode.Impulse: //Instant force applied to player, using mass.
                forceToAdd /= mass;
                break;

            case ForceMode.VelocityChange:
                //do nothing to the force, VC ignores mass and delta time
                break;

            case ForceMode.Force: //Applies over time, using mass
                forceToAdd /= mass;
                forceToAdd *= _machine.DeltaTime;
                break;

            case ForceMode.Acceleration: //Applies over time, ignoring mass.
                forceToAdd *= _machine.DeltaTime;
                break;
        }

        switch (overrideMode)
        {
            case OverrideMode.All: //overrides all current force, setting them to the input.
                Debug.Log(forceToAdd.y);
                forceCurrent = forceToAdd;
            break;

            case OverrideMode.OnlyChanged: //only changes the axes that force is being applied. Ex: Vector3 (0, 10, 0) the Y axis will be set to 10, but x and z will be left alone.
                if (forceToAdd.y != 0)
                {
                    if (_gravity)
                    {
                        _gravity.OverrideVerticalForce(forceToAdd.y);
                    }
                    else
                    {
                        forceCurrent.y = forceToAdd.y;
                    }
                }
                if (forceToAdd.x != 0) forceCurrent.x = forceToAdd.x;
                if (forceToAdd.z != 0) forceCurrent.z = forceToAdd.z;
            break;

            case OverrideMode.None: //adds force normally.
                if (_gravity)
                {
                    _gravity.AddVerticalForce(forceToAdd.y);
                }
                else
                {
                    forceCurrent.y += forceToAdd.y;
                }

                forceCurrent.x += forceToAdd.x;
                forceCurrent.z += forceToAdd.z;
            break;
        }
    }

    public Vector3 UpdateForce()
    {
        float useDrag = _machine.isGrounded ? groundedDrag : airDrag;
        if (forceCurrent.magnitude > 0.05f) forceCurrent = Vector3.Lerp(forceCurrent, Vector3.zero, useDrag * _machine.DeltaTime);
        else forceCurrent = Vector3.zero;

        return forceCurrent;
    }
}
