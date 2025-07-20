using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AnimationCurve spacialSoundCurve;
    public float MasterVolume { get { return masterVolume;}}
    public float SFXVolume { get { return masterVolume * sfxVolume;}}
    public float MusicVolume { get { return masterVolume * musicVolume;}}


    [SerializeField] float masterVolume = 1;
    [SerializeField]float sfxVolume = 1;
    [SerializeField]float musicVolume = 1;

    [SerializeField] AudioClip mainMenuMusic;
    [SerializeField] AudioClip levelsMusic;

    AudioSource sfxSource;
    [SerializeField] AudioSource musicSource;

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

        if (!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetFloat("MasterVolume", 1f);
        }
        if (!PlayerPrefs.HasKey("SFXVolume"))
        {
            PlayerPrefs.SetFloat("SFXVolume", 1f);
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1f);
        }
    }

    private void Update()
    {
        UpdateVolumes();    
    }

    void UpdateVolumes()
    {
        if(musicSource.volume != musicVolume * masterVolume)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
        if(sfxSource.volume != sfxVolume * masterVolume)
        {
            sfxSource.volume = sfxVolume * masterVolume;
        }
    }

    void OnEnable()
    {
        ChangeSong(SceneManager.GetSceneByName("MainMenu"));
        SceneManager.sceneLoaded += ChangeSong;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= ChangeSong;
    }

    private void ChangeSong(Scene scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (mainMenuMusic == null && levelsMusic == null) return;

        musicSource.loop = true;
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
            Debug.LogWarning("No Music is assigned to this scene. Playing Main Menu Music instead");
            musicSource.clip = mainMenuMusic;
            musicSource.Play();
        }
    }

    public void PlaySoundEffect(SoundEffectSO sfx)
    {
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume * masterVolume * sfx.volume;
        sfxSource.pitch = sfx.usesRandomPitch ? sfx.RandomPitch * Time.timeScale : 1 * Time.timeScale;
        sfxSource.PlayOneShot(sfx.SoundEffect());
        sfxSource.pitch = 1;

        sfxSource.volume = sfxVolume * masterVolume;
    }

    public void PlaySoundEffectOnObject(SoundEffectSO sfx, Transform parentObject, float spatialBlend = 1f)
    {
        GameObject audioObject = new GameObject();
        audioObject.name = sfx.name;
        audioObject.transform.parent = parentObject;
        audioObject.transform.localPosition = Vector3.zero;


        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.loop = false;
        source.spatialBlend = spatialBlend;

        source.maxDistance = 35f; // sets the max distance the audio can be heard
        source.rolloffMode = AudioRolloffMode.Custom; // switches it from log to custom curve
        source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, spacialSoundCurve); // switches that curve to the variable in the inspector, spacialSoundCurve

        source.volume = sfxVolume * masterVolume * sfx.volume;
        source.pitch = sfx.usesRandomPitch ? sfx.RandomPitch * Time.timeScale : 1 * Time.timeScale;
        source.clip = sfx.SoundEffect();

        source.Play();
        Destroy(audioObject, source.clip.length + 0.1f);
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
        sfxSource.volume = sfxVolume * masterVolume;
    }

    public void UpdateMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        musicVolume = volume;
        musicSource.volume = musicVolume * masterVolume;
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
