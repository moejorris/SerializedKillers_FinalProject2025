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
    private BehaviorSlot combatSlot;
    private BehaviorSlot movementSlot;
    public BehaviorSlot centerSlot;

    private BehaviorSlot selectedBehaviorSlot;

    private Animator animator;

    public Behavior targetedBehavior;
    [SerializeField] private Sprite emptySprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combatSlot = transform.Find("BG/CombatSlot").GetComponent<BehaviorSlot>();
        movementSlot = transform.Find("BG/MovementSlot").GetComponent<BehaviorSlot>();
        centerSlot = transform.Find("BG/CenterSlot").GetComponent<BehaviorSlot>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (northButton.action.WasPressedThisFrame())
        {
            if (menuOpen)
            {
                menuPanel.SetActive(false);
                menuOpen = false;
            }
            else
            {
                menuPanel.SetActive(true);
                menuOpen = true;
            }
        }

        if (menuOpen)
        {
            if (leftJoystick.action.ReadValue<Vector2>().x > 0.4f) // being held right
            {
                animator.SetInteger("Slot", 1);
            }
            else if (leftJoystick.action.ReadValue<Vector2>().x < -0.4f) // being held left
            {
                animator.SetInteger("Slot", -1);
            }
            else
            {
                animator.SetInteger("Slot", 0);
            }
        }

        //if (northButton.action.ReadValue<float>() > 0) // holding Y
        //{
        //    menuPanel.SetActive(true);
        //    menuOpen = true;
        //}
        //else // let go of Y
        //{
        //    menuPanel.SetActive(false);
        //    menuOpen = false;
        //    ApplyBehaviorSelection();
        //}

        if (menuOpen)
        {
            slowTime -= Time.deltaTime * 2;
            if (slowTime < 0.1f)
            {
                slowTime= 0.1f;
            }

            float mult = 10 - (slowTime * 10);
            animator.SetFloat("Speed", mult);

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

            float mult = 100 - (slowTime * 100);
            animator.SetFloat("Speed", mult);

            Cursor.lockState = CursorLockMode.Locked;
        }

        Time.timeScale = slowTime;


        //if (InputIsKeyboard())
        //{
        //    arrow.transform.rotation = Quaternion.Euler(0,0, (Mathf.Atan2(Input.mousePosition.y - arrow.transform.position.y, Input.mousePosition.x - arrow.transform.position.x) * Mathf.Rad2Deg) - 90);


        //    Vector2 localPos = arrow.transform.Find("Image").transform.localPosition;
        //    localPos.y = Vector2.Distance(arrow.transform.position, Input.mousePosition);
            
        //    if (localPos.y > 180)
        //    {
        //        localPos.y = 180;
        //    }

        //    arrow.transform.Find("Image").transform.localPosition = localPos;
        //}
        //else
        //{
        //    if (leftJoystick.action.ReadValue<Vector2>().x != 0 || leftJoystick.action.ReadValue<Vector2>().y != 0)
        //    {
        //        arrow.transform.eulerAngles = new Vector3(0, 0, (Mathf.Atan2(leftJoystick.action.ReadValue<Vector2>().y, leftJoystick.action.ReadValue<Vector2>().x) * Mathf.Rad2Deg) - 92);
        //    }

        //    Vector2 localPos = arrow.transform.Find("Image").transform.localPosition;
        //    localPos.y = leftJoystick.action.ReadValue<Vector2>().magnitude * 180f;
        //    arrow.transform.Find("Image").transform.localPosition = localPos;
        //}


        //if (menuOpen)
        //{
        //    if (Vector2.Distance(arrow.transform.Find("Image").position, movementSlot.transform.position) < 50) // in range of movement slot
        //    {
        //        MovementSlotSelection();
        //        combatSlot.UpdateSlot();
        //    }
        //    else if (Vector2.Distance(arrow.transform.Find("Image").position, combatSlot.transform.position) < 50) // in range of combat slot
        //    {
        //        CombatSlotSelection();
        //        movementSlot.UpdateSlot();
        //    }
        //    else
        //    {
        //        combatSlot.UpdateSlot();
        //        movementSlot.UpdateSlot();
        //        selectedBehaviorSlot = null;
        //    }
        //}
        //else
        //{
        //    selectedBehaviorSlot = null;
        //}
    }

    public void ApplyBehaviorSelection()
    {
        if (centerSlot.heldBehavior != null && selectedBehaviorSlot != null)
        {
            if (selectedBehaviorSlot.heldBehavior != null)
            {
                selectedBehaviorSlot.RemoveBehavior(); // 
                selectedBehaviorSlot.AddBehavior(centerSlot.heldBehavior);
            }
            else
            {
                selectedBehaviorSlot.AddBehavior(centerSlot.heldBehavior);
            }
        }
    }

    public void MovementSlotSelection()
    {
        if (centerSlot.heldBehavior != null)
        {
            movementSlot.transform.Find("Icon").GetComponent<Image>().sprite = centerSlot.heldBehavior.behavioricon;
            selectedBehaviorSlot = movementSlot;
        }
        else
        {
            movementSlot.UpdateSlot();
            selectedBehaviorSlot = null;
        }
    }

    public void CombatSlotSelection()
    {
        if (centerSlot.heldBehavior != null)
        {
            combatSlot.transform.Find("Icon").GetComponent<Image>().sprite = centerSlot.heldBehavior.behavioricon;
            selectedBehaviorSlot = combatSlot;
        }
        else
        {
            combatSlot.UpdateSlot();
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
