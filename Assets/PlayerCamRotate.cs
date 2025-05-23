using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamRotate : MonoBehaviour
{
    [SerializeField] Vector2 upDownLimit = new Vector2(30, 90);
    [SerializeField] float joystickSensitivity = 100f, mouseSensitivity = 30f;
    [SerializeField] InputActionReference cameraInput;
    [SerializeField] bool enableCollision;
    [SerializeField] Transform cameraTargetPosition;

    Vector3 camLocalPos;

    float xRot = 0;
    float yRot = 0;

    void Start()
    {
        xRot = transform.eulerAngles.x;
        yRot = transform.eulerAngles.y;
        camLocalPos = cameraTargetPosition.localPosition;

        // Hi joe I added this because mouse go way off screen otherwise teehee sorry!
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = cameraInput.action.ReadValue<Vector2>();

        if (input.magnitude <= 1)
        {
            input *= joystickSensitivity;
        }
        else
        {
            input *= mouseSensitivity;
        }

        xRot -= input.y * Time.deltaTime;
        yRot += input.x * Time.deltaTime;



        if (Mathf.Abs(yRot) >= 360)
        {
            yRot = 0;
        }

        xRot = Mathf.Clamp(xRot, upDownLimit.x, upDownLimit.y);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(xRot, yRot, 0), 10);

        if (enableCollision) Collide();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * camLocalPos.z));
    }

    void Collide()
    {
        RaycastHit hitInfo;

        if (Physics.Linecast(transform.position, transform.position + (transform.forward * camLocalPos.z), out hitInfo, LayerMask.NameToLayer("Player"), QueryTriggerInteraction.Ignore))
        {
            cameraTargetPosition.position = hitInfo.point;
        }
        else
        {
            cameraTargetPosition.localPosition = camLocalPos;
        }
    }
}
