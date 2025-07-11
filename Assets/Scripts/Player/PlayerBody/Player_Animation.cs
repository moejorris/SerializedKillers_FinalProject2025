using UnityEngine;

//Joe Morris
public class Player_Animation : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] Transform playerMeshTransform;
    [SerializeField] Animator playerMeshAnimator;

    float foursTimer = 0;

    void LateUpdate()
    {

        foursTimer += Time.deltaTime;

        if (foursTimer >= 0.1f) // 0.1f in between updates to emulate choppy feeling 
        {
            playerMeshTransform.forward = PlayerController.instance.MovementMachine.ForwardDirection;
            playerMeshAnimator.SetFloat("NormalizedWalkSpeed", PlayerController.instance.Walk.GetNormalizedSpeed() * PlayerController.instance.MovementMachine.MovementMultiplier);
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
