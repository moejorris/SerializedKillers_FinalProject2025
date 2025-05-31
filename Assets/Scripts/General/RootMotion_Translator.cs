using UnityEngine;

public class RootMotion_Translator : MonoBehaviour
{
    //Translates the motion used in a root motion animation to readable values that can be applied to Rigidbodies, Character Controllers, and Transforms other than what is moved in the Animation. Developed for use with the Player Character's Attack Animations.
    [SerializeField] Animator animator;

    public Vector3 RootMovement { get => _moveForce; }
    public Quaternion RootRotation { get => _rotation; }

    Vector3 _moveForce;
    Quaternion _rotation;

    void OnAnimatorMove()
    {
        _moveForce = animator.deltaPosition;
        _rotation = animator.deltaRotation;
    }
}
