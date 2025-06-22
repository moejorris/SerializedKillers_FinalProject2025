using UnityEngine;

[CreateAssetMenu(menuName = "Player Attacks/Normal Attack")]

//Joe Morris
public class PlayerAttackSO : ScriptableObject
{
    [Header("Damaging")]
    public float damage;
    public float hitboxDelay;
    public float hitboxDuration;

    [Header("Animation")]
    public AnimationClip animation;
    public float animationSpeed = 1;

    [Header("Player Movement")]
    public bool overrideMotion;
    public bool usesRootMotion;
    public Vector3 vectorForce;

    [Header("Enemy Movement")]
    public float knockback;

    [Header("Cosmetic")]
    public GameObject particleEffect;
    
    [Header("Sound")]
    public AudioClip swingSound;
    public AudioClip impactSound;

}
