using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField]
    public Health.DamageInfo DamageInfo;

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
        hp?.Hurt(this.DamageInfo);
    }
}
