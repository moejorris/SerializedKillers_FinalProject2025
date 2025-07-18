using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private TMP_Text middleTextbox;
    [SerializeField] private TMP_Text bottomTextbox;
    [SerializeField] private TMP_Text continueText;
    [SerializeField] private Sprite scriptHudWithStuff;
    [SerializeField] private Image HUDImage;
    private bool continueAvailable = false;
    private bool continuePressed = false;

    private bool tutorialFinished = false;

    [SerializeField] private TMP_Text textBox;
    [SerializeField] private Animator panelAnimator;
    [SerializeField] private string[] phaseOneMessages;
    [SerializeField] private string[] phaseTwoMessages;
    [SerializeField] private float secondsBetweenCharacters = 0.01f;
    [SerializeField] private float speedUpMult = 1;

    private int phase = 0;
    private string textToWrite;
    private bool cont = false;

    [Header("Enemies")]
    [SerializeField] private GameObject tutorialOverclockPrefab;
    [SerializeField] private Transform spawnPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPhaseOne();
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorialFinished) return;

        if (jumpAction.action.WasPerformedThisFrame() || Input.GetMouseButtonDown(0))
        {
            if (continueAvailable)
            {
                continueAvailable = false;
                continuePressed = true;
            }
            else
            {
                speedUpMult = 0.5f;
            }
        }

        if (continueAvailable && continueText.color.a < 1)
        {
            Color aColor = continueText.color;
            aColor.a += Time.deltaTime * 2;
            continueText.color = aColor;
        }
        else if (continueText.color.a > 0)
        {
            Color aColor = continueText.color;
            aColor.a -= Time.deltaTime * 2;
            continueText.color = aColor;
        }
    }

    public void StartPhaseOne()
    {
        StopCoroutine("TutorialPhaseOne");
        StartCoroutine("TutorialPhaseOne");
    }

    public void StartPhaseTwo()
    {
        StopCoroutine("TutorialPhaseOne");

        if (phase == 2) return;

        StopCoroutine("TutorialPhaseTwo");
        StartCoroutine("TutorialPhaseTwo");
    }

    IEnumerator TutorialPhaseOne()
    {
        phase = 1;

        yield return new WaitForSeconds(1);
        //-------------------------------INITIAL APPEAR-------------

        middleTextbox.text = string.Empty;
        textToWrite = phaseOneMessages[0];
        panelAnimator.SetBool("Appear", true);

        yield return new WaitForSeconds(1);

        //-------------------------------FIRST TEXT-------------

        foreach (char letter in textToWrite)
        {
            middleTextbox.text += letter;
            yield return new WaitForSeconds(secondsBetweenCharacters * speedUpMult);
        }

        yield return new WaitForSeconds(1);

        continuePressed = false;
        continueAvailable = true;

        yield return new WaitUntil(() => continuePressed);
        speedUpMult = 1;

        while(middleTextbox.text.Length > 0)
        {
            middleTextbox.text = middleTextbox.text.Substring(0, middleTextbox.text.Length - 1);

            yield return new WaitForSeconds(secondsBetweenCharacters * 0.5f * speedUpMult);
        }

        //-------------------------------IMAGE AND SECOND TEXT-------------

        while (HUDImage.color.a < 1)
        {
            Color aColor = HUDImage.color;
            aColor.a += 0.1f;
            yield return new WaitForSeconds(0.1f * speedUpMult);
            HUDImage.color = aColor;
        }

        middleTextbox.text = string.Empty;
        textToWrite = phaseOneMessages[1];

        foreach (char letter in textToWrite)
        {
            bottomTextbox.text += letter;
            yield return new WaitForSeconds(secondsBetweenCharacters * speedUpMult);
        }

        yield return new WaitForSeconds(1);

        continuePressed = false;
        continueAvailable = true;

        yield return new WaitUntil(() => continuePressed);
        speedUpMult = 1;

        while (bottomTextbox.text.Length > 0)
        {
            bottomTextbox.text = bottomTextbox.text.Substring(0, bottomTextbox.text.Length - 1);

            yield return new WaitForSeconds(secondsBetweenCharacters * 0.5f * speedUpMult);
        }

        //-------------------------------SECOND IMAGE AND THIRD TEXT-------------

        HUDImage.sprite = scriptHudWithStuff;
        bottomTextbox.text = string.Empty;
        textToWrite = phaseOneMessages[2];

        foreach (char letter in textToWrite)
        {
            bottomTextbox.text += letter;
            yield return new WaitForSeconds(secondsBetweenCharacters * speedUpMult);
        }

        yield return new WaitForSeconds(1);

        continuePressed = false;
        continueAvailable = true;

        yield return new WaitUntil(() => continuePressed);
        speedUpMult = 1;
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
