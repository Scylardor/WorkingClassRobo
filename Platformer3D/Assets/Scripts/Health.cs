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
        CurrentHP = Math.Max(CurrentHP - damage, 0);
        if (CurrentHP == 0)
        {
            GameManager.Instance.RespawnPlayer();
        }

        HurtEvent?.Invoke(CurrentHP);
    }

    public void ResetHealth()
    {
        CurrentHP = MaxHP;
    }

}
