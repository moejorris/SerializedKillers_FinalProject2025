using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScriptStealMenu : MonoBehaviour
{
    public InputActionReference northButton; // Pressing Y
    public InputActionReference leftJoystick;
    public InputActionReference southButton; // Pressing A
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
    public EnemyAI_Base selectedEnemy;


    [SerializeField] private LayerMask enemyLayer;

    private Animator animator;

    //public Behavior targetedBehavior;
    [SerializeField] private Sprite emptySprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combatSlot = transform.Find("BG/CombatSlot").GetComponent<BehaviorSlot>();
        movementSlot = transform.Find("BG/MovementSlot").GetComponent<BehaviorSlot>();
        centerSlot = transform.Find("BG/CenterSlot").GetComponent<BehaviorSlot>();
        animator = GetComponent<Animator>();
        menuPanel.SetActive(menuOpen);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(Camera.main.transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(5), color: Color.red);
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cameraRay, out RaycastHit hit, 100, enemyLayer, queryTriggerInteraction: QueryTriggerInteraction.Collide))
        {
            if (hit.transform.parent.GetComponent<EnemyAI_Base>() != null && hit.transform.parent.GetComponent<EnemyAI_Base>().behaviorActive)
            {
                hit.transform.parent.GetComponent<EnemyAI_Base>().SelectEnemy();
            }
        }
        else if (selectedEnemy != null)
        {
            selectedEnemy.DeselectEnemy();
        }



        if (northButton.action.WasPressedThisFrame()) // Checks if the player has pressed Y and toggles menu
        {
            //Debug.Log("Y was pressed");
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

        if (menuOpen) // Updates UI when menu is open
        {
            if (leftJoystick.action.ReadValue<Vector2>().x > 0.4f) // being held right
            {
                animator.SetInteger("Slot", 1);
                CombatSlotSelection();
                movementSlot.UpdateSlot();
            }
            else if (leftJoystick.action.ReadValue<Vector2>().x < -0.4f) // being held left
            {
                animator.SetInteger("Slot", -1);
                MovementSlotSelection();
                combatSlot.UpdateSlot();
            }
            else
            {
                animator.SetInteger("Slot", 0);
                movementSlot.UpdateSlot();
                combatSlot.UpdateSlot();
            }

            // --------------------- CHECKING INPUTS -----------------

            if (southButton.action.WasPerformedThisFrame())
            {
                if (selectedBehaviorSlot != null)
                {
                    ApplyBehaviorSelection();
                }
                else
                {
                    // error button or something lol
                }
            }

            // --------------------- SLOWING TIME --------------------
            slowTime -= Time.deltaTime * 2;
            if (slowTime < 0.1f)
            {
                slowTime = 0.1f;
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
                slowTime = 1;
            }

            float mult = 100 - (slowTime * 100);
            animator.SetFloat("Speed", mult);

            Cursor.lockState = CursorLockMode.Locked;
        }

        Time.timeScale = slowTime;
    }

    public void ApplyBehaviorSelection()
    {
        if (centerSlot.heldBehavior != null && selectedBehaviorSlot != null)
        {
            Debug.Log("behavior is held. slot is selected.");
            if (selectedBehaviorSlot.heldBehavior != null)
            {
                Debug.Log("something was in slot, replaced.");
                selectedBehaviorSlot.RemoveBehavior();
                selectedBehaviorSlot.AddBehavior(centerSlot.heldBehavior);
            }
            else
            {
                Debug.Log("thing added to slot");
                selectedBehaviorSlot.AddBehavior(centerSlot.heldBehavior);
            }

            centerSlot.RemoveBehavior();
            selectedEnemy.behaviorActive = false;
            selectedEnemy.DeselectEnemy();
            Debug.Log("selectedEnemy.behaviorActive = false");
            menuPanel.SetActive(false);
            menuOpen = false;
        }
    }

    public void MovementSlotSelection()
    {
        if (centerSlot.heldBehavior != null)
        {
            //Debug.Log("Center slot held behavior != null");
            movementSlot.transform.Find("Icon").GetComponent<Image>().sprite = centerSlot.heldBehavior.behavioricon;
            selectedBehaviorSlot = movementSlot;
        }
        else
        {
            //Debug.Log("Center slot held behavior = null");
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
