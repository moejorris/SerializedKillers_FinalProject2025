using UnityEngine;

public class ResetPlayerPos : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = new Vector3(0, 1, 0); // Reset player position to origin
        }
        else
        {
            Destroy(other); // Destroy the game object if it is not the player
        }
    }
}
