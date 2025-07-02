using UnityEngine;

public class Player_Respawn : MonoBehaviour
{
    public Vector3 respawnPoint;
    public Player_HealthComponent health => GetComponent<Player_HealthComponent>();

    public void Respawn()
    {
        if (respawnPoint == null) return;

        //Reset Health
        health.ResetHealth();
        health.UpdatePlayerHealth();

        //Change Position
        GetComponent<CharacterController>().enabled = false;
        transform.position = respawnPoint;
        GetComponent<CharacterController>().enabled = true;

        //Remove Script / Status Effects
        GetComponent<Player_ScriptSteal>().ReturnScript();
    }
}
