using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpHurtbox : DamageDealer
{
    public PlayerController BounceController;

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

        BounceController?.Bounce();

    }
}
