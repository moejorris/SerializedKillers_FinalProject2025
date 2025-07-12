using UnityEngine;

public class WeightHoldingIceChunk : IceChunk
{
    [SerializeField] private Animator weight;
    private bool fallen = false;
    private LaserPuzzleRoom puzzleRoom => transform.parent.GetComponent<LaserPuzzleRoom>();

    public override void PerformAction()
    {
        if (snow != null) snow.Stop();
        WeightFall();
    }
    public void WeightFall()
    {
        if (fallen) return;
        fallen = true;

        if (weight != null)
        {
            weight.Play("WeightFall1");
            puzzleRoom.WeightFallen();
        }
    }
}
