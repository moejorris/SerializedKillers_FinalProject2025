using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Rotate : MonoBehaviour
{
    [SerializeField] InputActionReference walkInput;
    Player_MovementMachine machine;
    Player_Walk walk;
    [SerializeField] AnimationCurve rotationBySpeed; //how quickly to rotate based on speed
    [SerializeField] float rotationSpeed = 8f; //scalar to adjust how quickly the rotation lerps

    void Awake()
    {
        machine = GetComponent<Player_MovementMachine>();
        walk = GetComponent<Player_Walk>();   
    }

    void Update()
    {
        //Walk Rotation
        Vector3 newDir = machine.ForwardDirection;
        Vector3 desiredDir = IntendedMoveDirection();
        float rotLerpValue = Mathf.Clamp(rotationBySpeed.Evaluate(walk.GetNormalizedSpeed()), 0, 0.8f);

        newDir = Vector3.Slerp(newDir, desiredDir, rotLerpValue * machine.DeltaTime * rotationSpeed);

        if (newDir.magnitude > 0.1f)
            machine.SetForwardDirection(newDir);
    }
    
    Vector3 IntendedMoveDirection()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);

        Vector2 inputDirection = walkInput.action.ReadValue<Vector2>();
        return inputDirection.x * camRight.normalized + inputDirection.y * camForward.normalized;
    }
}
