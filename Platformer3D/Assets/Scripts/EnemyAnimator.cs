using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : MonoBehaviour
{
    public NavMeshAgent Agent;
    public Animator AnimController;

    // With a velocity square magnitude equal or superior to this, we are considered as moving.
    public float MovingSquareVelocityTolerance = 2f;

    public float AttackCooldown = 1f;

    public bool stopUponAttack = true;

    private bool IsAttackCoolingDown = false;

    private bool IsAttacking = false;

    public PatrolController AIController;

    public GameObject DeathParticles;

    public GameObject DroppedPickup;

    // Start is called before the first frame update
    void Start()
    {
        if (AIController != null)
        {
            AIController.OnStartAttack += this.OnAttack;
            AIController.OnEndAttack += this.OnAttackEnd;
        }

        var HP = this.GetComponent<Health>() ?? this.GetComponentInChildren<Health>();
        if (HP != null)
        {
            HP.HurtEvent += this.OnHurt;
        }
    }

    private void OnHurt(int newhp)
    {
        if (newhp == 0)
        {
            if (this.DeathParticles != null)
            {
                Instantiate(this.DeathParticles, transform.position + new Vector3(0, 1.2f, 0), transform.rotation);
            }

             if (this.DroppedPickup != null)
                Instantiate(this.DroppedPickup, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);

            Destroy(gameObject);
        }
    }

    private void OnAttackEnd()
    {
        this.IsAttacking = false;
    }

    private void OnAttack()
    {
        IsAttacking = true;
    }

    private void AnimEvent_UnStopAfterAttack()
    {
        this.Agent.isStopped = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsAttacking && !this.IsAttackCoolingDown)
        {
            if (this.stopUponAttack)
            {
                this.Agent.velocity = Vector3.zero;
                this.Agent.isStopped = true;
            }

            this.AnimController.SetTrigger("Attack");

            StartCoroutine(this.CooldownAttack());
        }

        this.AnimController.SetBool(
            "IsMoving",
            this.Agent.velocity.sqrMagnitude >= this.MovingSquareVelocityTolerance);
    }

    IEnumerator CooldownAttack()
    {
        this.IsAttackCoolingDown = true;
        yield return new WaitForSeconds(this.AttackCooldown);
        this.IsAttackCoolingDown = false;
    }
}
