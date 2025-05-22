using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EquippedScriptsMenuTest : MonoBehaviour
{
    public InputActionReference openMenu;
    public InputActionReference menuWheel;
    public GameObject panel;

    public GameObject selectionIcon;
    public GameObject topScriptPos;
    public GameObject leftScriptPos;
    public GameObject rightScriptPos;

    private GameObject newSelectPosition;
    public GameObject selectedIcon;
    public GameObject topEquippedScriptPos;
    public GameObject leftEquippedScriptPos;
    public GameObject rightEquippedScriptPos;


    public Image selectedScriptImage;
    private Sprite currentScriptSprite;
    //public Sprite selectedScriptSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentScriptSprite = selectedScriptImage.sprite;
        newSelectPosition = selectedIcon;
    }

    // Update is called once per frame
    void Update()
    {
        //panel.SetActive(openMenu.action.ReadValue<bool>());
        if (openMenu.action.ReadValue<float>() > 0)
        {
            panel.SetActive(true);
            if (menuWheel.action.ReadValue<Vector2>().x < -0.5f && menuWheel.action.ReadValue<Vector2>().y > -0.3f) // left
            {
                selectionIcon.SetActive(true);
                selectionIcon.transform.position = leftScriptPos.transform.position;
                selectedScriptImage.sprite = leftScriptPos.transform.Find("Icon").GetComponent<Image>().sprite;

                newSelectPosition = leftEquippedScriptPos;
            }
            else if (menuWheel.action.ReadValue<Vector2>().x > 0.5f && menuWheel.action.ReadValue<Vector2>().y > -0.3f) // right
            {
                selectionIcon.SetActive(true);
                selectionIcon.transform.position = rightScriptPos.transform.position;
                selectedScriptImage.sprite = rightScriptPos.transform.Find("Icon").GetComponent<Image>().sprite;

                newSelectPosition = rightEquippedScriptPos;
            }
            else if (menuWheel.action.ReadValue<Vector2>().x < 0.5f && menuWheel.action.ReadValue<Vector2>().x > -0.5f && menuWheel.action.ReadValue<Vector2>().y > 0.5f) // up
            {
                selectionIcon.SetActive(true);
                selectionIcon.transform.position = topScriptPos.transform.position;
                selectedScriptImage.sprite = topScriptPos.transform.Find("Icon").GetComponent<Image>().sprite;

                newSelectPosition = topEquippedScriptPos;
            }
            else
            {
                selectionIcon.SetActive(false);
                selectedScriptImage.sprite = currentScriptSprite;

                newSelectPosition = selectedIcon;
            }
        }
        else
        {
            if (selectedScriptImage.sprite != currentScriptSprite)
            {
                currentScriptSprite = selectedScriptImage.sprite;
            }

            if (newSelectPosition != selectedIcon)
            {
                selectedIcon.transform.position = newSelectPosition.transform.position;
                newSelectPosition = selectedIcon;
            }

            selectionIcon.SetActive(false);
            panel.SetActive(false);
        }

        //Debug.Log(menuWheel.action.ReadValue<Vector2>());
    }
}
