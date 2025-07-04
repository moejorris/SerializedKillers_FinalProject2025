using UnityEngine;

public interface ITargetable
{
    public float TargetScore { get; set; }
    Transform transform { get; }
    public float TargetScoreWeight { get => 1f;}
}
