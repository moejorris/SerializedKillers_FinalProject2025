using UnityEngine;

public class TutorialRoom : PuzzleRoom
{
    public int firesStarted = 0;
    
    public void StartFire()
    {
        firesStarted++;
        if (firesStarted >= 2)
        {
            RoomComplete();
        }
    }
}
