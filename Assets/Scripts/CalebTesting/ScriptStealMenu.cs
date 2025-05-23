using UnityEngine;
using UnityEngine.InputSystem;

public class ScriptStealMenu : MonoBehaviour
{
    public InputActionReference northButton; // holding Y
    public InputActionReference leftJoystick;

    public GameObject menuPanel;

    public GameObject arrow;
    public bool menuOpen = false;
    public float slowTime = 1;

    public float offset = -10;

    public CharacterController characterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (northButton.action.ReadValue<float>() > 0)
        {
            menuPanel.SetActive(true);
            menuOpen = true;
            //characterController.enabled = false;
        }
        else
        {
            menuPanel.SetActive(false);
            menuOpen = false;
            //characterController.enabled = true;
        }

        if (menuOpen)
        {
            slowTime -= Time.deltaTime * 2;
            if (slowTime < 0.1f)
            {
                slowTime= 0.1f;
            }
        }
        else
        {
            slowTime += (Time.deltaTime * 4);
            if (slowTime > 1)
            {
                slowTime= 1;
            }
        }

        Time.timeScale = slowTime;

        if (leftJoystick.action.ReadValue<Vector2>().x != 0 || leftJoystick.action.ReadValue<Vector2>().y != 0)
        {
            arrow.transform.eulerAngles = new Vector3(0, 0, (Mathf.Atan2(leftJoystick.action.ReadValue<Vector2>().y, leftJoystick.action.ReadValue<Vector2>().x) * Mathf.Rad2Deg) + offset);
        }

        Vector2 localPos = arrow.transform.Find("Image").transform.localPosition;
        localPos.y = leftJoystick.action.ReadValue<Vector2>().magnitude * 180f;
        arrow.transform.Find("Image").transform.localPosition = localPos;
    }
}
