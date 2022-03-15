using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField]
    public Health.DamageInfo DamageInfo;

    private bool initialHurtFailed = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log(this.gameObject.name + " hurts " + other.name);
        var hp = other.GetComponent<Health>();

          if (hp != null)
        {
            initialHurtFailed = !hp.Hurt(this.DamageInfo);
        }
    }


    protected virtual void OnTriggerStay(Collider other)
    {
        if (this.initialHurtFailed)
        {
            var hp = other.GetComponent<Health>();

            if (hp != null)
            {
                initialHurtFailed = !hp.Hurt(this.DamageInfo);
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        initialHurtFailed = false;
    }
}
