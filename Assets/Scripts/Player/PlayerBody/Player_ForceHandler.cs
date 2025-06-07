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

    void OnEnable() => _machine.AddMover(this); //Add itself to the movement machine!
    void OnDisable() => _machine.RemoveMover(this); //remove itself from the movement machine when no longer active!


    public void AddForce(Vector3 forceToAdd, ForceMode forceMode = ForceMode.VelocityChange)
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

        //if gravity is present, apply vertical force there because gravity acts upon it. This could be reverse where gravity affects this script, but gravity was coded first so for now I'll leave it as is.
        if (_gravity != null) _gravity.AddVerticalForce(forceToAdd.y);
        else forceCurrent.y += forceToAdd.y;

        forceCurrent.x += forceToAdd.x;
        forceCurrent.z += forceToAdd.z;
    }

    public Vector3 UpdateForce()
    {
        float useDrag = _machine.isGrounded ? groundedDrag : airDrag;
        Debug.Log("Updating Force");
        if (forceCurrent.magnitude > 0.05f) forceCurrent = Vector3.Lerp(forceCurrent, Vector3.zero, useDrag * _machine.DeltaTime);
        else forceCurrent = Vector3.zero;

        return forceCurrent;
    }
}
