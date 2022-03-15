using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpHurtbox : DamageDealer
{
    public PlayerController BoundController;


    // Start is called before the first frame update
    void Start()
    {
        var HP = this.BoundController.GetComponent<Health>();
        if (HP != null)
            HP.InvincibleEvent += this.OnInvincibilityChange;
    }

    private void OnInvincibilityChange(bool isinvincible)
    {
        // Deactiate the hurtbox when the player is invincible due to received damage.
        this.gameObject.SetActive(!isinvincible);
    }

    // Update is called once per frame
    void Update()
    {

    }
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        this.BoundController?.Bounce();
        if (other.GetComponent<Bounceable>() != null)
            this.BoundController?.Bounce();

    }
}
