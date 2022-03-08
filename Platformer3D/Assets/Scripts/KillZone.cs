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
        AudioClip oldPlayerDeathSound = null;
        if (other.tag == "Player")
        {
            oldPlayerDeathSound = GameManager.Instance.PlayerDeathSound;
            GameManager.Instance.PlayerDeathSound = PlayerKilledSound;
        }

        var HP = other.GetComponent<Health>();
        if (HP != null)
        {
            HP.Hurt(HP.MaxHP);
        }

        // Put back the original death sound
        if (oldPlayerDeathSound != null)
        {
            GameManager.Instance.PlayerDeathSound = oldPlayerDeathSound;
        }
    }
}
