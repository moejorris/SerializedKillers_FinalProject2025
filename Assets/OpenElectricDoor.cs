using UnityEngine;

public class OpenElectricDoor : MonoBehaviour, IElemental
{
    [Header("Door Settings")]
    [Tooltip("Refrence to room script")]
    [SerializeField] private Room room;
    [Tooltip("Particle effect for blocked door")]
    [SerializeField] private ParticleSystem electricDoor;
    [SerializeField] private Behavior requiredBehavior;

    public void InteractElement(Behavior behavior)
    {
        Debug.Log(behavior.behaviorName);
        if (behavior == null) return;

        if (behavior == requiredBehavior && PlayerController.instance.ScriptSteal.BehaviorActive())
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        UnblockDoor();
    }

    void UnblockDoor()
    {
        electricDoor.Stop();
        room.RoomComplete();
        room.OpenDoors(room.exitDoors);
    }



}
