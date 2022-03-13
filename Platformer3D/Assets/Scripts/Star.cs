using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public AudioClip PickedUpSound;
    public float SoundScale = 3;

    public AudioClip PickedUpJingle;

    public AudioSource starLoopSound;

    public float RotationSpeed = 10; // degrees per second

    public Animator AnimController;

    public float AnimationTimeUntilPause = 0f;
    public float AnimationPauseTime = 0f;

    public float CollectedMusicFadeOutDuration = 0.5f;

    public float CollectedMusicFadeInDuration = 5f;


    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnPlayerLanded()
    {
        PlayerController.Instance.OnLanded -= this.OnPlayerLanded;

        StartCoroutine(this.ScheduleCollectionMusic());
        StartCoroutine(this.ScheduleCollectedPlayerAnimation());
    }

    private IEnumerator ScheduleCollectionMusic()
    {
        AudioManager.Instance.Play2DSound(this.PickedUpJingle, SoundScale);

        yield return new WaitForSeconds(5f);

        AudioManager.Instance.FadeMusicVolume(CollectedMusicFadeInDuration, 1);
    }

    IEnumerator ScheduleCollectedPlayerAnimation()
    {
        PlayerController.Instance.Controller.enabled = false;

        PlayerController.Instance.PlayerAnimator.SetTrigger("ToggleThumbsUp");

        // to avoid player triggering running animation erroneously
        PlayerController.Instance.PlayerAnimator.SetFloat("Speed", 0f);

        if (this.AnimationTimeUntilPause != 0f)
        {
            yield return new WaitForSeconds(this.AnimationTimeUntilPause);

            // Freeze animation.
            float oldSpeed = PlayerController.Instance.PlayerAnimator.speed;
            PlayerController.Instance.PlayerAnimator.speed = 0f;

            yield return new WaitForSeconds(this.AnimationPauseTime);

            // Resume animation.
            PlayerController.Instance.PlayerAnimator.speed = oldSpeed;
        }

        PlayerController.Instance.Controller.enabled = true;
    }


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, this.RotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        AudioManager.Instance.Play2DSound(this.PickedUpSound, SoundScale);

        AudioManager.Instance.FadeMusicVolume(this.CollectedMusicFadeOutDuration, 0f);


        this.GetComponent<Collider>().enabled = false;
        starLoopSound.Stop();

        AnimController.SetTrigger("Collected");

        this.RotationSpeed *= 10f;

        if (PlayerController.Instance.Controller.isGrounded)
            this.OnPlayerLanded();
        else
            PlayerController.Instance.OnLanded += this.OnPlayerLanded;
    }
}
