using System.Collections;
using TMPro;
using UnityEngine;

public class ComputerScreenCutscene : MonoBehaviour
{
    public TMP_Text screenText;

    private void Start()
    {
        StartCoroutine(ShowCutsceneMessages());
    }

    IEnumerator ShowCutsceneMessages()
    {
        yield return ShowMessage("Booting system...", 0.05f, 1.5f);
        yield return ShowMessage("Delete the game?", 0.05f, 22.0f);
        yield return ShowMessage("Error: File in use", 0.05f, 1.5f);
        yield return ShowMessage("Error: File in use", 0.05f, 1.5f);
    }

    IEnumerator ShowMessage(string message, float letterDelay, float holdDelay)
    {
        screenText.text = "";
        foreach (char letter in message)
        {
            screenText.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }
        yield return new WaitForSeconds(holdDelay);
    }
}
