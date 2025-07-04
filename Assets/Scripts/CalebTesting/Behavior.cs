using UnityEngine;

[CreateAssetMenu(fileName = "Behavior", menuName = "Scriptable Objects/Behavior")]
public class Behavior : ScriptableObject
{
    public Sprite deactivatedBehaviorIcon;
    public Sprite activatedBehaviorIcon;
    public Sprite[] animation;
    public string behaviorName;
    public Color color;
    public Behavior weakness;
}
