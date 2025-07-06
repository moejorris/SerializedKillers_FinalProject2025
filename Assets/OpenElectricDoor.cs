using UnityEngine;

public class OpenElectricDoor : MonoBehaviour, IElemental
{
    [Header("Door Settings")]
    [Tooltip("Refrence to room script")]
    [SerializeField] private Room room;
    [Tooltip("Particle effect for blocked door")]
    [SerializeField] private ParticleSystem electricDoor;

    public void InteractElement(Behavior behavior)
    {
        if (behavior == null) return;
        if (behavior.behaviorName == "electric")
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
