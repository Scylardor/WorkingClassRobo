using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolController : MonoBehaviour
{
    public Transform[] patrolPoints;

    public float DestinationReachedTolerance = .2f;

    private int currentPoint = 0;

    public NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        this.agent.SetDestination(this.patrolPoints[this.currentPoint].position);
    }

    // Update is called once per frame
    void Update()
    {

        if (this.agent.remainingDistance <= this.DestinationReachedTolerance)
        {
            this.currentPoint = (this.currentPoint + 1) % this.patrolPoints.Length;
            this.agent.SetDestination(this.patrolPoints[this.currentPoint].position);
        }
    }
}
