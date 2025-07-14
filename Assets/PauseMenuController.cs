using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public static bool isPaused = false;
    [SerializeField] InputActionReference pauseButton;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] Slider masterVolume;
    [SerializeField] Slider sfx;
    [SerializeField] Slider music;

    void Start()
    {
        isPaused = false;
        ChangePausedState();
    }

    void Update()
    {
        InputCheck();
    }

    void InputCheck()
    {
        if (pauseButton.action.WasPressedThisFrame())
        {
            isPaused = !isPaused;
            ChangePausedState();
        }
    }

    void ChangePausedState()
    {
        Time.timeScale = isPaused ? 0 : 1;

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;

        for (int i = 0; i < transform.childCount; i++)
        {
            Debug.Log(i);
            transform.GetChild(i)?.gameObject?.SetActive(isPaused);
        }
        optionsMenu.SetActive(false);
        
        if (isPaused)
        {
            UpdateSoundSliders();
        }

    }

    public void UnPause()
    {
        isPaused = false;
        ChangePausedState();
    }

    public void RestartButton()
    {
        SceneSwitcher.instance.RestartLevel();
    }

    public void QuitToMenu()
    {
        SceneSwitcher.instance.ReturnToMenu();
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

    public void UpdateCameraSensitivity(float sensitivity)
    {
        
    }

    void UpdateSoundSliders()
    {
        masterVolume.value = SoundManager.instance.GetMasterVolume();
        sfx.value = SoundManager.instance.GetSFXVolume();
        music.value = SoundManager.instance.GetMusicVolume();
    }
}
