using UnityEngine;

public class WeightHoldingIceChunk : IceChunk
{
    [SerializeField] private Animator weight;

    public override void PerformAction()
    {
        WeightFall();
    }
    public void WeightFall()
    {
        if (weight != null)
        {
            weight.Play("WeightFall1");
        }
    }
}
