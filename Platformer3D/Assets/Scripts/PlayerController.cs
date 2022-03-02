using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float    MoveSpeed = 1;
    public float    RotationSpeed = 1;
    public float    JumpForce = 1;
    public float    GravityScale = 5;

    public CharacterController Controller;

    private Camera PlayerCamera;

    private Vector3 MoveDirection = new Vector3();

    public GameObject PlayerModel;


    // Start is called before the first frame update
    void Start()
    {
        PlayerCamera = Camera.main;
    }


    // Update is called once per frame
    void Update()
    {
        float moveY = MoveDirection.y;

        MoveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal"));

        MoveDirection *= MoveSpeed;
        MoveDirection.y = moveY;

        if (Input.GetButtonDown("Jump"))
        {
            MoveDirection.y = JumpForce;
        }

        MoveDirection.y += Physics.gravity.y * Time.deltaTime * GravityScale;

        Controller.Move(MoveDirection * Time.deltaTime);

        // Only rotate the player if it's moving around
        if (MoveDirection.x != 0f || MoveDirection.z != 0f)
        {
            transform.rotation = Quaternion.Euler(0f, PlayerCamera.transform.rotation.eulerAngles.y, 0f);

            Quaternion newRotation = Quaternion.LookRotation(new Vector3(MoveDirection.x, 0f, MoveDirection.z));
            PlayerModel.transform.rotation = Quaternion.Slerp(PlayerModel.transform.rotation, newRotation, RotationSpeed * Time.deltaTime);
        }
    }
}
