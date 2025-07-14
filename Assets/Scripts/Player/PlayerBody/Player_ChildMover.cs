using UnityEngine;

public class Player_ChildMover : MonoBehaviour, IPlayerMover
{
    public Transform Parent { get { return _parent; } }

    Transform _parent;
    Vector3 previousPosition;

    [SerializeField] Vector3 force;

    void OnEnable() => PlayerController.instance.MovementMachine.AddMover(this); //Add itself to the movement machine!

    void OnDisable() => PlayerController.instance.MovementMachine.RemoveMover(this); //remove itself from the movement machine when no longer active!

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

    public Transform GetParent()
    {
        return _parent;
    }

    public Vector3 UpdateForce()
    {
        if (_parent != null && previousPosition != _parent.position && previousPosition != Vector3.zero)
        {
            Vector3 moveDelta = _parent.position - previousPosition;
            previousPosition = _parent.position;
            Debug.Log(moveDelta);
            return force = moveDelta / PlayerController.instance.MovementMachine.DeltaTime; //div by deltaTime because we already know how much it moved over time
        }
        else return Vector3.zero;
    }

}
