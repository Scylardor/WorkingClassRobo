using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float    MoveSpeed;
    public float    JumpForce;

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

        transform.position += MoveDirection;
    }
}
