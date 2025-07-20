using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    //A very quickly made menu script, made by Tyler

    [Header("Panel Input")]
    public GameObject MainPanel;
    public GameObject SettingsPanel;
    public GameObject HelpPanel;
    public GameObject CreditsPanel;

    [Header("Animators")]
    public Animator FadeInOutAnimator;
    public Animator CameraTransformAnimator;

    private void Start()    //On scene start
    {
        MainPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        HelpPanel.SetActive(false);
        CreditsPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartBtn()  //When start button is pressed
    {SceneSwitcher.instance.LoadLevels();}

    public void CreditsBtn() //When credits button is pressed
    {FadeInOutAnimator.SetTrigger("FadeOutCreditsMenu");}

    public void creditsPanelEnable()
    {
        CameraTransformAnimator.SetBool("MoveToCredits", true);
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        HelpPanel.SetActive(false);
        CreditsPanel.SetActive(true);
    }

    public void HelpBtn()   //When help button is pressed
    {FadeInOutAnimator.SetTrigger("FadeOutHelpMenu");}

    public void helpPanelEnable()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        HelpPanel.SetActive(true);
        CreditsPanel.SetActive(false);
    }

    public void SettingsBtn()   //When settings button is pressed
    { FadeInOutAnimator.SetTrigger("FadeOutSettingsMenu"); }

    public void SettingsPanelEnable()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(true);
        HelpPanel.SetActive(false);
        CreditsPanel.SetActive(false);
    }
    public void BackBtn()   //When back button is pressed
    { FadeInOutAnimator.SetTrigger("FadeOutReturn"); }

    public void homePanelEnable()
    {
        CameraTransformAnimator.SetBool("MoveToCredits", false);
        MainPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        HelpPanel.SetActive(false);
        CreditsPanel.SetActive(false);
    }

    public void QuitBtn()
    {Application.Quit();}
}
