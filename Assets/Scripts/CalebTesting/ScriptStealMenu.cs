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

    //private Image hud_combatSlotIcon;
    //private Image hud_movementSlotIcon;
    private BehaviorSlot menu_combatSlotIcon;
    private BehaviorSlot menu_movementSlotIcon;
    public BehaviorSlot menu_heldBehaviorSlot;

    private BehaviorSlot selectedBehaviorSlot;

    public Behavior targetedBehavior;
    [SerializeField] private Sprite emptySprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menu_combatSlotIcon = transform.Find("BG/CombatSlot").GetComponent<BehaviorSlot>();
        menu_movementSlotIcon = transform.Find("BG/MovementSlot").GetComponent<BehaviorSlot>();
        menu_heldBehaviorSlot = transform.Find("BG/BottomMiddle").GetComponent<BehaviorSlot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (northButton.action.ReadValue<float>() > 0) // holding Y
        {
            menuPanel.SetActive(true);
            menuOpen = true;
        }
        else // let go of Y
        {
            menuPanel.SetActive(false);
            menuOpen = false;
            ApplyBehaviorSelection();
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


        if (menuOpen)
        {
            if (Vector2.Distance(arrow.transform.Find("Image").position, menu_movementSlotIcon.transform.position) < 50) // in range of movement slot
            {
                MovementSlotSelection();
                menu_combatSlotIcon.UpdateSlot();
            }
            else if (Vector2.Distance(arrow.transform.Find("Image").position, menu_combatSlotIcon.transform.position) < 50) // in range of combat slot
            {
                CombatSlotSelection();
                menu_movementSlotIcon.UpdateSlot();
            }
            else
            {
                menu_combatSlotIcon.UpdateSlot();
                menu_movementSlotIcon.UpdateSlot();
                selectedBehaviorSlot = null;
            }
        }
        else
        {
            selectedBehaviorSlot = null;
        }
    }

    public void ApplyBehaviorSelection()
    {
        if (menu_heldBehaviorSlot.heldBehavior != null && selectedBehaviorSlot != null)
        {
            if (selectedBehaviorSlot.heldBehavior != null)
            {
                selectedBehaviorSlot.RemoveBehavior(); // 
                selectedBehaviorSlot.AddBehavior(menu_heldBehaviorSlot.heldBehavior);
            }
            else
            {
                selectedBehaviorSlot.AddBehavior(menu_heldBehaviorSlot.heldBehavior);
            }
        }
    }

    public void MovementSlotSelection()
    {
        if (menu_heldBehaviorSlot.heldBehavior != null)
        {
            menu_movementSlotIcon.transform.Find("Icon").GetComponent<Image>().sprite = menu_heldBehaviorSlot.heldBehavior.behavioricon;
            selectedBehaviorSlot = menu_movementSlotIcon;
        }
        else
        {
            menu_movementSlotIcon.UpdateSlot();
            selectedBehaviorSlot = null;
        }
    }

    public void CombatSlotSelection()
    {
        if (menu_heldBehaviorSlot.heldBehavior != null)
        {
            menu_combatSlotIcon.transform.Find("Icon").GetComponent<Image>().sprite = menu_heldBehaviorSlot.heldBehavior.behavioricon;
            selectedBehaviorSlot = menu_combatSlotIcon;
        }
        else
        {
            menu_combatSlotIcon.UpdateSlot();
            selectedBehaviorSlot = null;
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
