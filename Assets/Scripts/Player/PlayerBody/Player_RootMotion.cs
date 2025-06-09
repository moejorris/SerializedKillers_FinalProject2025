using UnityEngine;

public class Player_RootMotion : MonoBehaviour, IPlayerMover
{
    Player_MovementMachine machine;
    [SerializeField] RootMotion_Translator rootMotion;

    void Awake() => machine = GetComponent<Player_MovementMachine>();

    void OnEnable() => machine.AddMover(this);
    void OnDisable() => machine.RemoveMover(this);

    public Vector3 UpdateForce()
    {
        return rootMotion.RootMovement / machine.DeltaTime;
    }



}
