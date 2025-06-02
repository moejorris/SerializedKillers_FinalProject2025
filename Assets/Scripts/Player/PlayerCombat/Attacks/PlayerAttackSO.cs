using UnityEngine;

[CreateAssetMenu(menuName = "Player Attacks/Normal Attack")]
public class PlayerAttackSO : ScriptableObject
{
    public float damage;
    public AnimationClip animation;
    public bool usesRootMotion;
}
