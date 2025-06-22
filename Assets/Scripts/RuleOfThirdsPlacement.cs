using UnityEngine;

public class RuleOfThirdsPlacement : MonoBehaviour
{
    public GameObject LVLine;   //Left Verticle Line
    public GameObject RVLine;   //Right Verticle Line
    public GameObject THLine;   //Top Horizontal Line
    public GameObject BHLine;   //Bottom Horizontal Line

    public bool CompositionGuideOn;

    void FixedUpdate()
    {
        //Abiltiy to toggle compositonal guide in inspector
        if (CompositionGuideOn == false)
        {
            LVLine.SetActive(false);
            RVLine.SetActive(false);
            THLine.SetActive(false);
            BHLine.SetActive(false);
        }
        else
        {
            LVLine.SetActive(true);
            RVLine.SetActive(true);
            THLine.SetActive(true);
            BHLine.SetActive(true);
        }

        //Takes the current size of the camera
        var CanvasSizeX = transform.position.x * 2;
        var CanvasSizeY = transform.position.y * 2;

        //Sets the postion for the Verticle lines
        LVLine.transform.position = new Vector3(CanvasSizeX/3, 0f);
        RVLine.transform.position = new Vector3(CanvasSizeX - CanvasSizeX/3, 0f);

        //Sets the position for the Horizontal lines
        THLine.transform.position = new Vector3(0f, CanvasSizeY - CanvasSizeY / 3);
        BHLine.transform.position = new Vector3(0f, CanvasSizeY / 3);
    }
}
