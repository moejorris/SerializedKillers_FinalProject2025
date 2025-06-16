using UnityEngine;

[CreateAssetMenu(menuName = "Player Attacks/Normal Attack")]

//Joe Morris
public class PlayerAttackSO : ScriptableObject
{
    public float damage;
    public AnimationClip animation;
    public float animationSpeed = 1;
    public bool overrideMotion;
    public bool usesRootMotion;
    public Vector3 vectorForce;
}
