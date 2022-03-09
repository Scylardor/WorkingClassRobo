using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : MonoBehaviour
{
    public NavMeshAgent Agent;
    public Animator AnimController;

    // With a velocity square magnitude equal or superior to this, we are considered as moving.
    public float MovingSquareVelocityTolerance = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.AnimController.SetBool("IsMoving", this.Agent.velocity.sqrMagnitude >= this.MovingSquareVelocityTolerance);
    }
}
