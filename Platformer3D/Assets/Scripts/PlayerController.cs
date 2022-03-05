using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float    MoveSpeed = 1;
    public float    RotationSpeed = 1;
    public float    JumpForce = 1;
    public float    GravityScale = 5;

    public CharacterController Controller;

    private Vector3 MoveDirection;

    private Camera PlayerCamera;

    public GameObject PlayerModel;

    public Animator PlayerAnimator;

    private Knockable KnockableCpnt;


    public static PlayerController Instance;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Just reference main camera
        PlayerCamera = Camera.main;

        KnockableCpnt = GetComponent<Knockable>();

        KnockableCpnt.KnockbackEvent += this.OnKnockback;
    }

    private void OnKnockback()
    {
        MoveDirection.y = KnockableCpnt.KnockbackPower.y;
        Controller.Move(MoveDirection * Time.deltaTime);
    }


    // Update is called once per frame
    void Update()
    {
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

    private void UpdateAnimationController()
    {
        PlayerAnimator.SetFloat("Speed", Mathf.Abs(MoveDirection.x) + Mathf.Abs(MoveDirection.z));
        PlayerAnimator.SetBool("Grounded", Controller.isGrounded);
    }
}
