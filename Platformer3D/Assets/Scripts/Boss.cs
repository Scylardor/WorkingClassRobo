using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Animator bossAnimator;

    public float thinkingDelayMin = 1f;

    public float thinkingDelayMax = 2f;

    public float LookatSpeed = 1f;

    public Health leftHandHP;
    public Health rightHandHP;
    private enum State
    {
        Idle,
        WantToSweep,
        WantToStomp
    }

    private State currentState = State.Idle;

    private bool leftHandDown, rightHandDown;

    // Start is called before the first frame update
    void Start()
    {
        this.leftHandDown = this.rightHandDown = false;

        if (this.leftHandHP)
            this.leftHandHP.HurtEvent += this.OnLeftHandHurt;

        if (this.rightHandHP)
            this.rightHandHP.HurtEvent += this.OnRightHandHurt;

        StartCoroutine(ThinkCo());
    }

    private void OnRightHandHurt(int newhp)
    {
        if (newhp == 0)
        {
            this.rightHandDown = true;
            if (this.leftHandDown)
            {
                this.bossAnimator.SetTrigger("IsDefeated");
            }
        }
        else
        {
            this.bossAnimator.SetTrigger("HurtRightHand");
        }
    }

    private void OnLeftHandHurt(int newhp)
    {
        if (newhp == 0)
        {
            this.leftHandDown = true;
            if (this.rightHandDown)
            {
                this.bossAnimator.SetTrigger("IsDefeated");
            }
        }
        else
        {
            this.bossAnimator.SetTrigger("HurtLeftHand");
        }
    }


    private IEnumerator ThinkCo()
    {
        while (!this.leftHandDown || !this.rightHandDown)
        {
            float thinkingDelay = Random.Range(this.thinkingDelayMin, this.thinkingDelayMax);
            yield return new WaitForSeconds(thinkingDelay);

            // pick a new attack
            this.currentState = this.PickNextAttack();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    void LateUpdate()
    {
        if (this.leftHandDown && this.rightHandDown)
            return;

        var targetRotation = Quaternion.LookRotation(PlayerController.Instance.transform.position - transform.position);

        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, LookatSpeed * Time.deltaTime);
    }

    private State PickNextAttack()
    {
        // flip a coin
        int rand = Random.Range(0, 1);
        if (rand == 0)
            return State.WantToStomp;

        return State.WantToSweep;
    }
}
