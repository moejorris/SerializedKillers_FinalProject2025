using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TutorialBase : MonoBehaviour
{
    [SerializeField] private TMP_Text textBox;
    [SerializeField] private float secondsBetweenCharacters = 0.01f;
    private string textToWrite;
    private bool cont = false;

    public virtual void Start()
    {
        textToWrite = textBox.text;
        textBox.text = string.Empty;
        StopCoroutine("WriteOutText");
        StartCoroutine("WriteOutText");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            cont = true;
        }
    }

    public virtual void DisplayPopup()
    {

    }

    public IEnumerator WriteOutText()
    {
        transform.Find("BG").GetComponent<Animator>().SetBool("Appear", true);
        yield return new WaitForSeconds(1);

        foreach (char letter in textToWrite)
        {
            textBox.text += letter;
            yield return new WaitForSeconds(secondsBetweenCharacters);
        }

        Time.timeScale = 0.1f;

        yield return new WaitUntil(() => cont);

        Time.timeScale = 1;
    }
}
