using UnityEngine;

public class PuzzleRoom : Room
{
    public override void Start()
    {
        base.Start();
        OpenDoors(entranceDoors);
    }

    public override void RoomComplete()
    {
        base.RoomComplete();
        OpenDoors(exitDoors);
    }

    public override void BeginChallenge()
    {
        base.BeginChallenge();
        CloseDoors(entranceDoors);
    }
}
