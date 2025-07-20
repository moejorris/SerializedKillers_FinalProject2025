using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Settings")]
    [SerializeField] Slider masterVolume;
    [SerializeField] Slider sfx;
    [SerializeField] Slider music;
    [SerializeField] Slider sensitivity;

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
        CameraTransformAnimator.SetBool("MoveToHelp", true);
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
        UpdateSliders();
    }
    public void BackBtn()   //When back button is pressed
    { FadeInOutAnimator.SetTrigger("FadeOutReturn"); }

    public void homePanelEnable()
    {
        CameraTransformAnimator.SetBool("MoveToCredits", false);
        CameraTransformAnimator.SetBool("MoveToHelp", false);
        MainPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        HelpPanel.SetActive(false);
        CreditsPanel.SetActive(false);
    }

    public void QuitBtn()
    {Application.Quit();}

    void UpdateSliders()
    {
        masterVolume.value = SoundManager.instance.GetMasterVolume();
        sfx.value = SoundManager.instance.GetSFXVolume();
        music.value = SoundManager.instance.GetMusicVolume();

        if (!PlayerPrefs.HasKey("CamSens"))
        {
            PlayerPrefs.SetFloat("CamSens", 0.5f);
        }

        sensitivity.value = PlayerPrefs.GetFloat("CamSens");
    }

    public void UpdateMasterVolume(float volume)
    {
        SoundManager.instance.UpdateMasterVolume(volume);
    }
    public void UpdateMusicVolume(float volume)
    {
        SoundManager.instance.UpdateMusicVolume(volume);
    }
    public void UpdateSoundEffectVolume(float volume)
    {
        SoundManager.instance.UpdateSoundEffectVolume(volume);
    }

    public void UpdateCameraSensitivity(float sens)
    {
        PlayerPrefs.SetFloat("CamSens", sens);
    }
}
