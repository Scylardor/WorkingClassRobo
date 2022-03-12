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

    private Camera PlayerCamera;

    public GameObject PlayerModel;

    public Animator PlayerAnimator;

    private Knockable KnockableCpnt;

    private InGameInputActions  PlayerActions;
    private InputAction PauseAction;

    public static PlayerController Instance;

    public delegate void PauseEventHandler(bool inPause);

    public event PauseEventHandler OnPauseTriggered;

    private bool Paused = false;

    private bool IsJumping = false;

    private bool isCurrentlyGrounded;

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
    }


    // Update is called once per frame
    void Update()
    {
        if (this.Controller.isGrounded && !this.isCurrentlyGrounded)
        {
            OnLanded?.Invoke(); // we were in the air and we just landed
            this.isCurrentlyGrounded = true;
        }
        else if (!this.Controller.isGrounded && this.isCurrentlyGrounded)
        {
            this.isCurrentlyGrounded = false;
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
    }

    private void ControlPlayer()
    {
        float moveY = MoveDirection.y;

        MoveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal"));
        MoveDirection.Normalize();
        MoveDirection *= MoveSpeed;
        MoveDirection.y = moveY;

        if (Controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                MoveDirection.y = JumpForce;
                IsJumping = true;
            }
            else
            {
                IsJumping = false;
            }
        }
        else
        {
            MoveDirection.y += Physics.gravity.y * Time.deltaTime * GravityScale;
        }


        Controller.Move(MoveDirection * Time.deltaTime);

        // Only rotate the player if it's moving around
        if (MoveDirection.x != 0f || MoveDirection.z != 0f)
        {
            transform.rotation = Quaternion.Euler(0f, PlayerCamera.transform.rotation.eulerAngles.y, 0f);

            Quaternion newRotation = Quaternion.LookRotation(new Vector3(MoveDirection.x, 0f, MoveDirection.z));
            PlayerModel.transform.rotation = Quaternion.Slerp(PlayerModel.transform.rotation, newRotation, RotationSpeed * Time.deltaTime);
        }
    }

    #region Animation
    private void UpdateAnimationController()
    {
        PlayerAnimator.SetFloat("Speed", Mathf.Abs(MoveDirection.x) + Mathf.Abs(MoveDirection.z));
        PlayerAnimator.SetBool("IsGrounded", Controller.isGrounded);
        PlayerAnimator.SetBool("IsJumping", this.IsJumping);
    }

    #endregion
}
