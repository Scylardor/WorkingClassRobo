using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager  Instance;

    public AudioClip[]  MusicClips;

    public int ClipToPlay;

    private int PlayingClip;

    public AudioSource MusicPlayer;

    public AudioSource  SoundPlayer2D;
    public AudioSource  SoundPlayer3D;

    public AudioMixerGroup MusicMixer;

    public AudioMixerGroup SFXMixer;


    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayMusic(ClipToPlay);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayMusic(int musicIdx)
    {
        this.MusicPlayer.clip = MusicClips[musicIdx];
        this.MusicPlayer.Play();
        PlayingClip = musicIdx;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("Clip passed to PlayMusic is null");
            return;
        }

        this.MusicPlayer.clip = clip;
        this.MusicPlayer.Play();
    }


    public void Play2DSound(AudioClip clip = null, float volumeScale = 1)
    {
        if (this.SoundPlayer2D == null)
        {
            Debug.LogError("Tried to use Play2DSound but SoundPlayer2D is null");
            return;
        }

        if (clip != null)
        {
            this.SoundPlayer2D.PlayOneShot(clip, volumeScale);
        }
    }
    public void Play3DSound(Vector3 position, AudioClip clip = null)
    {
        if (this.SoundPlayer3D == null)
        {
            Debug.LogError("Tried to use Play3DSound but SoundPlayer3D is null");
            return;
        }

        if (clip != null)
        {
            this.transform.position = position;
            this.SoundPlayer3D.PlayOneShot(clip);
        }
    }
    public void OnValidate()
    {
        if (this.ClipToPlay != this.PlayingClip)
            PlayMusic(this.ClipToPlay);
    }


    public void SetMusicLevel(float value)
    {
        this.MusicMixer.audioMixer.SetFloat("MusicVolume", value);
    }

    public float GetMusicCurrentVolume()
    {
        float vol;
        this.MusicMixer.audioMixer.GetFloat("MusicVolume", out vol);
        return vol;
    }
    public void FadeMusicVolume(float duration, float targetVolume)
    {
        StartCoroutine(FadeVolumeRoutine("MusicVolume", duration, targetVolume));
    }

    public void Crossfade(float fadeOutDuration, float fadeInDuration, AudioClip incomingClip)
    {
        if (incomingClip == null)
            incomingClip = this.MusicClips[0];

        StartCoroutine(CrossfadeCo(fadeOutDuration, fadeInDuration, incomingClip));
    }

    private IEnumerator CrossfadeCo(float fadeOutDuration, float fadeInDuration, AudioClip incomingClip)
    {
        StartCoroutine(FadeVolumeRoutine("MusicVolume", fadeOutDuration, 0));

        yield return new WaitForSeconds(fadeOutDuration);

        MusicPlayer.clip = incomingClip;
        MusicPlayer.Play();

        StartCoroutine(FadeVolumeRoutine("MusicVolume", fadeInDuration, 1));
    }

    // Inspired by https://johnleonardfrench.com/how-to-fade-audio-in-unity-i-tested-every-method-this-ones-the-best/
    private IEnumerator FadeVolumeRoutine(string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        this.MusicMixer.audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            this.MusicMixer.audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }
    }


    public void SetSFXLevel(float value)
    {
        this.SFXMixer.audioMixer.SetFloat("SFXVolume", value);
    }
}
