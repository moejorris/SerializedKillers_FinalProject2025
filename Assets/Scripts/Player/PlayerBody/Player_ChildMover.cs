using UnityEngine;

public class Player_ChildMover : MonoBehaviour, IPlayerMover
{
    Player_MovementMachine _machine => GetComponent<Player_MovementMachine>();
    public Transform Parent { get { return _parent; } }

    Transform _parent;
    Vector3 previousPosition;

    void OnEnable() => _machine.AddMover(this); //Add itself to the movement machine!

    void OnDisable() => _machine.RemoveMover(this); //remove itself from the movement machine when no longer active!

    public void UpdateParent(Transform _newParent)
    {
        _parent = _newParent;
        previousPosition = _parent.position;
    }

    public void RemoveParent()
    {
        _parent = null;
        previousPosition = Vector3.zero;
    }

    public Vector3 UpdateForce()
    {
        if (_parent != null && previousPosition != _parent.position && previousPosition != Vector3.zero)
        {
            Vector3 moveDelta = _parent.position - previousPosition;
            previousPosition = _parent.position;
            Debug.Log(moveDelta);
            return moveDelta / _machine.DeltaTime; //div by deltaTime because we already know how much it moved over time
        }
        else return Vector3.zero;
    }

}
