using UnityEngine;

public class Player_Respawn : MonoBehaviour
{
    public Vector3 respawnPoint;

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
        transform.Find("FireStatusEffect").GetComponent<FireDamageEffect>().StopFire();

        //Change Position
        GetComponent<CharacterController>().enabled = false;
        transform.position = respawnPoint;
        GetComponent<CharacterController>().enabled = true;

        //Remove Script / Status Effects
        PlayerController.instance.ScriptSteal.ReturnScript();
    }
}
