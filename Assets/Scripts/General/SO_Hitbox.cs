using UnityEngine;

[CreateAssetMenu(menuName = "Player Attacks/Hitbox")]

public class SO_Hitbox : ScriptableObject
{
    public enum HitboxShape { Box, Sphere }
    public Color gizmoColor = Color.green;

    public HitboxShape shape;
    public  float size = 1f;
    public GameObject parentPrefab;
    public string parentBone;
    public  Vector3 position;
    public  float damage = 10;
}
