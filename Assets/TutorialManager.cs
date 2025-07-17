using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TMP_Text textBox;
    [SerializeField] private Animator panelAnimator;
    [SerializeField] private string[] phaseOneMessages;
    [SerializeField] private string[] phaseTwoMessages;
    [SerializeField] private float secondsBetweenCharacters = 0.01f;
    private int phase = 0;
    private string textToWrite;
    private bool cont = false;

    [Header("Enemies")]
    [SerializeField] private GameObject tutorialOverclockPrefab;
    [SerializeField] private Transform spawnPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPhaseTwo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TutorialPhaseOne()
    {
        yield return new WaitForSeconds(1);
    }

    public void StartPhaseTwo()
    {
        StopCoroutine("TutorialPhaseOne");

        if (phase == 2) return;

        StopCoroutine("TutorialPhaseTwo");
        StartCoroutine("TutorialPhaseTwo");
    }

    IEnumerator TutorialPhaseTwo()
    {
        phase = 2;
        textBox.text = string.Empty;
        textToWrite = phaseTwoMessages[0];
        panelAnimator.SetBool("Appear", true);

        yield return new WaitForSeconds(1);

        foreach (char letter in textToWrite)
        {
            textBox.text += letter;
            yield return new WaitForSeconds(secondsBetweenCharacters);
        }

        yield return new WaitForSeconds(1);

        GameObject enemy = Instantiate(tutorialOverclockPrefab, spawnPos);
        enemy.transform.position = enemy.transform.parent.position;

        textBox.text = string.Empty;
        textToWrite = phaseTwoMessages[1];

        foreach (char letter in textToWrite)
        {
            textBox.text += letter;
            yield return new WaitForSeconds(secondsBetweenCharacters);
        }

        yield return new WaitUntil(() => phase == 3);

        StartPhaseThree();
    }

    public void StartPhaseThree()
    {
        StopCoroutine("TutorialPhaseTwo");

        if (phase == 3) return;

        StopCoroutine("TutorialPhaseThree");
        StartCoroutine("TutorialPhaseThree");
    }
}
