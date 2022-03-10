using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    public AudioClip PlayerKilledSound;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            AudioManager.Instance.Play2DSound(this.PlayerKilledSound);
        }

        var HP = other.GetComponent<Health>();
        if (HP != null)
        {
            HP.Hurt(new Health.DamageInfo(HP.MaxHP, null, Health.HurtSoundType.Silent));
        }

    }
}
