using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScriptStealMenu : MonoBehaviour
{
    public InputActionReference northButton; // holding Y
    public InputActionReference leftJoystick;
    public PlayerInput playerInput;

    //public InputAction action;

    public GameObject menuPanel;

    public GameObject arrow;
    public bool menuOpen = false;
    public float slowTime = 1;

    public float offset = -10;

    public CharacterController characterController;

    private Image hud_combatSlotIcon;
    private Image hud_movementSlotIcon;
    private Image menu_combatSlotIcon;
    private Image menu_movementSlotIcon;
    private Image menu_replacementScriptSlotIcon;

    public Sprite currentSelectedEnemySprite;
    [SerializeField] private Sprite emptySprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hud_combatSlotIcon = transform.parent.Find("HUD/CombatSlot/Icon").GetComponent<Image>();
        hud_movementSlotIcon = transform.parent.Find("HUD/MovementSlot/Icon").GetComponent<Image>();
        menu_combatSlotIcon = transform.Find("BG/CombatSlot/Icon").GetComponent<Image>();
        menu_movementSlotIcon = transform.Find("BG/MovementSlot/Icon").GetComponent<Image>();
        menu_replacementScriptSlotIcon = transform.Find("BG/BottomMiddle/Icon").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (northButton.action.ReadValue<float>() > 0)
        {
            menuPanel.SetActive(true);
            menuOpen = true;
        }
        else
        {
            menuPanel.SetActive(false);
            menuOpen = false;
        }

        if (menuOpen)
        {
            slowTime -= Time.deltaTime * 2;
            if (slowTime < 0.1f)
            {
                slowTime= 0.1f;
            }

            if (InputIsKeyboard())
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else
        {
            slowTime += (Time.deltaTime * 4);
            if (slowTime > 1)
            {
                slowTime= 1;
            }

            Cursor.lockState = CursorLockMode.Locked;
        }

        Time.timeScale = slowTime;


        if (InputIsKeyboard())
        {
            arrow.transform.rotation = Quaternion.Euler(0,0, (Mathf.Atan2(Input.mousePosition.y - arrow.transform.position.y, Input.mousePosition.x - arrow.transform.position.x) * Mathf.Rad2Deg) - 90);


            Vector2 localPos = arrow.transform.Find("Image").transform.localPosition;
            localPos.y = Vector2.Distance(arrow.transform.position, Input.mousePosition);
            
            if (localPos.y > 180)
            {
                localPos.y = 180;
            }

            arrow.transform.Find("Image").transform.localPosition = localPos;
        }
        else
        {
            if (leftJoystick.action.ReadValue<Vector2>().x != 0 || leftJoystick.action.ReadValue<Vector2>().y != 0)
            {
                arrow.transform.eulerAngles = new Vector3(0, 0, (Mathf.Atan2(leftJoystick.action.ReadValue<Vector2>().y, leftJoystick.action.ReadValue<Vector2>().x) * Mathf.Rad2Deg) - 92);
            }

            Vector2 localPos = arrow.transform.Find("Image").transform.localPosition;
            localPos.y = leftJoystick.action.ReadValue<Vector2>().magnitude * 180f;
            arrow.transform.Find("Image").transform.localPosition = localPos;
        }


        if (currentSelectedEnemySprite != null)
        {
            menu_replacementScriptSlotIcon.sprite = currentSelectedEnemySprite;
        }
        else
        {
            menu_replacementScriptSlotIcon.sprite = emptySprite;
        }

        if (Vector2.Distance(arrow.transform.Find("Image").position, hud_movementSlotIcon.transform.position) > 1)
        {
            Debug.Log("In Range");
        }
    }

    public bool InputIsKeyboard()
    {
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    
}
