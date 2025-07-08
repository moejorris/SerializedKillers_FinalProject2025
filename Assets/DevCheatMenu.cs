using UnityEngine;

public class DevCheatMenu : MonoBehaviour
{
    private GameObject menu => transform.Find("Menu").gameObject;
    [SerializeField] private Behavior[] behaviors;
    [SerializeField] private RespawnPointTrigger[] checkpoints;
    private Vector3 lastPoint = Vector3.zero;

    private void Start()
    {
        lastPoint = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            menu.SetActive(!menu.activeInHierarchy);
            if (menu.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void GiveScript(int script)
    {
        PlayerController.instance.ScriptSteal.ReturnScript();
        PlayerController.instance.ScriptSteal.StealScript(behaviors[script]);
    }

    public void GiveMana()
    {
        PlayerController.instance.Mana.GainMana(100);
    }

    public void Heal()
    {
        PlayerController.instance.Health.Heal(24);
    }

    public void CheckpointTeleport(int checkpoint)
    {
        if (checkpoints[checkpoint] != null)
        {
            lastPoint = GameObject.FindGameObjectWithTag("Player").transform.position;
            checkpoints[checkpoint].used = false;
            PlayerController.instance.Respawn.TeleportPlayer(checkpoints[checkpoint].transform.position);
        }
        else if (checkpoint == 000)
        {
            Vector3 tempPoint = GameObject.FindGameObjectWithTag("Player").transform.position;
            PlayerController.instance.Respawn.TeleportPlayer(lastPoint);
            lastPoint = tempPoint;
        }
    }
}
