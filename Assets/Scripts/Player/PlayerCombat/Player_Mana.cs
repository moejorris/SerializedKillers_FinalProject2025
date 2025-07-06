using UnityEngine;
using UnityEngine.UI;

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
    public bool manaInUse = false;
    [SerializeField] private float maxMana = 100f;
    public float currentMana = 100f;
    public UsageType usageType;
    [SerializeField] private KeyCode timerToggleKey = KeyCode.T;
    public enum UsageType { PerUse, Timer }
    public bool scriptActive = false;

    private RectTransform manaBar => GameObject.FindGameObjectWithTag("Canvas").transform.Find("HUD/Mana/Bar").GetComponent<RectTransform>();
    private RectTransform whiteManaBar => GameObject.FindGameObjectWithTag("Canvas").transform.Find("HUD/Mana/WhiteBar").GetComponent<RectTransform>();
    [SerializeField] private float whiteManaBarMult = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (usageType == UsageType.PerUse)
        {
            whiteManaBar.GetComponent<Image>().enabled = true;
        }
        else
        {
            whiteManaBar.GetComponent<Image>().enabled = false;
        }

        if (whiteManaBar.localScale.x > manaBar.localScale.x)
        {
            float lerpScale = Mathf.Lerp(whiteManaBar.localScale.x, manaBar.localScale.x, whiteManaBarMult);
            Vector3 whiteBarScale = whiteManaBar.localScale;
            whiteBarScale.x = lerpScale;
            whiteManaBar.localScale = whiteBarScale;
            whiteManaBarMult += (Time.deltaTime / 25f);
        }
        else if (whiteManaBar.localScale.x < manaBar.localScale.x)
        {
            whiteManaBar.localScale = manaBar.localScale;
            whiteManaBarMult = 0;
        }

        if (usageType == UsageType.Timer)
        {
            if (scriptActive)
            {
                currentMana -= Time.deltaTime;
                if (currentMana < 0)
                {
                    scriptActive = false;
                    currentMana = 0;
                }
                UpdateUI();
            }
        }

        if (Input.GetKeyDown(timerToggleKey))
        {
            scriptActive = !scriptActive;
            PlayerController.instance.ScriptSteal.UpdateUI();
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

        if (currentMana < 0) currentMana = 0;

        whiteManaBarMult = 0;
        UpdateUI();
    }

    public void UpdateUI()
    {
        Vector3 scale = manaBar.localScale;
        scale.x = currentMana / maxMana;
        manaBar.localScale = scale;
    }

    public bool ElementActive()
    {
        if (usageType == UsageType.PerUse && currentMana >= generalCost || usageType == UsageType.Timer && currentMana >= 0) return true;
        else return false;
    }
}
