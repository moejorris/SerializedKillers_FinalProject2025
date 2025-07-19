using UnityEngine;

public class OpenElectricDoor : MonoBehaviour, IElemental
{
    [Header("Door Settings")]
    [Tooltip("Refrence to room script")]
    [SerializeField] private Room room;
    [Tooltip("Particle effect for blocked door")]
    [SerializeField] private ParticleSystem electricDoor;

    [SerializeField] private Material glowingMat;
    [SerializeField] private Renderer[] cords;

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
        foreach (Renderer meshy in cords)
        {
            meshy.material = glowingMat;
        }
    }

    void UnblockDoor()
    {
        electricDoor.Stop();
        room.RoomComplete();
        room.OpenDoors(room.exitDoors);
    }



}
