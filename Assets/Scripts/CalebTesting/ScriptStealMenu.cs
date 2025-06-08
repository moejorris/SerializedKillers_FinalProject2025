using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScriptStealMenu : MonoBehaviour
{
    public InputActionReference northButton; // Pressing Y
    public InputActionReference leftJoystick;
    public InputActionReference southButton; // Pressing A
    public InputActionReference westButton; // Pressing X
    public PlayerInput playerInput;

    public GameObject player;
    public float attackDistance;

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

    private BehaviorSlot hud_combatSlot;
    private BehaviorSlot hud_movementSlot;

    private BehaviorSlot selectedBehaviorSlot;
    public EnemyAI_Base selectedEnemy;


    [SerializeField] private LayerMask enemyLayer;

    private Animator animator;

    //public Behavior targetedBehavior;
    [SerializeField] private Sprite emptySprite;

    [SerializeField] private EnemyManager enemyManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combatSlot = transform.Find("BG/CombatSlot").GetComponent<BehaviorSlot>();
        movementSlot = transform.Find("BG/MovementSlot").GetComponent<BehaviorSlot>();
        centerSlot = transform.Find("BG/CenterSlot").GetComponent<BehaviorSlot>();
        animator = GetComponent<Animator>();
        menuPanel.SetActive(menuOpen);

        hud_combatSlot = transform.parent.Find("HUD/CombatSlot").GetComponent<BehaviorSlot>();
        hud_movementSlot = transform.parent.Find("HUD/MovementSlot").GetComponent<BehaviorSlot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!menuOpen)
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cameraRay, out RaycastHit hit, 100, enemyLayer, queryTriggerInteraction: QueryTriggerInteraction.Collide))
            {
                if (hit.transform.gameObject.layer != 7) // temp issue due to rigidbody making parent take the collider stuff and not child?
                {
                    if (hit.transform.GetComponent<EnemyAI_Base>() != null && hit.transform.GetComponent<EnemyAI_Base>().behaviorActive)
                    {
                        hit.transform.GetComponent<EnemyAI_Base>().SelectEnemy();
                    }
                }
                else
                {
                    if (hit.transform.parent.GetComponent<EnemyAI_Base>() != null && hit.transform.parent.GetComponent<EnemyAI_Base>().behaviorActive)
                    {
                        hit.transform.parent.GetComponent<EnemyAI_Base>().SelectEnemy();
                    }
                }
            }
            else if (selectedEnemy != null)
            {
                selectedEnemy.DeselectEnemy();
            }
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
            //Debug.Log(Input.mousePosition);
            if (leftJoystick.action.ReadValue<Vector2>().x > 0.4f || Input.mousePosition.x > (Screen.width / 3 * 2) - 50) // being held right
            {
                animator.SetInteger("Slot", 1);
                CombatSlotSelection();
                movementSlot.UpdateSlot();
            }
            else if (leftJoystick.action.ReadValue<Vector2>().x < -0.4f || Input.mousePosition.x < Screen.width / 3 + 50) // being held left
            {
                animator.SetInteger("Slot", -1);
                MovementSlotSelection();
                combatSlot.UpdateSlot();
            }
            else
            {
                animator.SetInteger("Slot", 0);
                selectedBehaviorSlot = null;
                movementSlot.UpdateSlot();
                combatSlot.UpdateSlot();
                //centerSlot.UpdateSlot();
            }

            // --------------------- CHECKING INPUTS -----------------

            if (southButton.action.WasPerformedThisFrame() || Input.GetMouseButtonDown(0)) // A BUTTON
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

            if (westButton.action.WasPerformedThisFrame() || Input.GetMouseButtonDown(1))
            {
                Debug.Log("Button X pressed");
                if (selectedBehaviorSlot != null)
                {
                    Debug.Log("Thing in slot");
                    RemoveBehaviorSelection();
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
            if (westButton.action.WasPerformedThisFrame())
            {
                
                RaycastHit[] hits = Physics.RaycastAll(player.transform.parent.position, player.transform.forward, attackDistance);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.gameObject.layer != 7) // temp issue due to rigidbody making parent take the collider stuff and not child?
                    {
                        if (hit.transform.GetComponent<EnemyAI_Base>() != null)
                        {
                            hit.transform.GetComponent<EnemyAI_Base>().TakeDamage(5);
                        }
                    }
                    else
                    {
                        if (hit.transform.parent.GetComponent<EnemyAI_Base>() != null)
                        {
                            hit.transform.parent.GetComponent<EnemyAI_Base>().TakeDamage(5);
                        }
                    }
                } 
            }


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

    public void RemoveBehaviorSelection()
    {
        Debug.Log("RemoveBehaviorSelection called");
        if (selectedBehaviorSlot.heldBehavior != null)
        {
            Debug.Log("Removed Behavior");
            enemyManager.ActivateBehavior(selectedBehaviorSlot.heldBehavior);

            selectedBehaviorSlot.RemoveBehavior();

            hud_combatSlot.heldBehavior = combatSlot.heldBehavior;
            hud_combatSlot.UpdateSlot();
            hud_movementSlot.heldBehavior = movementSlot.heldBehavior;
            hud_movementSlot.UpdateSlot();
        }
    }

    public void ApplyBehaviorSelection()
    {
        if (centerSlot.heldBehavior != null && selectedBehaviorSlot != null)
        {
            Debug.Log("behavior is held. slot is selected.");
            if (selectedBehaviorSlot.heldBehavior != null)
            {
                Debug.Log("something was in slot, replaced.");

                enemyManager.ActivateBehavior(selectedBehaviorSlot.heldBehavior); // ACTIVATES THE SCRIPT ABOUT TO BE REPLACED

                selectedBehaviorSlot.RemoveBehavior();
                selectedBehaviorSlot.AddBehavior(centerSlot.heldBehavior);
            }
            else
            {
                Debug.Log("thing added to slot");
                selectedBehaviorSlot.AddBehavior(centerSlot.heldBehavior);
            }

            enemyManager.DeactivateBehavior(selectedBehaviorSlot.heldBehavior); // NOW DEACTIVATES THE NEW SLOT!!!!

            centerSlot.RemoveBehavior();
            //selectedEnemy.behaviorActive = false;
            selectedEnemy.DeselectEnemy();
            menuPanel.SetActive(false);
            menuOpen = false;
        }

        hud_combatSlot.heldBehavior = combatSlot.heldBehavior;
        hud_combatSlot.UpdateSlot();
        hud_movementSlot.heldBehavior = movementSlot.heldBehavior;
        hud_movementSlot.UpdateSlot();
    }

    public void MovementSlotSelection()
    {
        if (centerSlot.heldBehavior != null)
        {
            //Debug.Log("Center slot held behavior != null");
            movementSlot.transform.Find("Icon").GetComponent<Image>().sprite = centerSlot.heldBehavior.behavioricon;
            //centerSlot.transform.Find("Icon").GetComponent<Image>().sprite = emptySprite; // making center blank?
            selectedBehaviorSlot = movementSlot;
        }
        else
        {
            //Debug.Log("Center slot held behavior = null");
            movementSlot.UpdateSlot();
            selectedBehaviorSlot = movementSlot; // ADDED TEMP
            //selectedBehaviorSlot = null;
        }
    }

    public void CombatSlotSelection()
    {
        if (centerSlot.heldBehavior != null)
        {
            combatSlot.transform.Find("Icon").GetComponent<Image>().sprite = centerSlot.heldBehavior.behavioricon;
            //centerSlot.transform.Find("Icon").GetComponent<Image>().sprite = emptySprite; // making center blank?
            selectedBehaviorSlot = combatSlot;
        }
        else
        {
            combatSlot.UpdateSlot();
            selectedBehaviorSlot = combatSlot;
            //selectedBehaviorSlot = null;
        }
    }

    public void UpdateCenterSlot()
    {
        if (selectedEnemy != null)
        {
            centerSlot.heldBehavior = selectedEnemy.heldBehavior;
            centerSlot.UpdateSlot();
        }
        else
        {
            centerSlot.heldBehavior = null;
            centerSlot.UpdateSlot();
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
