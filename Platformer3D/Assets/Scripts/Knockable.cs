using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockable : MonoBehaviour
{

    public bool IsKnockedBack = false;

    public float KnockbackDuration = 0.5f;

    public Vector2 KnockbackPower;

    private float KnockbackCountdown = 0f;

    public delegate void OnKnockback();

    public event OnKnockback KnockbackEvent;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsKnockedBack)
        {
            KnockbackCountdown -= Time.deltaTime;
            if (KnockbackCountdown <= 0)
                IsKnockedBack = false;

        }
    }

    public void Knockback()
    {
        IsKnockedBack = true;
        KnockbackCountdown = KnockbackDuration;
        Debug.Log("Knocked back !");
        KnockbackEvent?.Invoke();
    }
}
