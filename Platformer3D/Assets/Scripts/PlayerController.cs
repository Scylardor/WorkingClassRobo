using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float    MoveSpeed = 1;
    public float    RotationSpeed = 1;
    public float    JumpForce = 1;
    public float    GravityScale = 5;

    public float EnemyHitBounceForce = 8f;

    public CharacterController Controller;

    private Vector3 MoveDirection;

    private float moveHorizontalForce;

    private Camera PlayerCamera;

    public GameObject PlayerModel;

    public Animator PlayerAnimator;

    [Tooltip("How much time do you have after landing to perform super or hyper jump (in seconds)")]
    public float JumpLandingResetDelay = 1f;
    public float SuperJumpBoost = 1.2f;
    public float HyperJumpBoost = 1.2f;

    public float HyperJumpRequiredHorizontalForce = 30f;

    public float aboutToLandDistance = 5f;

    public LayerMask AboutToLandRayLayer;

    private Knockable KnockableCpnt;

    private InGameInputActions  PlayerActions;
    private InputAction PauseAction;

    public static PlayerController Instance;

    public delegate void PauseEventHandler(bool inPause);

    public event PauseEventHandler OnPauseTriggered;

    private bool Paused = false;

    private bool isJumping = false;

    private float currentJumpForce;

    private float timeSpentJumping;

    private bool isCurrentlyGrounded;


    private enum JumpCapability
    {
        SimpleJump,
        SuperJump,
        HyperJump
    }

    private JumpCapability currentJumpCap = JumpCapability.SimpleJump;

    private IEnumerator jumpStateResetCoroutine = null;

    public delegate void LandedEventHandler();
    public event LandedEventHandler OnLanded;

    void Awake()
    {
        Instance = this;
        this.PlayerActions = new InGameInputActions();
    }


    private void OnEnable()
    {
        this.PauseAction = this.PlayerActions.Player.Pause;
        this.PauseAction.started += this.OnPauseInput;
        this.PauseAction.Enable();
    }
    private void OnDisable()
    {
        this.PauseAction.started -= this.OnPauseInput;
        this.PauseAction.Disable();
    }
    private void OnPauseInput(InputAction.CallbackContext obj)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        this.Paused = !this.Paused;
        OnPauseTriggered(this.Paused);
        Debug.Log("Pause triggered");
    }

    public void Bounce()
    {
        float bounceForce = this.EnemyHitBounceForce * (Input.GetButton("Jump") ? 1.5f : 1f);

        this.MoveDirection.y = bounceForce;
        this.Controller.Move(this.MoveDirection * Time.deltaTime);

    }

    // Start is called before the first frame update
    void Start()
    {
        // Just reference main camera
        PlayerCamera = Camera.main;

        KnockableCpnt = GetComponent<Knockable>();

        KnockableCpnt.KnockbackEvent += this.OnKnockback;

        var HPCpnt = GetComponent<Health>();
        if (HPCpnt != null)
        {
            HPCpnt.HurtEvent += this.OnHurt;
        }

        OnLanded += this.UpdateJumpCapabilityUponLanding;

        currentJumpForce = this.JumpForce;
    }

    private void UpdateJumpCapabilityUponLanding()
    {
        if (this.jumpStateResetCoroutine != null)
            StopCoroutine(this.jumpStateResetCoroutine);

        this.jumpStateResetCoroutine = ResetJumpStateCo();
        StartCoroutine(this.jumpStateResetCoroutine);

        switch (this.currentJumpCap)
        {
            case JumpCapability.SimpleJump:
                currentJumpForce *= this.SuperJumpBoost;
                this.currentJumpCap = JumpCapability.SuperJump;
                break;
            case JumpCapability.SuperJump:
                if (this.moveHorizontalForce >= this.HyperJumpRequiredHorizontalForce)
                {
                    this.currentJumpForce *= this.HyperJumpBoost;
                    this.currentJumpCap = JumpCapability.HyperJump;
                }
                else
                {
                    this.currentJumpForce = this.JumpForce;
                    this.currentJumpCap = JumpCapability.SimpleJump;
                }
                break;
            case JumpCapability.HyperJump:
                this.currentJumpForce = this.JumpForce;
                this.currentJumpCap = JumpCapability.SimpleJump;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator ResetJumpStateCo()
    {
        // If the player doesn't jump again after some time, they lose their ability to super/hyper jump.
        // Just reset them to simple jump.
        yield return new WaitForSeconds(this.JumpLandingResetDelay);
        this.currentJumpCap = JumpCapability.SimpleJump;
        this.currentJumpForce = this.JumpForce;
    }

    private void OnHurt(int newHP)
    {
        if (newHP == 0)
        {
            GameManager.Instance.RespawnPlayer();
        }
    }

    private void OnKnockback()
    {
        MoveDirection.y = KnockableCpnt.KnockbackPower.y;
        Controller.Move(MoveDirection * Time.deltaTime);

        this.currentJumpCap = JumpCapability.SimpleJump;
        this.currentJumpForce = this.JumpForce;

        if (this.jumpStateResetCoroutine != null)
            StopCoroutine(this.jumpStateResetCoroutine);
    }


    // Update is called once per frame
    void Update()
    {
        if (this.Controller.isGrounded && !this.isCurrentlyGrounded)
        {
            this.timeSpentJumping = 0f;
            OnLanded?.Invoke(); // we were in the air and we just landed
            this.isCurrentlyGrounded = true;
        }
        else if (!this.Controller.isGrounded && this.isCurrentlyGrounded)
        {
            this.isCurrentlyGrounded = false;
        }
        else if (!this.Controller.isGrounded)
        {
            // Count the time we've been in the air
            this.timeSpentJumping += Time.deltaTime;
        }

        // Can be useful e.g. when controls are locked during a dialog or star collected moment
        if (this.Controller.enabled == false)
            return;

        bool knockedBack = (KnockableCpnt != null && KnockableCpnt.IsKnockedBack);
        // do not interfere with the knocking movement
        if (!knockedBack)
        {
            ControlPlayer();
        }
        else
        {
            KnockbackPlayer();
        }

        UpdateAnimationController();
    }

    private void KnockbackPlayer()
    {
        float moveY = MoveDirection.y;

        MoveDirection = transform.forward * -KnockableCpnt.KnockbackPower.x;
        MoveDirection.y = moveY;

        MoveDirection.y += Physics.gravity.y * Time.deltaTime * GravityScale;

        Controller.Move(MoveDirection * Time.deltaTime);

        this.moveHorizontalForce = 0f;
    }

    private void ControlPlayer()
    {
        float moveY = MoveDirection.y;

        Vector3 inputVector = new Vector3(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"), 0);
        MoveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal"));

        // It can happen, e.g. when controlling with keyboard, that the move direction is not normalized if going "sideways" (vertical and horizontal are both pressed)
        // in that case, Normalize to avoid going twice as fast when going sideways.
        if (this.MoveDirection.magnitude > 1.2f)
            this.MoveDirection.Normalize();

        MoveDirection *= MoveSpeed;
        MoveDirection.y = moveY;

        if (Controller.isGrounded)
        {
            ManageJumping();
        }
        else
        {
            MoveDirection.y += Physics.gravity.y * Time.deltaTime * GravityScale;
        }

        Controller.Move(MoveDirection * Time.deltaTime);

        this.moveHorizontalForce = Mathf.Abs(this.MoveDirection.x) + Mathf.Abs(this.MoveDirection.z);
        // Only rotate the player if it's moving around
        if (this.moveHorizontalForce > 0)
        {
            transform.rotation = Quaternion.Euler(0f, PlayerCamera.transform.rotation.eulerAngles.y, 0f);

            Quaternion newRotation = Quaternion.LookRotation(new Vector3(MoveDirection.x, 0f, MoveDirection.z));
            PlayerModel.transform.rotation = Quaternion.Slerp(PlayerModel.transform.rotation, newRotation, RotationSpeed * Time.deltaTime);
        }
    }

    private void ManageJumping()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (this.jumpStateResetCoroutine != null)
                StopCoroutine(this.jumpStateResetCoroutine);

            MoveDirection.y = this.currentJumpForce;

            this.isJumping = true;
        }
        else
        {
            this.isJumping = false;
        }
    }

    #region Animation
    private void UpdateAnimationController()
    {
        PlayerAnimator.SetFloat("Speed", Mathf.Min(this.Controller.velocity.magnitude, this.MoveSpeed));
        PlayerAnimator.SetBool("IsGrounded", Controller.isGrounded);
        PlayerAnimator.SetBool("isJumping", this.isJumping);
        PlayerAnimator.SetFloat("TimeSpentJumping", this.timeSpentJumping);

        this.PlayerAnimator.SetFloat("HorizontalSpeed", this.moveHorizontalForce);

        if (!Controller.isGrounded)
        {
            bool aboutToLand = Physics.Raycast(
                transform.position,
                -transform.up,
                aboutToLandDistance,
                AboutToLandRayLayer,
                QueryTriggerInteraction.Ignore);

            PlayerAnimator.SetBool("IsAboutToLand", aboutToLand);

            Debug.DrawRay(transform.position, -transform.up * this.aboutToLandDistance, Color.red);
        }
        else
        {
            PlayerAnimator.SetBool("IsAboutToLand", true);
        }
    }

    #endregion
}
