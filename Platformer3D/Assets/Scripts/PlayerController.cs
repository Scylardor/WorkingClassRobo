using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float    MoveSpeed = 1;
    public float    JumpForce = 1;
    public float GravityScale = 5;

    private Vector3 MoveDirection = new Vector3();
    public CharacterController Controller;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float moveY = MoveDirection.y;
        MoveDirection.x = Input.GetAxisRaw("Horizontal");
        MoveDirection.y = 0;
        MoveDirection.z = Input.GetAxisRaw("Vertical");
        MoveDirection *= MoveSpeed;
        MoveDirection.y = moveY;

        if (Input.GetButtonDown("Jump"))
        {
            MoveDirection.y = JumpForce;
        }

        MoveDirection.y += Physics.gravity.y * Time.deltaTime * GravityScale;

        Controller.Move(MoveDirection * Time.deltaTime);
    }
}
