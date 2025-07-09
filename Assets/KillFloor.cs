using UnityEngine;

public class KillFloor : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.GetComponent<Player_Respawn>() != null)
        {
            PlayerController.instance.Health.TakeDamage(100);
        }
    }
}
