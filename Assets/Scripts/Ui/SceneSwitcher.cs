using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] CanvasGroup fadeInOutPanel;
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] Slider loadingBar;
    public static SceneSwitcher instance;

    bool isTransitioning = false;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeInOutPanel == null)
        {
            fadeInOutPanel = GetComponentInChildren<CanvasGroup>();
        }
    }
    
    public void LoadIntro()
    {
        if(isTransitioning) return;

        Time.timeScale = 1;
        StartCoroutine(ChangeSceneFancy("Intro2"));
    }

    public void LoadOutro()
    {
        if(isTransitioning) return;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 1;
        StartCoroutine(ChangeSceneFancy("Outro2"));
    }

    public void LoadLevels()
    {
        if (isTransitioning) return;

        Time.timeScale = 1;
        StartCoroutine(ChangeSceneFancy("Levels"));
    }

    public void ReturnToMenu()
    {
        if(isTransitioning) return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 1;
        StartCoroutine(ChangeSceneFancy("MainMenu"));

    }

    public void RestartLevel()
    {
        if(isTransitioning) return;

        Time.timeScale = 1;
        StartCoroutine(ChangeSceneFancy(SceneManager.GetActiveScene().name));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator ChangeSceneFancy(string sceneName)
    {
        if (isTransitioning)
        {
            yield break;
        }
        isTransitioning = true;

        Scene currentScene = SceneManager.GetActiveScene();

        loadingText.text = "Loading";
        loadingBar.value = 0f;

        fadeInOutPanel.blocksRaycasts = true;
        Time.timeScale = 0;

        float t = 0;
        float transitionTime = 1f;
        float fadeInOutAlpha;

        while (t < transitionTime)
        {
            yield return new WaitForEndOfFrame();
            t += Time.unscaledDeltaTime;
            fadeInOutAlpha = t / transitionTime;
            fadeInOutPanel.alpha = fadeInOutAlpha;
            Time.timeScale = 0;
        }

        fadeInOutPanel.alpha = 1f;
        fadeInOutAlpha = 1f;

        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);


        int periodCount = 0;

        while (!loading.isDone)
        {
            yield return new WaitForSecondsRealtime(0.2f);

            if (periodCount < 3)
            {
                loadingText.text += ".";
                periodCount++;
            }
            else
            {
                periodCount = 0;
                loadingText.text = "Loading";
            }

            loadingBar.value = loading.progress;

        }


        t = 0;
        while (t < transitionTime)
        {
            yield return new WaitForEndOfFrame();
            t += Time.unscaledDeltaTime;
            fadeInOutAlpha = t / transitionTime;
            Time.timeScale = fadeInOutAlpha;
            fadeInOutPanel.alpha = 1 - fadeInOutAlpha;
        }

        fadeInOutPanel.alpha = 0f;

        fadeInOutPanel.blocksRaycasts = false;
        Time.timeScale = 1;

        isTransitioning = false;
    }
}
