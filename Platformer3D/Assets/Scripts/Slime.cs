using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Slime : MonoBehaviour
{
    // How much do we scale down upon hurt event.
    public float ScaleDownFactor = 0.1f;

    [Tooltip("In seconds")]
    public float ScaleDownSpeed = 1f;

    private Vector3 targetScale;

    private DamageDealer damager;

    private NavMeshAgent agent;

    private float agentSpeedIncrease;

    // Start is called before the first frame update
    void Start()
    {
        var HP = this.GetComponentInChildren<Health>();
        HP.HurtEvent += this.OnHurt;

        this.targetScale = transform.localScale;

        this.damager = this.GetComponentInChildren<DamageDealer>();
        this.agent = this.GetComponentInChildren<NavMeshAgent>();
        this.agentSpeedIncrease = 1f + this.ScaleDownFactor;
    }

    private void OnHurt(int newhp)
    {
        if (newhp != 0)
        {
            this.targetScale -= Vector3.one * this.ScaleDownFactor;

            if (this.damager)
                this.damager.DamageInfo.Damage = Math.Max(this.damager.DamageInfo.Damage - 1, 1);

            if (this.agent)
                this.agent.speed *= this.agentSpeedIncrease;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale != this.targetScale)
            transform.localScale = Vector3.MoveTowards(transform.localScale, this.targetScale, Time.deltaTime * this.ScaleDownSpeed);
    }
}
