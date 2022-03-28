using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

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

    public bool KnockOnDamageTaken = false;

    public enum HurtSoundType
    {
        Sound2D,

        Sound3D,

        Silent
    }

    [System.Serializable]
    public struct DamageInfo
    {
        public GameObject Source { get; set; }

        public int Damage;

        public AudioClip HurtSound;

        public HurtSoundType Type;

        public DamageInfo(GameObject aggressor, int dmg, AudioClip sound = null, HurtSoundType soundType = HurtSoundType.Sound2D)
        {
            this.Source = aggressor;
            this.Damage = dmg;
            this.HurtSound = sound;
            this.Type = soundType;
        }
    }

    public Health.HurtSoundType SoundType = Health.HurtSoundType.Sound2D;

    [Tooltip("GameObjects having one of those tags will not be able to Hurt this object.")]
    public List<string> IgnoredDamagerTags;


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
            bool damagerIsIgnored = IgnoredDamagerTags.Any(ignoredTag => dmgInfo.Source.CompareTag(ignoredTag));
            if (!damagerIsIgnored)
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
        // stop beating a dead horse!
        if (this.CurrentHP == 0)
            yield break;

        this.CurrentHP = Math.Max(this.CurrentHP - dmgInfo.Damage, 0);

        this.HurtEvent?.Invoke(this.CurrentHP);

        if (dmgInfo.Type != HurtSoundType.Silent)
            this.PlayHurtSound(dmgInfo);

        if (this.KnockOnDamageTaken)
        {
            Knockable knckbl = dmgInfo.Source.GetComponentInChildren<Knockable>()
                               ?? dmgInfo.Source.GetComponentInParent<Knockable>();

            if (knckbl)
                knckbl.Knockback();
        }

        if (this.InvincibilityDuration != 0f && this.CurrentHP != 0)
        {
            this.IsInvincible = true;
            this.InvincibleEvent?.Invoke(true);

            if (this.InvincibilityFlashingObjects.Length != 0)
            {
                StartCoroutine(this.FlashingRoutine());
            }

            yield return new WaitForSeconds(this.InvincibilityDuration);

            this.IsInvincible = false;
            this.InvincibleEvent?.Invoke(false);
        }
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

    }
}
