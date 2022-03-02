using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float    MoveSpeed = 1;
    public float    JumpForce = 1;

    private Vector3 MoveDirection = new Vector3();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MoveDirection.x = Input.GetAxisRaw("Horizontal");
        MoveDirection.z = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            MoveDirection.y += Time.deltaTime * JumpForce;
        }

        transform.position += (MoveDirection * Time.deltaTime * MoveSpeed);
    }
}
