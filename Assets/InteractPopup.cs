using UnityEngine;
using UnityEngine.UI;

public class InteractPopup : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float dist = 3;
    private Image controllerIcon;
    private Image keyboardIcon;
    private Transform player => GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerController");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (animator == null) Debug.LogError("Please Assign an Animator to this Popup");
        else
        {
            keyboardIcon = animator.transform.Find("Keyboard").GetComponent<Image>();
            controllerIcon = animator.transform.Find("Controller").GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.position, transform.position) < dist)
        {
            animator.SetBool("Nearby", true);
            UpdateControls();
        }
        else animator.SetBool("Nearby", false);
    }

    public void UpdateControls()
    {
        keyboardIcon.enabled = PlayerController.instance.ScriptSteal.InputIsKeyboard();
        controllerIcon.enabled = !PlayerController.instance.ScriptSteal.InputIsKeyboard();
    }
}
