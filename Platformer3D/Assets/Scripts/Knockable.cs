using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockable : MonoBehaviour
{

    public bool IsKnockedBack = false;

    public float KnockbackDuration = 0.5f;

    public Vector2 KnockbackPower;

    public delegate void OnKnockback();

    public event OnKnockback KnockbackEvent;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Knockback()
    {
        StartCoroutine(KnockbackRoutine());
    }

    private IEnumerator KnockbackRoutine()
    {
        IsKnockedBack = true;

        Debug.Log("Knocked back !");
        KnockbackEvent?.Invoke();

        yield return new WaitForSeconds(KnockbackDuration);

        IsKnockedBack = false;
    }
}
