using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Dash : MonoBehaviour, IPlayerMover
{
    [SerializeField] InputActionReference dashInput;
    [SerializeField] InputActionReference walkInput;
    [Header("How far the dash should move the player (in Meters/Unity Units)")]
    [SerializeField] float _distance = 4f;
    [Header("How long one dash should take (in Seconds)")]
    public float _travelTime = 0.25f;
    [Header("Whether or not Gravity is applied while dashing")]
    [SerializeField] bool _ignoresGravity = true;

    [Header("Limits")]
    [SerializeField] float cooldownOnGround = 0.25f;
    [SerializeField] int dashesAllowedInAir = 2;
    int airDashesLeft = 0;
    bool canDash = true;


    Vector3 _force;
    bool _isDashing = false;

    void Awake()
    {

    }
    void OnEnable()
    {
        PlayerController.instance.MovementMachine.AddMover(this); //Add itself to the movement machine!
        PlayerController.instance.Gravity.PlayerJustLanded += ResetDashLimits;
    }

    void OnDisable()
    {
        PlayerController.instance.MovementMachine.RemoveMover(this); //remove itself from the movement machine when no longer active!
        PlayerController.instance.Gravity.PlayerJustLanded -= ResetDashLimits;
    }

    void Update()
    {
        if (!_isDashing && dashInput.action.WasPerformedThisFrame())
        {
            if (!PlayerController.instance.MovementMachine.isGrounded && airDashesLeft < 1) return;
            
            StartCoroutine(Dash(transform.position + (DashDirection() * _distance)));
        }
    }

    public Vector3 UpdateForce()
    {
        return _force / PlayerController.instance.MovementMachine.DeltaTime;
        // return Vector3.zero;
    }

    public void ExternalDash(Vector3 destination, bool changeY = false)
    {
        StartCoroutine(Dash(destination, false, 0, changeY, true));
    }

    IEnumerator Dash(Vector3 destination, bool updateOtherMovers = true, float tTime = 0, bool changeY = false, bool ignoreCooldown = false)
    {
        if (tTime == 0) tTime = _travelTime;

        if (!PlayerController.instance.MovementMachine.isGrounded)
        {
            airDashesLeft--;
        }

        _isDashing = true;

        EnableTrail();

        SoundManager.instance.PlaySoundEffect(PlayerController.instance.SoundBank.GetSoundByName("DashSound"));

        Vector3 forwardDir = destination - transform.position;

        forwardDir.y = 0;

        PlayerController.instance.MovementMachine.SetForwardDirection(forwardDir); //Snaps player to the direction they are trying to dash in so they dash in the correct direction

        Vector3 startPosition = transform.position;

        // Debug.DrawLine(startPosition, destination, Color.blue, tTime);

        // destination.y = 0;
        // startPosition.y = 0;

        PlayerController.instance.Animation.PlayDashAnimation();

        if (updateOtherMovers) UpdateOtherMoveComponents();

        float t = 0;

        if (tTime < PlayerController.instance.MovementMachine.DeltaTime)
        {
            PlayerController.instance.CharacterController.Move(destination - startPosition);

            yield return new WaitForSeconds(PlayerController.instance.MovementMachine.DeltaTime);

        }
        else while (t < tTime)
            {

                if (!changeY) destination.y = transform.position.y;
                startPosition.y = transform.position.y;

                _force = Vector3.Slerp(startPosition, destination, t / tTime) - transform.position;

                _force = Vector3.ProjectOnPlane(_force, PlayerController.instance.MovementMachine.GroundInformation.normal);
                // Debug.Log(_force.y);


                yield return new WaitForSeconds(PlayerController.instance.MovementMachine.DeltaTime);

                t = Mathf.Clamp(t + PlayerController.instance.MovementMachine.DeltaTime, 0, tTime);

            }

        _force = Vector3.zero;


        DisableTrail();

        _isDashing = false;
        if (updateOtherMovers) UpdateOtherMoveComponents(); //re-enables walking and turning

        _isDashing = true;
        if (PlayerController.instance.MovementMachine.isGrounded) yield return new WaitForSeconds(cooldownOnGround);

        _isDashing = false;
    }

    void UpdateOtherMoveComponents()
    {
        PlayerController.instance.Rotate.enabled = !_isDashing;
        PlayerController.instance.Gravity.enabled = _ignoresGravity ? !_isDashing : true;

        if (_isDashing)
        {
            PlayerController.instance.MovementMachine.DisableAllMovers(this, PlayerController.instance.Gravity);
        }
        else
        {
            PlayerController.instance.MovementMachine.EnableAllMovers();
        }
    }

    Vector3 DashDirection()
    {
        Vector2 moveInput = walkInput.action.ReadValue<Vector2>();

        if (moveInput.magnitude < 0.1f) return PlayerController.instance.MovementMachine.ForwardDirection; //if player is not inputting a direction, then the current forward direction will be used.

        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);

        return moveInput.x * camRight.normalized + moveInput.y * camForward.normalized;
    }

    void EnableTrail()
    {
        TrailRenderer tr = GetComponent<TrailRenderer>();
        if (!tr) return;

        // tr.time = _travelTime;
        tr.emitting = true;

        tr.colorGradient = ColorGradient(PlayerController.instance.ScriptSteal.scriptEffectColor, Color.white);
    }

    Gradient ColorGradient(Color color1, Color color2)
    {
        var gradient = new Gradient();

        // Blend color from red at 0% to blue at 100%
        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(color1, 0.0f);
        colors[1] = new GradientColorKey(color2, 1.0f);

        // Blend alpha from opaque at 0% to transparent at 100%
        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(0.0f, 1.0f);

        gradient.SetKeys(colors, alphas);

        return gradient;
    }

    void DisableTrail()
    {
        TrailRenderer tr = GetComponent<TrailRenderer>();

        if (!tr) return;

        tr.emitting = false;
    }

    void ResetDashLimits()
    {
        airDashesLeft = dashesAllowedInAir;
    }

}
