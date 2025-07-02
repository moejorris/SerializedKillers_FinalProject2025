using UnityEngine;

public class PuzzleRoom : Room
{
    public override void RoomComplete()
    {
        base.RoomComplete();
        OpenDoors(exitDoors);
    }
}
