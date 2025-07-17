using UnityEngine;

public class Player_Respawn : MonoBehaviour
{
    public Vector3 respawnPoint;
    public Room currentRoom;

    void Start()
    {
        if (respawnPoint != Vector3.zero) return;
        respawnPoint = transform.position;   
    }

    public void Respawn()
    {
        if (respawnPoint == null) return;

        //Reset Health
        PlayerController.instance.Health.ResetHealth();
        PlayerController.instance.Health.UpdatePlayerHealth();
        PlayerController.instance.ScriptSteal.ReturnScript();
        PlayerController.instance.Mana.UseMana(100);
        PlayerController.instance.Mana.ToggleMana();
        transform.Find("FireStatusEffect").GetComponent<FireDamageEffect>().StopFire();

        if (currentRoom != null && !currentRoom.roomCompleted) currentRoom.ResetRoom(); // newly added, hopefully won't be jank

        //Change Position
        
        TeleportPlayer(respawnPoint);

        //Remove Script / Status Effects
        PlayerController.instance.ScriptSteal.ReturnScript();
    }

    public void TeleportPlayer(Vector3 _respawnPoint)
    {
        GetComponent<CharacterController>().enabled = false;
        transform.position = _respawnPoint;
        GetComponent<CharacterController>().enabled = true;
    }
}
