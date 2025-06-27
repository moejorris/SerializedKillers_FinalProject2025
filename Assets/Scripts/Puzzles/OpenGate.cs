using UnityEngine;

public class OpenGate : MonoBehaviour
{
    // Number of buttons pushed required to open the gate
    private int buttonsPushed = 0;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void LiftUp()
    {
        buttonsPushed++;
        Debug.Log("Button pushed! Total buttons pushed: " + buttonsPushed);

        switch (buttonsPushed)
        {
            case 1:
                Debug.Log("Button pushed! Gate is opening slightly.");
                anim.SetInteger("OpenState", 1);
                break;
            case 2:
                Debug.Log("Button pushed! Gate is opening more.");
                anim.SetInteger("OpenState", 2);
                break;
            default:
                Debug.Log("Button pushed! Gate is fully open.");
                break;
        }
    }
}
