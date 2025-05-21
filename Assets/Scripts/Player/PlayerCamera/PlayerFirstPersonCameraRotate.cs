using UnityEngine;

public class PlayerFirstPersonCameraRotate : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField] [Range(1f, 10f)] float sensitivity = 6f;

    [Header("Options")]
    [SerializeField] float lerpSpeed = 13f;

    Vector2 cameraInput;

    Vector3 cameraRot;

    Quaternion targetRotation;

    void Update()
    {
        GetInput();
    }

    void LateUpdate()
    {
        Rotate();
    }

    void GetInput()
    {
        cameraInput.x = Input.GetAxis("Mouse Y");
        cameraInput.y = Input.GetAxis("Mouse X");
    }

    void Rotate()
    {
        RotateX();
        RotateY();

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
    }

    void RotateY()
    {
        cameraRot.y += cameraInput.y * sensitivity;
    }

    void RotateX()
    {
        cameraRot.x = Mathf.Clamp(cameraRot.x + cameraInput.x * sensitivity, -90, 90);
    }
}
