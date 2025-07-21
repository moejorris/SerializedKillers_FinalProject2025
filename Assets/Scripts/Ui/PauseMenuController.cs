using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuController : MonoBehaviour
{
    public static bool isPaused = false;
    [SerializeField] InputActionReference pauseButton;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] Slider masterVolume;
    [SerializeField] Slider sfx;
    [SerializeField] Slider music;
    [SerializeField] Slider sensitivitySlider;

    [SerializeField] GameObject pauseFirstButton;
    [SerializeField] GameObject settingsFirstButton;

    bool isPlayerAlreadyPaused = false;

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
            transform.GetChild(i)?.gameObject?.SetActive(isPaused);
        }
        optionsMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);

        if (isPaused)
        {
            UpdateSliders();
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);

            if (PlayerController.instance.PlayerInput.inputIsActive)
            {
                PlayerController.instance.PlayerInput.DeactivateInput();
                isPlayerAlreadyPaused = false;
            }
            else
            {
                isPlayerAlreadyPaused = true;
            }
        }
        else
        {
            if (!isPlayerAlreadyPaused)
            {
                PlayerController.instance.PlayerInput.ActivateInput();
                isPlayerAlreadyPaused = false;
            }
        }

    }

    public void UnPause()
    {
        isPaused = false;
        ChangePausedState();
    }

    public void SettingsButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsFirstButton);
    }

    public void BackButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);

    }

    public void RestartLevelButton()
    {
        SceneSwitcher.instance.RestartLevel();
    }
    public void RestartCheckpointButton()
    {
        PlayerController.instance.Respawn.Respawn();
        UnPause();
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
        PlayerPrefs.SetFloat("CamSens", sensitivity);
        PlayerCamRotate.instance.gameSensitivity = sensitivity;
    }

    void UpdateSliders()
    {
        masterVolume.value = SoundManager.instance.GetMasterVolume();
        sfx.value = SoundManager.instance.GetSFXVolume();
        music.value = SoundManager.instance.GetMusicVolume();
        sensitivitySlider.value = PlayerPrefs.GetFloat("CamSens");
        PlayerCamRotate.instance.gameSensitivity = sensitivitySlider.value;
    }
}
