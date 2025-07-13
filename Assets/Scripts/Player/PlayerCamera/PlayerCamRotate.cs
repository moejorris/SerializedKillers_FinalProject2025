using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Joe Morris
/*
TODO 7/5/25
Tutorial UI
Auto Rotate Cam (combat) make it slower
*/
public class PlayerCamRotate : MonoBehaviour
{
    public static PlayerCamRotate instance;

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

    enum CamMode { Manual, Traversal, Combat, Cutscene }
    CamMode currentMode = CamMode.Manual;
    Vector2 input;
    bool unscaledTime = false;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;

        }

        instance = this;   
    }

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
        UpdateCamera();
        if (enableCollision && currentMode != CamMode.Cutscene) Collide();
    }

    void UpdateCamera()
    {
        switch (currentMode)
        {
            case CamMode.Manual:
                ManualCam();
                break;

            case CamMode.Traversal:
                TraversalAutoCam();
                break;

            case CamMode.Combat:
                CombatAutoCam(); //currently this just calls TraversalAutoCam(), but this should be changed later to have a different camera setting
                break;

            case CamMode.Cutscene:
                //it runs in a coroutine so we can ignore this...
                break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * camLocalPos.z));
    }

    void CamInput()
    {
        if (currentMode == CamMode.Cutscene) return;

        input = cameraInput.action.ReadValue<Vector2>();

        if (input.magnitude < 0.01f) //if didn't input, with a very slight deadzone
        {
            didntInputThisFrame = true;
        }
        else
        {
            CancelAutoRotate();
        }

        if (GetComponent<PlayerInput>()?.currentControlScheme != "Keyboard&Mouse") //magnitude should only exceed 1 if using the mouse
        {
            input *= joystickSensitivity;
        }
        else
        {
            input *= mouseSensitivity;
        }

        AutoCamCheck();
    }

    void ManualCam()
    {
        if (unscaledTime)
        {
            xRot -= input.y * Time.unscaledDeltaTime;
            yRot += input.x * Time.unscaledDeltaTime;
            return;
        }


        xRot -= input.y * Time.deltaTime;
        yRot += input.x * Time.deltaTime;

        UpdateTransform();
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

    void AutoCamCheck()
    {
        if (!didntInputThisFrame) return;

        if (!isAutoRotating && !autoRotateQueued)
        {
            autoRotateQueued = true;
            CancelInvoke("StartAutoRotate");
            Invoke("StartAutoRotate", 2.5f);
            return;
        }
    }

    void TraversalAutoCam()
    {
        xRot = Mathf.Lerp(xRot, defaultCamRotationX, 10f * Time.deltaTime);

        Vector3 eulerAngles = transform.eulerAngles;

        transform.forward = PlayerController.instance.MovementMachine.ForwardDirection;

        float angleDistance = Mathf.Abs(Mathf.DeltaAngle(yRot, transform.eulerAngles.y)); //how far the camera has to go in degrees before reaching destination angle

        if (angleDistance > autoRotateAngleThreshold)
        {
            angleDistance *= angleDistanceFactor;

            yRot = Mathf.MoveTowardsAngle(yRot, transform.eulerAngles.y, (autoRotateSpeedY + angleDistance) * Time.deltaTime);

            transform.eulerAngles = eulerAngles;
        }

        UpdateTransform();
    }

    public void StartCutscene(Vector3 destinationPosition, Quaternion destinationRotation, float startDelayTime = 0f, float travelToTime = 0f, float holdTime = 1.5f, float travelReturnTime = 0.5f, float endDelayTime = 0.1f)
    {
        if (destinationPosition == null || destinationRotation == null)
        {
            Debug.LogError("Cutscene Cam: No destination position/rotation provided.");
            return;
        }

        StartCoroutine(CutsceneCamera(destinationPosition, destinationRotation, startDelayTime, travelToTime, holdTime, travelReturnTime, endDelayTime));
    }

    IEnumerator CutsceneCamera(Vector3 destinationPosition, Quaternion destinationRotation, float startDelayTime = 0.5f, float travelToTime = 0.25f, float holdTime = 1.5f, float travelReturnTime = 0.5f, float endDelayTime = 0.1f)
    {
        currentMode = CamMode.Cutscene;
        Vector3 startPoint = cameraTargetPosition.position;
        Quaternion startRotation = cameraTargetPosition.rotation;

        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(startDelayTime);

        float t = 0;

        while (t < travelToTime)
        {
            cameraTargetPosition.position = Vector3.Lerp(startPoint, destinationPosition, t / travelToTime);
            cameraTargetPosition.rotation = Quaternion.Slerp(startRotation, destinationRotation, t / travelToTime);

            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            t += Time.unscaledDeltaTime;
        }

        cameraTargetPosition.position = destinationPosition;
        cameraTargetPosition.rotation = destinationRotation;

        yield return new WaitForSecondsRealtime(holdTime);

        t = 0;

        while (t < travelReturnTime)
        {
            cameraTargetPosition.position = Vector3.Lerp(destinationPosition, startPoint, t / travelReturnTime);
            cameraTargetPosition.rotation = Quaternion.Slerp(destinationRotation, startRotation, t / travelReturnTime);

            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            t += Time.unscaledDeltaTime;
        }

        cameraTargetPosition.position = startPoint;
        cameraTargetPosition.rotation = startRotation;

        yield return new WaitForSecondsRealtime(endDelayTime);

        Time.timeScale = 1;
        currentMode = CamMode.Traversal;
    }

    void CombatAutoCam()
    {
        TraversalAutoCam();
    }

    void StartAutoRotate()
    {
        isAutoRotating = true;
        autoRotateQueued = false;
        currentMode = CamMode.Traversal;
    }

    void CancelAutoRotate()
    {
        isAutoRotating = false;
        autoRotateQueued = false;
        CancelInvoke("StartAutoRotate");
        currentMode = CamMode.Manual;
    }
}
