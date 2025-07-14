using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField]float masterVolume = 1;
    [SerializeField]float sfxVolume = 1;
    [SerializeField]float musicVolume = 1;

    [SerializeField] AudioClip mainMenuMusic;
    [SerializeField] AudioClip levelsMusic;

    AudioSource sfxSource;
    AudioSource musicSource;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = GetComponent<AudioSource>();
        musicSource = GetComponentInChildren<AudioSource>();

        SetDefaults();

        masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");
    }

    void SetDefaults()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");

        if (masterVolume == 0)
        {
            PlayerPrefs.SetFloat("MasterVolume", 1f);
        }
        if (sfxVolume == 0)
        {
            PlayerPrefs.SetFloat("SFXVolume", 1f);
        }
        if (musicVolume == 0)
        {
            PlayerPrefs.SetFloat("MusicVolume", 1f);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += ChangeSong;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= ChangeSong;
    }

    private void ChangeSong(Scene scene, LoadSceneMode mode)
    {
        if (mainMenuMusic == null || levelsMusic == null) return;

        musicSource.volume = musicVolume * masterVolume;

        musicSource.Stop();
        switch (scene.name)
        {
            case "Levels":
                musicSource.clip = levelsMusic;
                musicSource.Play();
                break;
            case "MainMenu":
                musicSource.clip = mainMenuMusic;
                musicSource.Play();
                break;
        }

        if (!musicSource.isPlaying)
        {
            Debug.LogWarning("No Music is assigned to this scene. Playing Main Menu Music");
            musicSource.clip = mainMenuMusic;
            musicSource.Play();
        }
    }

    public void PlaySoundEffect(SoundEffectSO sfx)
    {
        sfxSource.volume = sfxVolume * masterVolume;
        sfxSource.pitch = sfx.usesRandomPitch ? sfx.RandomPitch * Time.timeScale : 1 * Time.timeScale;
        sfxSource.PlayOneShot(sfx.SoundEffect());
    }

    public void PlaySoundEffectDelayed(SoundEffectSO sfx, float delay)
    {
        StartCoroutine(DelayedSound(sfx, delay));
    }

    IEnumerator DelayedSound(SoundEffectSO sfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySoundEffect(sfx);
    }

    public void UpdateMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        masterVolume = volume;
    }

    public void UpdateSoundEffectVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        sfxVolume = volume;
    }

    public void UpdateMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        musicVolume = volume;
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }
}
