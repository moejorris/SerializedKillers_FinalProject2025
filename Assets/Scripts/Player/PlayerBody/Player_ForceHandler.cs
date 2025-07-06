using UnityEngine;

//Joe Morris

public class Player_ForceHandler : MonoBehaviour, IPlayerMover
{

    [SerializeField] float mass = 2;
    [SerializeField] float groundedDrag = 5f;
    [SerializeField] float airDrag = 3f;

    [SerializeField] Vector3 forceCurrent;

    public enum OverrideMode {None, OnlyChanged, All}

    void OnEnable() => PlayerController.instance.MovementMachine.AddMover(this); //Add itself to the movement machine!
    void OnDisable() => PlayerController.instance.MovementMachine.RemoveMover(this); //remove itself from the movement machine when no longer active!

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
                forceToAdd *= PlayerController.instance.MovementMachine.DeltaTime;
                break;

            case ForceMode.Acceleration: //Applies over time, ignoring mass.
                forceToAdd *= PlayerController.instance.MovementMachine.DeltaTime;
                break;
        }

        switch (overrideMode)
        {
            case OverrideMode.All: //overrides all current force, setting them to the input.
                forceCurrent = forceToAdd;
            break;

            case OverrideMode.OnlyChanged: //only changes the axes that force is being applied. Ex: Vector3 (0, 10, 0) the Y axis will be set to 10, but x and z will be left alone.
                if (forceToAdd.y != 0)
                {
                    if (PlayerController.instance.Gravity)
                    {
                        PlayerController.instance.Gravity.OverrideVerticalForce(forceToAdd.y);
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
                if (PlayerController.instance.Gravity)
                {
                    PlayerController.instance.Gravity.AddVerticalForce(forceToAdd.y);
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
        float useDrag = PlayerController.instance.MovementMachine.isGrounded ? groundedDrag : airDrag;
        if (forceCurrent.magnitude > 0.05f) forceCurrent = Vector3.Lerp(forceCurrent, Vector3.zero, useDrag * PlayerController.instance.MovementMachine.DeltaTime);
        else forceCurrent = Vector3.zero;

        return forceCurrent;
    }

    public void ResetVelocity()
    {
        forceCurrent = Vector3.zero;
    }
}
