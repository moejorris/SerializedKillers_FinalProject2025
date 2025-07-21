using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class Player_Mana : MonoBehaviour
{


    [Header("Things Requiring Mana")]
    [SerializeField] private bool specialAttacks_req = false;
    [SerializeField] private bool regularAttacks_req = false;
    [SerializeField] private bool dashAttacks_req = false;
    [SerializeField] private bool damageToPlayer_req = false;

    [SerializeField] private float generalCost = 10f;

    [SerializeField] private float specialAttackCost = 10f;
    [SerializeField] private float regularAttackCost = 5f;
    [SerializeField] private float dashAttackCost = 15f;
    [SerializeField] private float playerDamageCost = 5f;

    [Header("Mana Settings")]
    //public bool manaInUse = false;
    public InputActionReference scriptToggleButton; // Pressing Q or Top Left Bumper
    [SerializeField] private float maxMana = 100f;
    public float currentMana = 100f;
    public UsageType usageType;
    [SerializeField] private KeyCode timerToggleKey = KeyCode.T;
    public enum UsageType { PerUse, Timer }
    public bool scriptActive = false;

    private RectTransform manaBar => GameObject.FindGameObjectWithTag("Canvas").transform.Find("HUD/Mana/Bar").GetComponent<RectTransform>();
    private Animator barAnimator => manaBar.GetComponent<Animator>();
    private RectTransform whiteManaBar => GameObject.FindGameObjectWithTag("Canvas").transform.Find("HUD/Mana/WhiteBar").GetComponent<RectTransform>();
    [SerializeField] private float manaBarLerpSpeed = 0f;

    [Header("SFX")]
    private AudioSource manaAudio;
    [SerializeField] private SoundEffectSO sfx_scriptOn;
    [SerializeField] private SoundEffectSO sfx_scriptOff;
    [SerializeField] private SoundEffectSO sfx_manaHum;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateUI();
        StartCoroutine("ManaOnLoop");
        manaAudio = GameObject.FindGameObjectWithTag("Canvas").transform.Find("HUD/Mana").GetComponent<AudioSource>();
        manaAudio.volume = SoundManager.instance.GetSFXVolume();
    }

    // Update is called once per frame
    void Update()
    {
        if (manaBar.localScale.x < whiteManaBar.localScale.x)
        {
            float lerpScale = Mathf.Lerp(manaBar.localScale.x, whiteManaBar.localScale.x, manaBarLerpSpeed);
            Vector3 manaBarScale = manaBar.localScale;
            manaBarScale.x = lerpScale;
            manaBar.localScale = manaBarScale;
            if (manaBar.localScale.x >= whiteManaBar.localScale.x - 0.005f) manaBar.localScale = whiteManaBar.localScale;
            manaBarLerpSpeed += (Time.deltaTime / 15f);
        }
        else if (manaBar.localScale.x >= whiteManaBar.localScale.x)
        {
            manaBar.localScale = whiteManaBar.localScale;
            manaBarLerpSpeed = 0;
        }

        if (usageType == UsageType.Timer)
        {
            if (scriptActive)
            {
                currentMana -= Time.deltaTime * 2.5f;
                if (currentMana < 0)
                {
                    scriptActive = false;
                    currentMana = 0;
                    //PlayerController.instance.ScriptSteal.UpdateUI();
                    PlayerController.instance.ScriptSteal.ApplyScriptEffects();
                }
                UpdateUI();
            }
        }

        if (scriptToggleButton.action.WasPressedThisFrame()) // Checks if the player has pressed Y and toggles menu
        {
            ToggleMana();
        }
    }

    public void ToggleMana()
    {
        if (PlayerController.instance.ScriptSteal.heldBehavior != null && currentMana > 0)
        {
            scriptActive = !scriptActive;

            if (scriptActive) ScriptActivateSound();
            else ScriptDeactivateSound();

            barAnimator.SetBool("Active", scriptActive);
            //PlayerController.instance.ScriptSteal.UpdateUI();
            PlayerController.instance.ScriptSteal.ApplyScriptEffects();
        }
        else
        {
            ScriptDeactivateSound();

            barAnimator.SetBool("Active", false);
            scriptActive = false;
            //PlayerController.instance.ScriptSteal.UpdateUI();
            PlayerController.instance.ScriptSteal.ApplyScriptEffects();
        }
    }

    public void GainMana(float mana = 15)
    {
        currentMana += mana;

        if (currentMana > maxMana) currentMana = maxMana;

        UpdateUI();
    }

    public void UseMana(float mana = 10)
    {
        currentMana -= mana;

        if (currentMana <= 0)
        {
            scriptActive = false;
            currentMana = 0;
            //PlayerController.instance.ScriptSteal.UpdateUI();
            PlayerController.instance.ScriptSteal.ApplyScriptEffects();
        }

        manaBarLerpSpeed = 0;
        UpdateUI();
    }

    public void UpdateUI()
    {
        Vector3 scale = whiteManaBar.localScale;
        scale.x = currentMana / maxMana;
        whiteManaBar.localScale = scale;
    }

    public bool ElementActive()
    {
        if (usageType == UsageType.PerUse && currentMana >= generalCost || usageType == UsageType.Timer && currentMana >= 0) return true;
        else return false;
    }

    IEnumerator ManaOnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            manaAudio.pitch = Mathf.Lerp(0.8f, 1.5f, (currentMana / 100));
            manaAudio.volume = SoundManager.instance.GetSFXVolume();
        }
    }

    public void ScriptActivateSound()
    {
        SoundManager.instance.PlaySoundEffect(sfx_scriptOn);
        manaAudio.Play();
    }

    public void ScriptDeactivateSound()
    {
        SoundManager.instance.PlaySoundEffect(sfx_scriptOff);
        manaAudio.Pause();
    }
}
