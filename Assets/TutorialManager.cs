using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private TMP_Text middleTextbox;
    [SerializeField] private TMP_Text bottomTextbox;
    [SerializeField] private TMP_Text continueText;
    [SerializeField] private Sprite scriptHudWithStuff;
    [SerializeField] private Image HUDImage;
    private AudioSource audioSource => GetComponent<AudioSource>();
    private bool continueAvailable = false;
    private bool continuePressed = false;

    private bool tutorialFinished = false;
    [SerializeField] private Animator panelAnimator;
    [SerializeField] private float secondsBetweenCharacters = 0.01f;
    [SerializeField] private float speedUpMult = 1;

    [Header("Phase One")]
    [SerializeField] private string[] phaseOneMessages;
    [SerializeField] Sprite[] phaseOneSprites;
    [SerializeField] bool[] phaseOneTinyPanel;


    [Header("Phase Two")]
    [SerializeField] private string[] phaseTwoMessages;
    [SerializeField] Sprite[] phaseTwoSprites;
    [SerializeField] bool[] phaseTwoTinyPanel;

    [Header("Phase Three")]
    [SerializeField] private string[] phaseThreeMessages;
    [SerializeField] Sprite[] phaseThreeSprites;
    [SerializeField] bool[] phaseThreeTinyPanel;
    [SerializeField] private Lever basementCellLever;

    [Header("Phase Four")]
    [SerializeField] private string[] phaseFourMessages;
    [SerializeField] Sprite[] phaseFourSprites;
    [SerializeField] bool[] phaseFourTinyPanel;

    private int phase = 0;
    private string textToWrite;
    private bool cont = false;
    public bool isRunning = false;

    [Header("ControllerKeyboardExchange")]
    [SerializeField] private string[] keyboardInputs;
    [SerializeField] private string[] controllerEquivalent;

    [Header("Enemies")]
    [SerializeField] private GameObject tutorialOverclockPrefab;
    [SerializeField] private Transform spawnPos;

    [Header("PuzzleElements")]
    [SerializeField] private Animator basementGate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPhaseOne();
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorialFinished) return;

        if (jumpAction.action.WasPerformedThisFrame())
        {
            if (continueAvailable)
            {
                continueAvailable = false;
                continuePressed = true;
            }
            else
            {
                speedUpMult = 0.25f;
            }
        }

        if (isRunning && Time.timeScale == 0)
        {
            audioSource.pitch = 0;
        }
        else audioSource.pitch = 1;
    }

    public void StartPhaseOne()
    {
        phase = 1;
        StartCoroutine(TutorialPhase(phase));
    }

    public void StartPhaseTwo()
    {
        if (phase >= 2) return;

        StartCoroutine(TutorialPhase(2));
        phase = 2;
    }

    public void StartPhaseThree()
    {
        if (phase >= 3) return;

        StartCoroutine(TutorialPhase(3));
        phase = 3;
    }
    public void StartPhaseFour()
    {
        if (phase >= 4) return;

        StartCoroutine(TutorialPhase(4));
        phase = 4;
    }

    IEnumerator ContinuePromptFlash()
    {
        bool increasing = true;
        Color aColor = continueText.color;
        aColor.a = 0;
        continueText.color = aColor;

        while (!continuePressed || aColor.a > 0) // this SHOULD only run until the continue is pressed and then once it is run until it's returned back to invis. - Joe
        {

            aColor.a += increasing ? 0.1f : -0.1f;
            continueText.color = aColor;

            yield return new WaitForSeconds(0.1f * speedUpMult);

            if (aColor.a > 1) increasing = false;
            else if (aColor.a < 0) increasing = true;
        }
    }

    string[] GetPhaseMessages(int phaseNumber = 1)
    {
        string[] arrayToReturn = new string[0];

        switch (phaseNumber)
        {
            case 1:
                arrayToReturn = phaseOneMessages;
                break;

            case 2:
                arrayToReturn = phaseTwoMessages;
                break;

            case 3:
                arrayToReturn = phaseThreeMessages;
                break;

            case 4:
                arrayToReturn = phaseFourMessages;
                break;

            default:
                arrayToReturn[0] = "No array created for phase " + phaseNumber + ". Please create a new one and add it to the switch statement in GetPhaseMessages().";
                arrayToReturn[1] = "Try again!";
                break;
        }

        return arrayToReturn;
    }

    Sprite[] GetPhaseSprites(int phaseNumber = 1)
    {
        Sprite[] arrayToReturn = null;

        switch (phaseNumber)
        {
            case 1:
                arrayToReturn = phaseOneSprites;
                break;

            case 2:
                arrayToReturn = phaseTwoSprites;
                break;

            case 3:
                arrayToReturn = phaseThreeSprites;
                break;

            case 4:
                arrayToReturn = phaseFourSprites;
                break;

            default:
                //No Sprite Array Created
                break;
        }

        return arrayToReturn;
    }

    bool[] GetPhaseSizes(int phaseNumber = 1)
    {
        bool[] arrayToReturn = new bool[0];

        switch (phaseNumber)
        {
            case 1:
                arrayToReturn = phaseOneTinyPanel;
                break;

            case 2:
                arrayToReturn = phaseTwoTinyPanel;
                break;

            case 3:
                arrayToReturn = phaseThreeTinyPanel;
                break;
            case 4:
                arrayToReturn = phaseFourTinyPanel;
                break;

            default:

                break;
        }

        return arrayToReturn;
    }

    IEnumerator TutorialPhase(int phaseNumber = 1)
    {
        yield return new WaitForSeconds(1);

        string[] phaseMessages = GetPhaseMessages(phaseNumber);
        Sprite[] phaseSprites = GetPhaseSprites(phaseNumber);
        bool[] phasePanelSizes = GetPhaseSizes(phaseNumber);

        for (int i = 0; i < phaseMessages.Length; i++)
        {
            StopCoroutine("TypePhaseText");
            yield return TypePhaseText(phaseMessages[i], phaseSprites[i], i == phaseMessages.Length - 1, phasePanelSizes[i]);
        }
    }

    IEnumerator TypePhaseText(string phaseStepText, Sprite sprite, bool isLast = false, bool tinyPanel = false) //this is the logic used for all phases. This is run for each element in the phaseMessages array per phase. 
    {
        isRunning = true;

        speedUpMult = 1;

        Color aColor = HUDImage.color;

        if (phaseStepText.Contains("/ALPHA/")) //Add /ALPHA/ if you want to re-fade in the current image when the next image doesn't match the previous.
        {
            aColor.a = 0;
            HUDImage.color = aColor;

            phaseStepText = phaseStepText.Replace("/ALPHA/", "");
        }

        TMP_Text textBox = middleTextbox;

        if (sprite != null)
        {
            HUDImage.sprite = sprite;

            while (HUDImage.color.a < 1)
            {
                aColor = HUDImage.color;
                aColor.a += 0.1f;
                yield return new WaitForSeconds(0.1f * speedUpMult);
                HUDImage.color = aColor;
            }
            aColor.a = 1;
            HUDImage.color = aColor;

            textBox = bottomTextbox;
        }
        else
        {
            aColor.a = 0;
            HUDImage.color = aColor;
        }


        bool beginningEvent = false;
        bool endEvent = false;
        if (phaseStepText.StartsWith('*'))
        {
            phaseStepText = phaseStepText.Replace("*", "");
            endEvent = false;
            beginningEvent = true;
        }
        else if (phaseStepText.EndsWith("*"))
        {
            phaseStepText = phaseStepText.Replace("*", "");
            beginningEvent = false;
            endEvent = true;
        }

        if (!PlayerController.instance.ScriptSteal.InputIsKeyboard())
        {
            for (int i = 0; i < keyboardInputs.Length; i++)
            {
                if (phaseStepText.Contains($"[{keyboardInputs[i]}]"))
                {
                    phaseStepText = phaseStepText.Replace($"[{keyboardInputs[i]}]", $"[{controllerEquivalent[i]}]");
                }
            }

            continueText.text = "Press [A] to continue...";
        }
        else
        {
            continueText.text = "Press [SPACE] to continue...";
        }

        yield return new WaitForSeconds(0.5f);
        panelAnimator.SetBool("Small", tinyPanel);

        textBox.text = "";
        textToWrite = phaseStepText;
        panelAnimator.SetBool("Appear", true);

        yield return new WaitForSeconds(0.7f);

        if (beginningEvent) SpecialEvent();

        audioSource.Stop();
        audioSource.volume = SoundManager.instance.SFXVolume;
        audioSource.loop = true;
        audioSource.Play();

        //type the text
        foreach (char letter in textToWrite)
        {
            textBox.text += letter;
            yield return new WaitForSeconds(secondsBetweenCharacters * speedUpMult);
        }

        audioSource.Stop();

        yield return new WaitForSeconds(1);

        continuePressed = false;
        continueAvailable = true;

        StartCoroutine(ContinuePromptFlash());

        yield return new WaitUntil(() => continuePressed);
        speedUpMult = 0.1f;

        audioSource.volume = SoundManager.instance.SFXVolume;
        audioSource.Play();

        //remove the text
        while (textBox.text.Length > 0)
        {
            textBox.text = textBox.text.Substring(0, textBox.text.Length - 1);

            yield return new WaitForSeconds(secondsBetweenCharacters * speedUpMult);
        }

        audioSource.Stop();

        if (HUDImage.color.a > 0) // separated from isLast check due to there being cases in which the image must fade without it being last
        {
            while (HUDImage.color.a > 0)
            {
                aColor = HUDImage.color;
                aColor.a -= 0.1f;
                yield return new WaitForSeconds(0.1f * speedUpMult);
                HUDImage.color = aColor;
            }
        }

        if (isLast) panelAnimator.SetBool("Appear", false);

        if (endEvent) SpecialEvent();

        isRunning = false;
    }

    public void SpecialEvent()
    {
        switch (phase)
        {
            case 1:

                break;
            case 2:
                basementGate.SetBool("Open", true);
                break;
            case 3:
                basementCellLever.EnableLever();
                break;
            default: break;
        }
    }
}
