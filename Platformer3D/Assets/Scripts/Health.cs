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

    private bool    IsInvincible = false;

    public delegate void OnHurt(int newHP);

    public event OnHurt HurtEvent;

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

    public void ResetHealth()
    {
        CurrentHP = MaxHP;
    }


    private IEnumerator HurtRoutine(int damage)
    {
        CurrentHP = Math.Max(CurrentHP - damage, 0);

        HurtEvent?.Invoke(CurrentHP);

        IsInvincible = true;

        if (InvincibilityDuration != 0f)
        {
            yield return new WaitForSeconds(InvincibilityDuration);
        }

        IsInvincible = false;
    }

}
