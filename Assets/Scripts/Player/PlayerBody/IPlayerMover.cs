using UnityEngine;

public interface IPlayerMover
{
    public bool affectedByMultipliers { get {return true;}}
    public Vector3 UpdateForce()
    {
        return Vector3.zero;
    }
}
