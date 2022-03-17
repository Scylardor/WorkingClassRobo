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

    public AudioClip DefaultHurtSound;

    public enum HurtSoundType
    {
        Sound2D,

        Sound3D,

        Silent
    }

    [System.Serializable]
    public struct DamageInfo
    {
        public int Damage;

        public AudioClip HurtSound;

        public HurtSoundType Type;

        public DamageInfo(int dmg, AudioClip sound = null, HurtSoundType soundType = HurtSoundType.Sound2D)
        {
            this.Damage = dmg;
            this.HurtSound = sound;
            this.Type = soundType;
        }
    }



    public Health.HurtSoundType SoundType = Health.HurtSoundType.Sound2D;


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


    public void SetInvincible(bool enabled)
    {
        this.IsInvincible = enabled;
    }

    public bool Hurt(DamageInfo dmgInfo)
    {
        bool isHurt = !this.IsInvincible;
        if (isHurt)
        {
            StartCoroutine(this.HurtRoutine(dmgInfo));
        }

        return isHurt;
    }

    public void Heal(int healed = 1)
    {
        this.CurrentHP = Math.Min(this.CurrentHP + healed, this.MaxHP);
        this.HealEvent?.Invoke(this.CurrentHP);
    }

    public void ResetHealth()
    {
        this.CurrentHP = this.MaxHP;
        this.IsInvincible = false;
        this.HealEvent?.Invoke(this.CurrentHP);
    }


    private IEnumerator HurtRoutine(DamageInfo dmgInfo)
    {
        this.CurrentHP = Math.Max(this.CurrentHP - dmgInfo.Damage, 0);

        this.HurtEvent?.Invoke(this.CurrentHP);

        if (dmgInfo.Type != HurtSoundType.Silent)
            this.PlayHurtSound(dmgInfo);

        if (this.InvincibilityDuration != 0f)
        {
            this.IsInvincible = true;
            yield return this.FlashingRoutine();
        }

        this.IsInvincible = false;
    }

    private void PlayHurtSound(DamageInfo dmgInfo)
    {
        var playedClip = dmgInfo.HurtSound != null ? dmgInfo.HurtSound : this.DefaultHurtSound;
        if (playedClip == null)
            return;

        switch (dmgInfo.Type)
        {
            case HurtSoundType.Sound2D:
                AudioManager.Instance.Play2DSound(playedClip);
                break;
            case HurtSoundType.Sound3D:
                AudioManager.Instance.Play3DSound(this.transform.position, playedClip);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator FlashingRoutine()
    {
        this.InvincibleEvent?.Invoke(true);

        float invincibilitySoFar = 0f;

        float halfPeriod = this.InvincibilityFlashingPeriod / 2f;

        foreach (GameObject flashingObj in this.InvincibilityFlashingObjects)
        {
            Collider collider = flashingObj.GetComponent<Collider>();
            if (collider)
                collider.enabled = false;
        }

        while (invincibilitySoFar < this.InvincibilityDuration)
        {
            foreach (GameObject flashingObj in this.InvincibilityFlashingObjects)
            {
                Renderer[] renders = flashingObj.GetComponentsInChildren<Renderer>();
                foreach (var rdr in renders)
                {
                    if (rdr)
                        rdr.enabled = false;
                }

                SkinnedMeshRenderer[] skRenders = flashingObj.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var rdr in skRenders)
                {
                    if (rdr)
                        rdr.enabled = false;
                }
            }

            invincibilitySoFar += halfPeriod;

            yield return new WaitForSeconds(halfPeriod);

            foreach (GameObject flashingObj in this.InvincibilityFlashingObjects)
            {
                Renderer[] renders = flashingObj.GetComponentsInChildren<Renderer>();
                foreach (var rdr in renders)
                {
                    if (rdr)
                        rdr.enabled = true;
                }

                SkinnedMeshRenderer[] skRenders = flashingObj.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var rdr in skRenders)
                {
                    if (rdr)
                        rdr.enabled = true;
                }
            }

            yield return new WaitForSeconds(halfPeriod);

            invincibilitySoFar += halfPeriod;
        }


        foreach (GameObject flashingObj in this.InvincibilityFlashingObjects)
        {
            Collider collider = flashingObj.GetComponent<Collider>();
            if (collider)
                collider.enabled = true;
        }

        this.InvincibleEvent?.Invoke(false);
    }
}
