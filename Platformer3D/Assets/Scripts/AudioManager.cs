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


    public void Play2DSound(AudioClip clip = null)
    {
        if (this.SoundPlayer2D == null)
        {
            Debug.LogError("Tried to use Play2DSound but SoundPlayer2D is null");
            return;
        }

        if (clip != null)
        {
            this.SoundPlayer2D.PlayOneShot(clip);
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


    public void SetSFXLevel(float value)
    {
        this.SFXMixer.audioMixer.SetFloat("SFXVolume", value);
    }
}
