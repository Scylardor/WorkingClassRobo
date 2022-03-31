using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingPatrolController : MonoBehaviour
{
    public float FlyingSpeed = 1f;

    public float SteeringSpeed = 1f;

    public Transform[] patrolPoints;

    public float DestinationReachedTolerance = .2f;

    public float PatrolPointReachedWaitTime = 2f;

    public float ChaseRangeSquare; // Square distance at which the AI starts chasing the player
    public float AttackRangeSquare; // Square distance at which the AI starts chasing the player

    private int currentPoint = 0;

    private IEnumerator idleCoroutine = null;

    public enum AIState
    {
        IsIdle,
        IsPatrolling,
        IsChasing,
        IsAttacking
    };

    public AIState currentState;

    public delegate void AttackEventHandler();

    public event AttackEventHandler OnStartAttack;
    public event AttackEventHandler OnEndAttack;

    private bool isDead = false;

    private Vector3 nextDestination;

    // Start is called before the first frame update
    void Start()
    {
        if (this.patrolPoints.Length != 0)
        {
            this.nextDestination = this.patrolPoints[this.currentPoint].position;
            this.currentState = AIState.IsPatrolling;
        }
        else
        {
            Debug.Log("No patrol points set, Patrol Controller will idle");
        }

        var HP = this.GetComponentInChildren<Health>();
        if (HP)
            HP.HurtEvent += this.OnHurt;
    }

    private void OnHurt(int newhp)
    {
        if (newhp == 0)
            this.isDead = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isDead)
            return;

        // If player is in range, go chasing them.
        float destinationDistance = Vector3.Distance(this.nextDestination, transform.position);
        float playerSqrDistance = Vector3.SqrMagnitude(PlayerController.Instance.transform.position - transform.position);

        if (this.currentState != AIState.IsAttacking)
        {
            if (playerSqrDistance <= this.AttackRangeSquare)
            {
                this.StartAttacking();
            }
            else if (playerSqrDistance <= this.ChaseRangeSquare)
            {
                this.StartChasing();
            }
            else if (this.currentState == AIState.IsChasing)
            {
                this.currentState = AIState.IsIdle;
            }
        }

        switch (this.currentState)
        {
            case AIState.IsIdle:
                if (this.idleCoroutine == null)
                {
                    idleCoroutine = WaitThenGoToNextPoint();
                    StartCoroutine(idleCoroutine);
                }

                break;
            case AIState.IsPatrolling:
                if (destinationDistance <= this.DestinationReachedTolerance)
                {
                    this.currentState = AIState.IsIdle;
                }
                break;
            case AIState.IsChasing:
                this.nextDestination = PlayerController.Instance.transform.position;
                break;
            case AIState.IsAttacking:
                if (playerSqrDistance > this.AttackRangeSquare)
                {
                    this.currentState = AIState.IsChasing;
                    OnEndAttack?.Invoke();
                }
                break; // do nothing for now
            default:
                throw new ArgumentOutOfRangeException();
        }

        // Move towards next destination.
        transform.position = Vector3.MoveTowards(
            transform.position,
            this.nextDestination,
            this.FlyingSpeed * Time.deltaTime);

        // Rotate towards next destination.
        Quaternion targetRotation = Quaternion.identity;

        if (destinationDistance > this.DestinationReachedTolerance)
        {
            // (use gram-schmidt to regenerate a new up vector and hope for the best)
            Vector3 newForward = (this.nextDestination - transform.position).normalized;
            Vector3 newRight = Vector3.Cross(Vector3.up, newForward);
            Vector3 newUp = Vector3.Cross(newForward, newRight);

            if (newForward != Vector3.zero && newUp != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(newForward, newUp);
            }
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, this.SteeringSpeed * Time.deltaTime);
    }

    private void StartChasing()
    {
        if (this.idleCoroutine != null)
        {
            this.StopCoroutine(this.idleCoroutine);
            this.idleCoroutine = null;
        }

        this.currentState = AIState.IsChasing;
    }

    private void StartAttacking()
    {
        this.OnStartAttack?.Invoke();

        this.currentState = AIState.IsAttacking;
    }

    private IEnumerator WaitThenGoToNextPoint()
    {
        yield return new WaitForSeconds(this.PatrolPointReachedWaitTime);

        if (this.patrolPoints.Length != 0)
        {
            this.currentPoint = (this.currentPoint + 1) % this.patrolPoints.Length;
            this.nextDestination = this.patrolPoints[this.currentPoint].position;

            this.currentState = AIState.IsPatrolling;
        }

        idleCoroutine = null;

        yield return null;
    }
}
