using UnityEngine;

[CreateAssetMenu(menuName = "Player Attacks/Normal Attack")]
public class PlayerAttackSO : ScriptableObject
{
    public float damage;
    public AnimationClip animation;
    public float animationSpeed = 1;
    public bool usesRootMotion;
}
