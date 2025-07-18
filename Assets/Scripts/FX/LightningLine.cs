using UnityEngine;

public class LightningLine : MonoBehaviour
{
    [SerializeField] private LineRenderer lineTest;
    // How long is a frame
    [SerializeField] private float framerate = 24;
    // update every new frame, or every other frame, or...
    [SerializeField] private float updateAmount = 4;
    // how many frames are present in the animation/SpriteSheet
    [SerializeField] private float frameAmount = 3;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Offset line material to the next sprite in the texture. Update amount stays clamped until next frame should update. https://www.desmos.com/calculator/6lgxbtizuc
        // Created this way to use as little processing as possible, as calculations are faster than comparisons.
        float framerateStepAmount = 1 / framerate;
        
        lineTest.material.SetTextureOffset("_MainTex", Vector2.right * (1/frameAmount) * Mathf.Ceil(Time.time*(1/(framerateStepAmount*updateAmount))));
    }

}
