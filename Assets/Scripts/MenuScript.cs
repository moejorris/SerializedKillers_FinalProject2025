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

    private void Start()
    {
        MainPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        HelpPanel.SetActive(false);
        CreditsPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartBtn()
    {
        SceneSwitcher.instance.LoadLevels();
    }

    public void HelpBtn()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        HelpPanel.SetActive(true);
        CreditsPanel.SetActive(false);
    }

    public void SettingsBtn()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(true);
        HelpPanel.SetActive(false);
        CreditsPanel.SetActive(false);
    }

    public void CreditsBtn()
    {
        /*
        FadeInOutAnimator.SetTrigger("FadeOut");
        CameraTransformAnimator.SetBool("MoveToCredits", true);
        */

        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        HelpPanel.SetActive(false);
        CreditsPanel.SetActive(true);
    }

    public void BackBtnFade()
    {
        FadeInOutAnimator.SetTrigger("FadeOut");
        CameraTransformAnimator.SetBool("MoveToCredits", false);
        BackBtn();
    }


    public void BackBtn()
    {
        MainPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        HelpPanel.SetActive(false);
        CreditsPanel.SetActive(false);
    }
    public void QuitBtn()
    {Application.Quit();}
}
