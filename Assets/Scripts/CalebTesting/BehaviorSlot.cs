using UnityEngine;
using UnityEngine.UI;

public class BehaviorSlot : MonoBehaviour
{
    public Image slotIcon;
    public Behavior heldBehavior;
    public Sprite emptySlot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slotIcon = transform.Find("Icon").GetComponent<Image>();
        UpdateSlot();
    }

    public Behavior SwapBehavior(Behavior newBehavior)
    {
        if (heldBehavior != null)
        {
            Behavior outGoingBehavior = heldBehavior;
            heldBehavior = newBehavior;
            UpdateSlot();
            return outGoingBehavior;
        }
        else
        {
            heldBehavior = newBehavior;
            UpdateSlot();
            return null;
        }
        
    }

    public void RemoveBehavior()
    {
        heldBehavior = null;
        UpdateSlot();
    }

    public void AddBehavior(Behavior newBehavior)
    {
        heldBehavior = newBehavior;
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (heldBehavior != null)
        {
            slotIcon.sprite = heldBehavior.behavioricon;
        }
        else
        {
            slotIcon.sprite = emptySlot;
        }
    }
}
