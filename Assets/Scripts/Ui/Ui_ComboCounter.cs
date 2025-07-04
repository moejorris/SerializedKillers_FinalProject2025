using TMPro;
using UnityEngine;

public class Ui_ComboCounter : MonoBehaviour
{
    public static Ui_ComboCounter instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] string countTextPrefix = "Combo:";
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] TextMeshProUGUI messageText;

    void Start()
    {
        HideComboCounter();
        messageText.text = "";
    }

    public void UpdateComboDisplay(int count)
    {
        if (count == 0)
        {
            HideComboCounter();
        }
        else countText.text = countTextPrefix + " " + count;

        if (messageText == null) return;

        string comboMessage = "";

        switch (count)
        {
            case > 25:
                comboMessage = "Outstanding!";
                break;

            case > 10:
                comboMessage = "Great!";
                break;

            case > 5:
                comboMessage = "Good";
                break;
        }

        messageText.text = comboMessage;
    }

    void HideComboCounter() //Change this to be fancy like slides in from off screen or something?????
    {
        countText.text = "";
        messageText.text = "";
    }
}
