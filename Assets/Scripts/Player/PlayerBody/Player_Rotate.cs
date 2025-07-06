using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Rotate : MonoBehaviour
{
    [SerializeField] InputActionReference walkInput;

    [SerializeField] AnimationCurve rotationBySpeed; //how quickly to rotate based on speed
    [SerializeField] float rotationSpeed = 8f; //scalar to adjust how quickly the rotation lerps

    void Awake()
    {

    }

    void Update()
    {
        //Walk Rotation
        Vector3 newDir = PlayerController.instance.MovementMachine.ForwardDirection;
        Vector3 desiredDir = IntendedMoveDirection();
        float rotLerpValue = Mathf.Clamp(rotationBySpeed.Evaluate(PlayerController.instance.Walk.GetNormalizedSpeed()), 0, 1f);

        newDir = Vector3.Slerp(newDir, desiredDir, rotLerpValue * PlayerController.instance.MovementMachine.DeltaTime * rotationSpeed);

        // Debug.Log("Lerp Value: " + rotLerpValue * machine.DeltaTime * rotationSpeed + " & Delta Time: " + machine.DeltaTime);

        if (newDir.magnitude > 0.1f)
            PlayerController.instance.MovementMachine.SetForwardDirection(newDir);
    }
    
    Vector3 IntendedMoveDirection()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);

        Vector2 inputDirection = walkInput.action.ReadValue<Vector2>();
        return inputDirection.x * camRight.normalized + inputDirection.y * camForward.normalized;
    }
}
