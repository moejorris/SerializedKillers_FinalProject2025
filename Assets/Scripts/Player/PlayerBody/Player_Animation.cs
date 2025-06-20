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

    float foursTimer = 0;

    void Awake()
    {
        machine = GetComponent<Player_MovementMachine>();
        walk = GetComponent<Player_Walk>();
    }

    void LateUpdate()
    {

        foursTimer += Time.deltaTime;

        if (foursTimer >= 0.1f) // 0.1f in between updates to emulate choppy feeling 
        {
            playerMeshTransform.forward = machine.ForwardDirection;
            playerMeshAnimator.SetFloat("NormalizedWalkSpeed", walk.GetNormalizedSpeed());
            foursTimer = 0;
        }

        playerMeshTransform.position = transform.position - Vector3.up; //tried making position choppy, but man it looked awful!
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
