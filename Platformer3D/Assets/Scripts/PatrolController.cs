using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolController : MonoBehaviour
{
    public Transform[] patrolPoints;

    public float DestinationReachedTolerance = .2f;

    public float PatrolPointReachedWaitTime = 2f;

    public float ChaseRangeSquare; // Square distance at which the AI starts chasing the player
    public float AttackRangeSquare; // Square distance at which the AI starts chasing the player

    private int currentPoint = 0;

    public NavMeshAgent agent;

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



    // Start is called before the first frame update
    void Start()
    {
        this.agent.SetDestination(this.patrolPoints[this.currentPoint].position);
        this.currentState = AIState.IsPatrolling;
    }

    // Update is called once per frame
    void Update()
    {
        // If player is in range, go chasing them.
        float sqrDistance = Vector3.SqrMagnitude(PlayerController.Instance.transform.position - transform.position);
        if (this.currentState != AIState.IsAttacking)
        {
            if (sqrDistance <= this.AttackRangeSquare)
            {
                this.StartAttacking();
            }
            else if (sqrDistance <= this.ChaseRangeSquare)
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
                if (this.agent.remainingDistance <= this.DestinationReachedTolerance)
                {
                    this.currentState = AIState.IsIdle;
                }
                break;
            case AIState.IsChasing:
                this.agent.SetDestination(PlayerController.Instance.transform.position);
                break;
            case AIState.IsAttacking:

                if (sqrDistance > this.AttackRangeSquare)
                {
                    this.agent.isStopped = false;
                    this.currentState = AIState.IsChasing;
                    OnEndAttack?.Invoke();
                }
                break; // do nothing for now
            default:
                throw new ArgumentOutOfRangeException();
        }
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
        this.agent.velocity = Vector3.zero;
        this.agent.isStopped = true;
        this.OnStartAttack?.Invoke();

        this.currentState = AIState.IsAttacking;
    }

    private IEnumerator WaitThenGoToNextPoint()
    {
        yield return new WaitForSeconds(this.PatrolPointReachedWaitTime);

        this.currentPoint = (this.currentPoint + 1) % this.patrolPoints.Length;
        this.agent.SetDestination(this.patrolPoints[this.currentPoint].position);

        this.currentState = AIState.IsPatrolling;

        idleCoroutine = null;

        yield return null;
    }
}
