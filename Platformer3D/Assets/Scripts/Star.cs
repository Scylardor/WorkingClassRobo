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


    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnPlayerLanded()
    {
        PlayerController.Instance.OnLanded -= this.OnPlayerLanded;
        PlayerController.Instance.Controller.enabled = false;
        PlayerController.Instance.PlayerAnimator.SetTrigger("ToggleThumbsUp");
        PlayerController.Instance.PlayerAnimator.SetFloat("Speed", 0f);
        StartCoroutine(this.PickupMusicManagement());
    }

    IEnumerator PickupMusicManagement()
    {
        AudioManager.Instance.Play2DSound(this.PickedUpJingle);
        yield return new WaitForSeconds(5f);
        AudioManager.Instance.FadeMusicVolume(5f, 1);
        PlayerController.Instance.Controller.enabled = true;
        yield return null;
    }


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, this.RotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        AudioManager.Instance.Play2DSound(this.PickedUpSound, 3);

        AudioManager.Instance.FadeMusicVolume(0.5f, 0f);

        PlayerController.Instance.OnLanded += this.OnPlayerLanded;

        this.GetComponent<Collider>().enabled = false;
        starLoopSound.Stop();

        AnimController.SetTrigger("Collected");

        this.RotationSpeed *= 5f;
    }
}
