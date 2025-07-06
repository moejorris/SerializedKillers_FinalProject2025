using UnityEngine;

public class Activate : MonoBehaviour
{
    [Header("Activate Settings")]
    [SerializeField] private int requiredTorches = 6;
    private int torchCount = 0;
    [SerializeField] private Room room;
    [SerializeField] private GameObject[] torches;


    public void AddTorch()
    {
        torchCount++;
        if (torchCount >= requiredTorches)
        {
            if (room != null)
            {
                room.RoomComplete();
                room.OpenDoors(room.exitDoors);
            } 
            foreach (GameObject torch in torches)
            {
                torch.GetComponent<TorchPuzzle>().puzzleComplete = true;
            }
        }
    }

    public void RemoveTorch()
    {
        torchCount--;
    }

}