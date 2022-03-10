using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Start is called before the first frame update

    [Min(0)]
    public int  CurrentHP;

    [Min(0)]
    public int  MaxHP;

    [Min(0f)]
    public float    InvincibilityDuration = 0f;

    [Min(0f)]
    public float InvincibilityFlashingPeriod = 0.1f;

    public GameObject[] InvincibilityFlashingObjects;

    public AudioClip HurtSound;

    public enum HurtSoundType
    {
        Sound2D,

        Sound3D
    };

    public HurtSoundType SoundType = HurtSoundType.Sound2D;


    private bool    IsInvincible = false;
    public delegate void InvincibilityEventHandler(bool isInvincible);

    public event InvincibilityEventHandler InvincibleEvent;


    public delegate void HPChange(int newHP);

    public event HPChange HurtEvent;
    public event HPChange HealEvent;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }



    public void Hurt(int damage = 1)
    {
        if (!IsInvincible)
        {
            StartCoroutine(HurtRoutine(damage));
        }

    }

    public void Heal(int healed = 1)
    {
        CurrentHP = Math.Min(CurrentHP + healed, MaxHP);
        HealEvent?.Invoke(CurrentHP);
    }

    public void ResetHealth()
    {
        CurrentHP = MaxHP;
        IsInvincible = false;
        HealEvent?.Invoke(CurrentHP);
    }


    private IEnumerator HurtRoutine(int damage)
    {
        CurrentHP = Math.Max(CurrentHP - damage, 0);

        HurtEvent?.Invoke(CurrentHP);

        if (this.HurtSound != null)
        {
            this.PlayHurtSound();
        }

        if (InvincibilityDuration != 0f)
        {
            IsInvincible = true;
            yield return FlashingRoutine();
        }

        IsInvincible = false;
    }

    private void PlayHurtSound()
    {
        switch (this.SoundType)
        {
            case HurtSoundType.Sound2D:
                AudioManager.Instance.Play2DSound(this.HurtSound);
                break;
            case HurtSoundType.Sound3D:
                AudioManager.Instance.Play3DSound(this.transform.position, this.HurtSound);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator FlashingRoutine()
    {
        InvincibleEvent?.Invoke(true);

        float invincibilitySoFar = 0f;

        float halfPeriod = InvincibilityFlashingPeriod / 2f;

        while (invincibilitySoFar < InvincibilityDuration)
        {
            foreach (GameObject flashingObj in InvincibilityFlashingObjects)
            {
                flashingObj.SetActive(false);
            }

            invincibilitySoFar += halfPeriod;

            yield return new WaitForSeconds(halfPeriod);

            foreach (GameObject flashingObj in InvincibilityFlashingObjects)
            {
                flashingObj.SetActive(true);
            }

            yield return new WaitForSeconds(halfPeriod);

            invincibilitySoFar += halfPeriod;
        }

        InvincibleEvent?.Invoke(false);
    }

}
