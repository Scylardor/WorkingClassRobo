using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Start is called before the first frame update

    [Min(0)]
    public int  CurrentHP = 0;

    [Min(0)]
    public int  MaxHP = 0;

    [Min(0f)]
    public float    InvincibilityDuration = 0f;

    [Min(0f)]
    public float InvincibilityFlashingPeriod = 0.1f;

    public GameObject[] InvincibilityFlashingObjects;

    private bool    IsInvincible = false;


    public delegate void HPChange(int newHP);

    public event HPChange HurtEvent;
    public event HPChange HealEvent;



    void Start()
    {
        CurrentHP = MaxHP;
    }

    // Update is called once per frame
    void Update()
    {
    }



    public void Hurt(int damage = 1)
    {
        if (!IsInvincible)
        {
            StartCoroutine(HurtRoutine(damage));
        }

    }

    public void Heal(int healed = 1)
    {
        CurrentHP = Math.Min(CurrentHP + 1, MaxHP);
        HealEvent?.Invoke(CurrentHP);
    }

    public void ResetHealth()
    {
        CurrentHP = MaxHP;
    }


    private IEnumerator HurtRoutine(int damage)
    {
        CurrentHP = Math.Max(CurrentHP - damage, 0);

        HurtEvent?.Invoke(CurrentHP);

        if (InvincibilityDuration != 0f)
        {
            IsInvincible = true;
            yield return FlashingRoutine();
        }

        IsInvincible = false;
    }


    private IEnumerator FlashingRoutine()
    {
        float invincibilitySoFar = 0f;

        float halfPeriod = InvincibilityFlashingPeriod / 2f;

        while (invincibilitySoFar < InvincibilityDuration)
        {
            foreach (GameObject flashingObj in InvincibilityFlashingObjects)
            {
                flashingObj.SetActive(false);
            }

            invincibilitySoFar += halfPeriod;

            yield return new WaitForSeconds(halfPeriod);

            foreach (GameObject flashingObj in InvincibilityFlashingObjects)
            {
                flashingObj.SetActive(true);
            }

            yield return new WaitForSeconds(halfPeriod);

            invincibilitySoFar += halfPeriod;
        }
    }

}
