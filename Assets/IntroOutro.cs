using UnityEngine;

public class IntroOutro : MonoBehaviour
{
    public void StartGame()
    {
        SceneSwitcher.instance.LoadLevels();
    }

    public void MainMenu()
    {
        SceneSwitcher.instance.ReturnToMenu();
    }
}
