using TreeEditor;
using UnityEngine;
using UnityEngine.InputSystem;

//Joe Morris
/*
TODO 7/5/25
Tutorial UI
Auto Rotate Cam (traversal and combat) make it slower
Player Checkpoints
Mana
Combo COunter
*/
public class PlayerCamRotate : MonoBehaviour
{
    Player_MovementMachine _moveMachine => transform.parent.GetComponentInChildren<Player_MovementMachine>();
    [SerializeField] Vector2 upDownLimit = new Vector2(30, 90);
    [SerializeField] float joystickSensitivity = 100f, mouseSensitivity = 30f;
    [SerializeField] InputActionReference cameraInput;
    [SerializeField] bool enableCollision;
    [SerializeField] Transform cameraTargetPosition;

    [SerializeField] bool autoRotateCam = true;
    [SerializeField] float defaultCamRotationX = 30f;
    [SerializeField] float autoRotateSpeedY = 1f;
    [SerializeField] float autoRotateAngleThreshold = 15f;
    [SerializeField] float angleDistanceFactor = 1f;

    bool didntInputThisFrame = false;
    bool isAutoRotating = false;
    bool autoRotateQueued = false;


    Vector3 camLocalPos;

    float xRot = 0;
    [SerializeField] float yRot = 0;

    enum CamMode {Manual, Traversal, Combat}
    CamMode currentMode = CamMode.Manual;

    bool unscaledTime = false;

    void Start()
    {
        if (FindAnyObjectByType<Dev_TimeScaleController>() != null)
        {
            unscaledTime = true;
        }

        xRot = transform.eulerAngles.x;
        yRot = transform.eulerAngles.y;
        camLocalPos = cameraTargetPosition.localPosition;

        // Hi joe I added this because mouse go way off screen otherwise teehee sorry! - caleb
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CamInput();

        AutoRotate();

        UpdateTransform();

        if (enableCollision) Collide();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * camLocalPos.z));
    }

    void CamInput()
    {
        Vector2 input = cameraInput.action.ReadValue<Vector2>();

        if (input.magnitude < 0.01f) //if didn't input, with a very slight deadzone
        {
            didntInputThisFrame = true;
        }
        else
        {
            CancelAutoRotate();
        }


        if (input.magnitude <= 1) //magnitude should only exceed 1 if using the mouse
        {
            input *= joystickSensitivity;
        }
        else
        {
            input *= mouseSensitivity;
        }

        if (unscaledTime)
        {
            xRot -= input.y * Time.unscaledDeltaTime;
            yRot += input.x * Time.unscaledDeltaTime;
            return;
        }


        xRot -= input.y * Time.deltaTime;
        yRot += input.x * Time.deltaTime;
    }

    void UpdateTransform()
    {
        if (Mathf.Abs(yRot) >= 360)
        {
            yRot -= Mathf.Sign(yRot) * 360f;
        }


        xRot = Mathf.Clamp(xRot, upDownLimit.x, upDownLimit.y);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(xRot, yRot, 0), 10);
    }

    void Collide()
    {
        RaycastHit hitInfo;

        if (Physics.Linecast(transform.position, transform.position + (transform.forward * camLocalPos.z), out hitInfo, LayerMask.NameToLayer("Player"), QueryTriggerInteraction.Ignore) && hitInfo.collider.GetComponent<IDamageable>() == null)
        {
            cameraTargetPosition.position = hitInfo.point;
        }
        else
        {
            cameraTargetPosition.localPosition = camLocalPos;
        }
    }

    void AutoRotate()
    {
        if (!didntInputThisFrame) return;

        if (!isAutoRotating && !autoRotateQueued)
        {
            autoRotateQueued = true;
            CancelInvoke("StartAutoRotate");
            Invoke("StartAutoRotate", 2.5f);
            return;
        }

        if (isAutoRotating)
        {
            xRot = Mathf.Lerp(xRot, defaultCamRotationX, 10f * Time.deltaTime);



            Vector3 eulerAngles = transform.eulerAngles;

            transform.forward = _moveMachine.ForwardDirection;

            float angleDistance = Mathf.Abs(Mathf.DeltaAngle(yRot, transform.eulerAngles.y)); //how far the camera has to go in degrees before reaching destination angle

            if (angleDistance <= autoRotateAngleThreshold) return;

            angleDistance *= angleDistanceFactor;

            yRot = Mathf.MoveTowardsAngle(yRot, transform.eulerAngles.y, (autoRotateSpeedY + angleDistance) * Time.deltaTime);

            transform.eulerAngles = eulerAngles;
        }
    }

    void StartAutoRotate()
    {
        isAutoRotating = true;
        autoRotateQueued = false;
    }

    void CancelAutoRotate()
    {
        isAutoRotating = false;
        autoRotateQueued = false;
        CancelInvoke("StartAutoRotate");
    }
}
