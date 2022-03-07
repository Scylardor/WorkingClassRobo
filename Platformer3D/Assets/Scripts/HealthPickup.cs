using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : BasePickup
{
    public int HealAmount = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        var hp = other.GetComponent<Health>();
        hp.Heal(HealAmount);
    }
}
