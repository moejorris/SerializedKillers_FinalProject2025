using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    //A very quickly made menu script, made by Tyler. Feel free to completely replace this with something more efficent.
    
    [Header("Panel Input")]
    public GameObject MainPanel, SettingsPanel, HelpPanel, CreditsPanel;

    private void Start()
    {
        MainPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        HelpPanel.SetActive(false);
        CreditsPanel.SetActive(false);
    }

    public void StartBtn()
    { SceneManager.LoadScene("Levels"); }

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
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        HelpPanel.SetActive(false);
        CreditsPanel.SetActive(true);
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
