using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

public class Boss : MonoBehaviour
{
    public Animator bossAnimator;

    public float thinkingDelayMin = 1f;

    public float thinkingDelayMax = 2f;

    public float LookatSpeed = 1f;

    public float ApproachSpeed = 1f;

    public float IdealTargetDistance = 1f;
    public float IdealTargetHeightDifference = 20f;

    public GameObject leftHand;

    public GameObject rightHand;

    private enum State
    {
        Idle,
        WantToSweep,
        WantToStomp
    }

    private State currentState = State.Idle;

    private bool leftHandDown, rightHandDown;

    public Transform target;

    private IEnumerator thinkCoroutine;

    private enum AttackingHand
    {
        None,

        Left,

        Right
    };

    private AttackingHand handAttacking = AttackingHand.None;
    private bool isCurrentlyHurt = false;

    // Start is called before the first frame update
    void Start()
    {
        if (this.target == null)
            this.target = PlayerController.Instance.transform;

        this.leftHandDown = this.rightHandDown = false;

        if (this.leftHand)
        {
            var hp = this.leftHand.GetComponentInChildren<Health>();
            if (hp)
                hp.HurtEvent += this.OnLeftHandHurt;
        }

        if (this.rightHand)
        {
            var hp = this.rightHand.GetComponentInChildren<Health>();
            if (hp)
                hp.HurtEvent += this.OnRightHandHurt;
        }

        ToggleAttackMode(AttackingHand.Left, false);
        ToggleAttackMode(AttackingHand.Right, false);

        thinkCoroutine = this.ThinkCo();
        StartCoroutine(this.thinkCoroutine);

    }

    private bool IsDefeated()
    {
        return this.leftHandDown && this.rightHandDown;
    }

    private void OnRightHandHurt(int newhp)
    {
        this.DeactivateAttackMode();

        if (newhp == 0)
        {
            this.rightHandDown = true;
            if (this.leftHandDown)
            {
                this.Defeat();
            }
        }
        else
        {
            this.bossAnimator.SetTrigger("HurtRightHand");
            isCurrentlyHurt = true;
        }
    }

    private void OnLeftHandHurt(int newhp)
    {
        this.DeactivateAttackMode();

        if (newhp == 0)
        {
            this.leftHandDown = true;
            if (this.rightHandDown)
            {
                Defeat();
            }
        }
        else
        {
            this.bossAnimator.SetTrigger("HurtLeftHand");
            isCurrentlyHurt = true;

        }
    }

    private void OnDefeatAnimationDone()
    {
        this.bossAnimator.enabled = false;
        // Let everything fall to the ground.
        Rigidbody[] bodies = this.GetComponentsInChildren<Rigidbody>();
        foreach (var body in bodies)
        {
            body.gameObject.transform.parent = null;
            body.isKinematic = false;
            body.useGravity = true;

        }
    }
    private void Defeat()
    {
        this.handAttacking = AttackingHand.None;
        this.leftHandDown = this.rightHandDown = true;

        this.bossAnimator.SetTrigger("IsDefeated");
        StopCoroutine(this.thinkCoroutine);
    }

    private void OnHurtAnimationDone()
    {
        isCurrentlyHurt = false;
    }


    private IEnumerator ThinkCo()
    {
        while (!this.IsDefeated())
        {
            while (this.isCurrentlyHurt || this.handAttacking != AttackingHand.None)
                yield return null; // try again next frame

            if (this.IsDefeated())
                break;

            float thinkingDelay = Random.Range(this.thinkingDelayMin, this.thinkingDelayMax);
            yield return new WaitForSeconds(thinkingDelay);

            // pick a new attack
            this.currentState = this.PickNextAttack();

            switch (this.currentState)
            {
                case State.WantToStomp:
                    ActivateAttackMode(AttackingHand.Right, "DoStomp");
                    break;

                case State.WantToSweep:
                    ActivateAttackMode(AttackingHand.Left, "DoSweep");
                    break;
            }
        }
    }

    private void ActivateAttackMode(AttackingHand attackHand, string attackTrigger)
    {
        ToggleAttackMode(attackHand, true);

        this.bossAnimator.SetTrigger(attackTrigger);

        this.handAttacking = attackHand;
    }

    private void DeactivateAttackMode()
    {
        ToggleAttackMode(this.handAttacking, false);
        this.handAttacking = AttackingHand.None;
    }

    private void ToggleAttackMode(AttackingHand attackHand, bool attackEnabled)
    {
        if (attackHand == AttackingHand.Left && this.leftHand)
        {
            ToggleHandAttackMode(this.leftHand, attackEnabled);
        }
        else if (attackHand == AttackingHand.Right && this.rightHand)
        {
            ToggleHandAttackMode(this.rightHand, attackEnabled);
        }
    }


    private void ToggleHandAttackMode(GameObject hand, bool attackEnabled)
    {
        var damageDealer = hand.GetComponent<DamageDealer>();
        if (damageDealer)
        {
            damageDealer.enabled = attackEnabled;
        }
        var HP = hand.GetComponentInChildren<Health>();
        if (HP)
            HP.SetInvincible(attackEnabled);

        // (de)activate vulnerability collider
        SphereCollider vulnCollider = hand.GetComponentInChildren<SphereCollider>();
        if (vulnCollider)
            vulnCollider.enabled = !attackEnabled;

        // disable or enable knock
        Knock handK = hand.GetComponentInChildren<Knock>();
        if (handK)
            handK.enabled = attackEnabled;

        // disable or enable the hurtbox
        BoxCollider hurtbox = hand.GetComponentInChildren<BoxCollider>();
        if (hurtbox)
            hurtbox.enabled = attackEnabled;
    }

    // Update is called once per frame
    void Update()
    {

    }




    void LateUpdate()
    {
        if (this.IsDefeated())
            return;

        // Adjust position (keep at an ideal distance)
        Vector3 vecToTarget = this.target.position - transform.position;
        Vector3 targetDir = vecToTarget.normalized;
        Vector3 fromTargetToMe = -targetDir;
        Vector3 idealLocation = this.target.position + fromTargetToMe * this.IdealTargetDistance;
        idealLocation.y = target.position.y + this.IdealTargetHeightDifference;

        Debug.DrawLine(transform.position, idealLocation, Color.red, 0f, false);

        float speed = vecToTarget.magnitude > this.IdealTargetDistance ? this.ApproachSpeed * 4 : this.ApproachSpeed;
        Vector3 targetLocation = Vector3.MoveTowards(transform.position, idealLocation, Time.deltaTime * speed);

        transform.position = targetLocation;

        // Adjust orientation (smoothly rotate to face the target point.)
        var targetRotation = Quaternion.LookRotation(vecToTarget);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, LookatSpeed * Time.deltaTime);
    }

    private State PickNextAttack()
    {
        if (this.leftHandDown)
            return State.WantToStomp;

        if (this.rightHandDown)
            return State.WantToSweep;

        // if both hands are available, flip a coin
        int rand = Random.Range(0, 2);
        if (rand == 0)
            return State.WantToStomp;

        return State.WantToSweep;
    }
}
