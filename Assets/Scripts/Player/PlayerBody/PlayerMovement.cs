using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 persistentVelocity;
    [SerializeField] Vector3 walkVelocity;
    Vector3 forwardDirection;
    [SerializeField] bool isGrounded;
    CharacterController charCon;

    [Header("Walking")]
    [SerializeField] float walkSpeed = 7f;
    [SerializeField] float walkAcceleration = 10f;
    [SerializeField] float walkDecceleration = 30f;
    [SerializeField] AnimationCurve walkSpeedUpCurve;
    [SerializeField] AnimationCurve walkRotationBySpeedCurve;


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
    [SerializeField] InputActionReference jumpInput;
    [SerializeField] InputActionReference attackInput;

    [SerializeField] Transform playerMesh;
    Animator animator;


    //Ground Check
    RaycastHit groundInfo;

    enum PlayerState
    {
        Grounded,
        Jumping,
        Falling,
        Override
    }

    PlayerState currentState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        charCon = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        forwardDirection = Vector3.forward;
    }

    void GroundCheck()
    {
        //Raycast Method: Physics.Raycast(transform.position, Vector3.down, out groundInfo, 0.05f + charCon.height / 2f, ~0, QueryTriggerInteraction.Ignore)
        //Spherecast Method: Physics.SphereCast(transform.position, 0.15f, Vector3.down, out groundInfo, charCon.height * 0.5f, ~0, QueryTriggerInteraction.Ignore)
        if (Physics.Raycast(transform.position, Vector3.down, out groundInfo, 0.15f + charCon.height / 2f, ~0, QueryTriggerInteraction.Ignore) && currentState != PlayerState.Jumping)
        {
            if (!isGrounded)
            {
                persistentVelocity.y = 0;
                currentState = PlayerState.Grounded;
            }

            isGrounded = true;
        }
        else
        {
            if (isGrounded)
            {
                if (currentState != PlayerState.Jumping)
                {
                    persistentVelocity.y = gravityOnGround;                    
                }

                currentState = PlayerState.Falling;
            }

            isGrounded = false;

        }
    }

    void OnDrawGizmos()
    {
        if (charCon)
        {
            //draw motion vector
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(groundInfo.point != Vector3.zero ? groundInfo.point : transform.position + Vector3.down * 0.5f * charCon.height, 0.15f);
            Gizmos.DrawRay(transform.position, persistentVelocity);
        }
    }


    // Update is called once per frame
    void Update()
    {




        // if (moveDir.magnitude > 1) moveDir.Normalize();


        // Vector3 walkVel = Vector3.ProjectOnPlane(moveDir * walkSpeed, groundInfo.normal);

        // // velocity = new Vector3(walkVel.x, isGrounded ? walkVel.y : velocity.y, walkVel.z);

        // if (isGrounded)
        // {
        //     if (jumpInput.action.WasPressedThisFrame())
        //     {
        //         velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityAcceleration * jumpingGravityScale);
        //         currentState = PlayerState.Jumping;
        //     }

        //     if (velocity.magnitude < 0.01) velocity = Vector3.zero;

        //     // velocity -= velocity.normalized * Time.deltaTime * velocity.magnitude * 5; //random math to slow down/emulate friction


        // }
        // else
        // {
        //     velocity.y += gravityAcceleration * gravityScale * Time.deltaTime;

        //     Vector3 moveVel = velocity;
        //     moveVel.y = 0;
        //     velocity -= moveVel.normalized * Time.deltaTime * moveVel.magnitude * 4.5f;
        // }

        // Vector3 currentWalkSpeed = velocity - (Vector3.up * velocity.y); //x and z velocity

        // float currentMagnitude = currentWalkSpeed.magnitude;

        // float lerpSpeed = currentWalkSpeed.magnitude <= walkSpeed * moveDir.magnitude ? walkAcceleration : walkDecceleration;
        // currentWalkSpeed = Vector3.Slerp(velocity.normalized, moveDir, walkRotationBySpeedCurve.Evaluate(currentMagnitude / walkSpeed) * Time.deltaTime * lerpSpeed);
        // currentWalkSpeed = Mathf.Lerp(currentMagnitude, walkSpeed * moveDir.magnitude, walkRotationBySpeedCurve.Evaluate(currentMagnitude / walkSpeed) * Time.deltaTime * lerpSpeed) * currentWalkSpeed.normalized;

        // if (currentState != PlayerState.Override)
        // {
        //     velocity = currentWalkSpeed + (Vector3.up * velocity.y);
        // }


        // charCon.Move(velocity * Time.deltaTime);

        // if (GetWalkSpeed() > 0.1f)
        // {
        //     Vector3 lookDir = velocity.normalized;
        //     lookDir.y = 0;

        //     if (playerMesh) playerMesh.forward = lookDir;
        //     else transform.forward = lookDir;
        // }

        GroundCheck();


        if (isGrounded)
        {
            if (jumpInput.action.WasPressedThisFrame())
            {
                persistentVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityAcceleration * jumpingGravityScale);
                currentState = PlayerState.Jumping;
            }

        }

        animator?.SetFloat("NormalizedWalkSpeed", walkVelocity.magnitude / walkSpeed);

        if (attackInput && attackInput.action.WasPressedThisFrame())
        {
            animator?.SetTrigger("Attack");
            OverrideVeloctiy(forwardDirection * 4f);
            CancelInvoke("EnableMovement");
            Invoke("EnableMovement", 0.3f);
        }
    }

    void FixedUpdate()
    {

        if(!isGrounded)
        {
            persistentVelocity.y += gravityAcceleration * gravityScale * Time.fixedDeltaTime;
        }
        if (persistentVelocity.y < -15f) persistentVelocity.y = -15f; //cap fall speed

        if (persistentVelocity.magnitude < 0.01) persistentVelocity = Vector3.zero;
        // if (walkVelocity.magnitude < 0.01) walkVelocity = Vector3.zero;

        Walk();

        velocity = walkVelocity + persistentVelocity;

        charCon.Move(velocity * Time.fixedDeltaTime);
    }




    void Walk()
    {
        Vector3 moveDir = IntendedMoveDirection();
        Vector3 currentWalkVector = walkVelocity;
        float currentWalkSpeed = currentWalkVector.magnitude;

        float lerpSpeed = currentWalkSpeed <= walkSpeed * moveDir.magnitude ? walkAcceleration : walkDecceleration;

        float newWalkSpeed = currentWalkVector.magnitude;

        if (currentState != PlayerState.Override)
        {
            //Speed Change
            float walkLerpValue = Mathf.Clamp(walkSpeedUpCurve.Evaluate(currentWalkSpeed / walkSpeed), 0.1f, 1f);
            newWalkSpeed = Vector3.Slerp(currentWalkVector, moveDir * walkSpeed, walkLerpValue * Time.fixedDeltaTime * lerpSpeed).magnitude;
            if (newWalkSpeed < 0.01f) newWalkSpeed = 0;
        }

        //Walk Rotation
        Vector3 newDir = forwardDirection;
        Vector3 desiredDir = moveDir;
        float rotLerpValue = Mathf.Clamp(walkRotationBySpeedCurve.Evaluate(newWalkSpeed / walkSpeed), 0, 0.8f);

        newDir = Vector3.Slerp(newDir, desiredDir, rotLerpValue * Time.fixedDeltaTime * walkAcceleration);

        if(newDir.magnitude > 0.1f)
            forwardDirection = newDir;

        if (playerMesh)
            playerMesh.forward = forwardDirection;


        //Velocity Vector Change
        walkVelocity = Vector3.ProjectOnPlane(forwardDirection, groundInfo.normal).normalized * newWalkSpeed;
    }

    Vector3 IntendedMoveDirection()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);

        Vector2 moveInput = movementInput.action.ReadValue<Vector2>();
        return moveInput.x * camRight.normalized + moveInput.y * camForward.normalized;
    }

    float GetWalkSpeed()
    {
        return Vector3.ProjectOnPlane(velocity, groundInfo.normal).magnitude;
    }

    public void OverrideVeloctiy(Vector3 newVelocity)
    {
        walkVelocity = newVelocity;
        currentState = PlayerState.Override;
    }

    public void DisableMovement()
    {
        currentState = PlayerState.Override;
    }

    public void EnableMovement()
    {
        currentState = isGrounded ? PlayerState.Grounded : PlayerState.Falling;
    }
}
