using UnityEngine;

public class RootMotion_Translator : MonoBehaviour
{
    //Translates the motion used in a root motion animation to readable values that can be applied to Rigidbodies, Character Controllers, and Transforms other than what is moved in the Animation. Developed for use with the Player Character's Attack Animations.
    Animator animator;

    public Vector3 RootMovement { get => _moveForce; }
    public Quaternion RootRotation { get => _rotation; }

    Vector3 _moveForce;
    Quaternion _rotation;

    void Awake() => animator = GetComponent<Animator>();

    void OnAnimatorMove()
    {
        if (!animator) return; //this should never happen because this function will only be called if there is an animator on the object, but it never hurts to have a failsafe.

        _moveForce = animator.deltaPosition;
        _rotation = animator.deltaRotation;
    }
}
