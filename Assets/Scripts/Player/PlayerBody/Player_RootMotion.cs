using UnityEngine;

public class Player_RootMotion : MonoBehaviour, IPlayerMover
{
    [SerializeField] RootMotion_Translator rootMotion;

    void OnEnable() => PlayerController.instance.MovementMachine.AddMover(this);
    void OnDisable() => PlayerController.instance.MovementMachine.RemoveMover(this);

    public Vector3 UpdateForce()
    {
        return rootMotion.RootMovement / PlayerController.instance.MovementMachine.DeltaTime;
    }



}
