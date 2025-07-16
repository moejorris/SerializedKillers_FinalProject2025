using UnityEngine;

public class RespawnPointTrigger : MonoBehaviour
{
    public bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other == PlayerController.instance.Collider && !used)
        {
            used = true;
            PlayerController.instance.Respawn.respawnPoint = transform.position;
        }
    }
}
