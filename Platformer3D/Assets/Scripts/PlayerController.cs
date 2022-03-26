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
    public float    GravityScale = 5;

    [Tooltip("Cooldown in seconds before you can punch again")]
    public float PunchAttackCooldown = 1f;

    public PlayerPunchHurtbox PunchHurtbox;

    public float EnemyHitBounceForce = 8f;

    public CharacterController Controller;


    public GameObject PlayerModel;

    public Animator PlayerAnimator;

    [System.Serializable]
    public struct JumpConfig
    {
        public float JumpForce;

        [Tooltip("How much time do you have after landing to perform super or hyper jump (in seconds)")]
        public float JumpLandingResetDelay;
        public float SuperJumpHeightBoost;
        public float SuperJumpSpeedBoost;

        public float HyperJumpHeightBoost;
        public float HyperJumpSpeedBoost;

        public float HyperJumpRequiredHorizontalForce;

        public float aboutToLandDistance;

        public LayerMask AboutToLandRayLayer;
    }

    public JumpConfig jumpConfig;

    private Knockable KnockableCpnt;

    private InGameInputActions  PlayerActions;
    private InputAction PauseAction;
    private InputAction JumpAction;
    private InputAction MoveAction;
    private InputAction PunchAction;

    public static PlayerController Instance;

    public delegate void PauseEventHandler(bool inPause);

    public event PauseEventHandler OnPauseTriggered;

    private bool isPaused = false;

    private Vector3 MoveDirection;
    private float moveHorizontalForce;
    private float currentMoveSpeed;

    private float currentJumpForce;
    private float timeSpentJumping;
    private bool isCurrentlyGrounded;

    private bool canPunch = true;

    private Camera PlayerCamera;


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

        this.PauseAction = this.PlayerActions.Player.Pause;
        this.PauseAction.started += this.OnPauseInput;

        this.JumpAction = this.PlayerActions.Player.Jump;
        this.JumpAction.started += this.OnJumpInput;

        this.MoveAction = this.PlayerActions.Player.Move;

        this.PunchAction = this.PlayerActions.Player.Punch;
        this.PunchAction.started += this.OnPunchInput;
    }

    private void OnPunchInput(InputAction.CallbackContext obj)
    {
        if (!this.canPunch || !this.Controller.isGrounded)
            return;

        PlayerAnimator.SetTrigger("IsPunching");

        if (this.PunchHurtbox != null)
        {
            this.PunchHurtbox.gameObject.SetActive(false);
        }

        StartCoroutine(CooldownPunchCo());
    }

    private IEnumerator CooldownPunchCo()
    {
        this.canPunch = false;
        yield return new WaitForSeconds(this.PunchAttackCooldown);
        this.canPunch = true;
    }

    private void OnEnable()
    {
        this.PauseAction.Enable();
        this.JumpAction.Enable();
        this.MoveAction.Enable();
        this.PunchAction.Enable();

    }


    private void OnDisable()
    {
        this.PauseAction.Disable();
        this.JumpAction.Disable();
        this.MoveAction.Disable();
        this.PunchAction.Disable();
    }

    private void OnJumpInput(InputAction.CallbackContext obj)
    {
        if (!this.Controller.isGrounded)
            return;

        if (this.jumpStateResetCoroutine != null)
            StopCoroutine(this.jumpStateResetCoroutine);

        MoveDirection.y = this.currentJumpForce;

        if (this.currentJumpCap == JumpCapability.SuperJump)
        {
            this.currentMoveSpeed = this.MoveSpeed * this.jumpConfig.SuperJumpSpeedBoost;
        }
        else if (this.currentJumpCap == JumpCapability.HyperJump)
        {
            this.currentMoveSpeed = this.MoveSpeed * this.jumpConfig.HyperJumpSpeedBoost;
        }
    }

    private void OnPauseInput(InputAction.CallbackContext obj)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        this.isPaused = !this.isPaused;
        OnPauseTriggered(this.isPaused);
        Debug.Log("Pause triggered");
    }


    private void AnimEventOnPunchStart()
    {
        if (this.PunchHurtbox)
            this.PunchHurtbox.gameObject.SetActive(true);
    }

    private void AnimEventOnPunchFinish()
    {
        if (this.PunchHurtbox)
            this.PunchHurtbox.gameObject.SetActive(false);
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

        currentJumpForce = this.jumpConfig.JumpForce;

        currentMoveSpeed = this.MoveSpeed;

        GameManager.Instance.OnPlayerRespawning += this.OnRespawn;

    }

    private void OnRespawn(Vector3 respawnPosition)
    {
        this.PlayerAnimator.SetBool("IsDead", false);
        this.KnockableCpnt.IsKnockedBack = false;
        this.Controller.enabled = true;
        this.transform.position = respawnPosition;
    }

    private void UpdateJumpCapabilityUponLanding()
    {
        this.currentMoveSpeed = this.MoveSpeed;

        if (this.jumpStateResetCoroutine != null)
            StopCoroutine(this.jumpStateResetCoroutine);

        this.jumpStateResetCoroutine = ResetJumpStateCo();
        StartCoroutine(this.jumpStateResetCoroutine);

        switch (this.currentJumpCap)
        {
            case JumpCapability.SimpleJump:
                currentJumpForce *= this.jumpConfig.SuperJumpHeightBoost;
                this.currentJumpCap = JumpCapability.SuperJump;
                break;
            case JumpCapability.SuperJump:
                if (this.moveHorizontalForce >= this.jumpConfig.HyperJumpRequiredHorizontalForce)
                {
                    this.currentJumpForce *= this.jumpConfig.HyperJumpHeightBoost;
                    this.currentJumpCap = JumpCapability.HyperJump;
                }
                else
                {
                    this.currentJumpForce = this.jumpConfig.JumpForce;
                    this.currentJumpCap = JumpCapability.SimpleJump;
                }
                break;
            case JumpCapability.HyperJump:
                this.currentJumpForce = this.jumpConfig.JumpForce;
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
        yield return new WaitForSeconds(this.jumpConfig.JumpLandingResetDelay);
        this.currentJumpCap = JumpCapability.SimpleJump;
        this.currentJumpForce = this.jumpConfig.JumpForce;
    }

    private void OnHurt(int newHP)
    {
        if (newHP == 0)
        {
            this.PlayerAnimator.SetBool("IsDead", true);
            this.Controller.enabled = false;
            GameManager.Instance.RespawnPlayer();
        }
    }

    private void OnKnockback()
    {
        MoveDirection.y = KnockableCpnt.KnockbackPower.y;
        Controller.Move(MoveDirection * Time.deltaTime);

        this.currentJumpCap = JumpCapability.SimpleJump;
        this.currentJumpForce = this.jumpConfig.JumpForce;

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

        Vector2 moveVector = this.MoveAction.ReadValue<Vector2>();
        MoveDirection = (transform.forward * moveVector.y) + (transform.right * moveVector.x);

        // It can happen, e.g. when controlling with keyboard, that the move direction is not normalized if going "sideways" (vertical and horizontal are both pressed)
        // in that case, Normalize to avoid going twice as fast when going sideways.
        if (this.MoveDirection.magnitude > 1.2f)
            this.MoveDirection.Normalize();

        MoveDirection *= this.currentMoveSpeed;
        MoveDirection.y = moveY;

        if (!Controller.isGrounded)
        {
            MoveDirection.y += Physics.gravity.y * Time.deltaTime * GravityScale;
        }

        if (this.MoveDirection != Vector3.zero)
        {
            Controller.Move(MoveDirection * Time.deltaTime);
        }

        this.moveHorizontalForce = Mathf.Abs(this.MoveDirection.x) + Mathf.Abs(this.MoveDirection.z);
        // Only rotate the player if it's moving around
        if (this.moveHorizontalForce > 0)
        {
            transform.rotation = Quaternion.Euler(0f, PlayerCamera.transform.rotation.eulerAngles.y, 0f);

            Quaternion newRotation = Quaternion.LookRotation(new Vector3(MoveDirection.x, 0f, MoveDirection.z));
            PlayerModel.transform.rotation = Quaternion.Slerp(PlayerModel.transform.rotation, newRotation, RotationSpeed * Time.deltaTime);
        }
    }

    #region Animation
    private void UpdateAnimationController()
    {
        PlayerAnimator.SetFloat("Speed", Mathf.Min(this.Controller.velocity.magnitude, this.MoveSpeed));
        PlayerAnimator.SetBool("IsGrounded", Controller.isGrounded);
        PlayerAnimator.SetFloat("TimeSpentJumping", this.timeSpentJumping);

        this.PlayerAnimator.SetFloat("HorizontalSpeed", this.moveHorizontalForce);

        if (!Controller.isGrounded)
        {
            bool aboutToLand = Physics.Raycast(
                transform.position,
                -transform.up,
                this.jumpConfig.aboutToLandDistance,
                this.jumpConfig.AboutToLandRayLayer,
                QueryTriggerInteraction.Ignore);

            PlayerAnimator.SetBool("IsAboutToLand", aboutToLand);

            Debug.DrawRay(transform.position, -transform.up * this.jumpConfig.aboutToLandDistance, Color.red);
        }
        else
        {
            PlayerAnimator.SetBool("IsAboutToLand", true);
        }
    }

    #endregion
}
