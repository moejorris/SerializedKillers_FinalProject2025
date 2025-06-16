using UnityEngine;

//Joe Morris
public class Player_Animation : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] Transform playerMeshTransform;
    [SerializeField] Animator playerMeshAnimator;

    [Header("Player Movement References")]
    [SerializeField] Player_MovementMachine machine;
    [SerializeField] Player_Walk walk;

    void Awake()
    {
        machine = GetComponent<Player_MovementMachine>();
        walk = GetComponent<Player_Walk>();
    }

    void LateUpdate()
    {
        playerMeshAnimator.SetFloat("NormalizedWalkSpeed", walk.GetNormalizedSpeed());


        if (machine.ForwardDirection != Vector3.zero)
            playerMeshTransform.forward = machine.ForwardDirection;
    }

    public void PlayDashAnimation()
    {
        // playerMeshAnimator.SetTrigger("Dash");
        playerMeshAnimator.Play("Dash");
    }

    public void PlayJumpAnimation()
    {
        playerMeshAnimator.Play("Jump");
    }

    public void PlayAirJumpAnimation()
    {
        playerMeshAnimator.Play("Air Jump");
    }

    public void UpdateGroundedStatus(bool _grounded)
    {
        playerMeshAnimator.SetBool("isGrounded", _grounded);
    }
}
