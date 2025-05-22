using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Vector3 velocity;
    [SerializeField] bool isGrounded;
    CharacterController charCon;
    [SerializeField] float speed = 7f;

    [Header("Jumping")]
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float jumpingGravityScale = 1f;
    [SerializeField] float fallingGravityScale = 3f;

    [Header("Gravity")]
    [SerializeField] float gravityScale = 1;
    [SerializeField] float gravityAcceleration = -10f;
    [SerializeField] float maxFallSpeed = -30f;
    [SerializeField] float gravityOnGround = -2f;

    [SerializeField] InputActionReference movementInput;

    //Ground Check
    RaycastHit groundInfo;

    enum PlayerState
    {
        Grounded,
        Jumping,
        Falling
    }

    PlayerState currentState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        charCon = GetComponent<CharacterController>();
    }

    void GroundCheck()
    {
        //Raycast Method: Physics.Raycast(transform.position, Vector3.down, out groundInfo, 0.05f + charCon.height / 2f, ~0, QueryTriggerInteraction.Ignore)
        //Spherecast Method: Physics.SphereCast(transform.position, 0.15f, Vector3.down, out groundInfo, charCon.height * 0.5f, ~0, QueryTriggerInteraction.Ignore)
        if(Physics.Raycast(transform.position, Vector3.down, out groundInfo, 0.15f + charCon.height / 2f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (!isGrounded) velocity.y = 0;

            currentState = PlayerState.Grounded;
            isGrounded = true;
        }
        else
        {
            if (isGrounded) velocity.y = gravityOnGround;

            isGrounded = false;

            if (velocity.y < 1) currentState = PlayerState.Falling;
        }
    }

    void OnDrawGizmos()
    {
        if (charCon)
        {
            Gizmos.DrawSphere(groundInfo.point != Vector3.zero ? groundInfo.point : transform.position + Vector3.down * 0.5f * charCon.height, 0.15f);
            Gizmos.DrawRay(transform.position, velocity);
        }
    }


    // Update is called once per frame
    void Update()
    {
        GroundCheck();

        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);

        Vector2 moveInput = movementInput.action.ReadValue<Vector2>();

        Vector3 moveDir = moveInput.x * camRight.normalized + moveInput.y * camForward.normalized;

        if (moveDir.magnitude > 1) moveDir.Normalize();

        Vector3 walkVel = Vector3.ProjectOnPlane(moveDir * speed, groundInfo.normal);

        velocity = new Vector3(walkVel.x, isGrounded ? walkVel.y : velocity.y, walkVel.z);

        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityAcceleration * jumpingGravityScale);
                currentState = PlayerState.Jumping;
            }
        }
        else
        {
            velocity.y += gravityAcceleration * gravityScale * Time.deltaTime;
        }

        charCon.Move(velocity * Time.deltaTime);
    }
}
