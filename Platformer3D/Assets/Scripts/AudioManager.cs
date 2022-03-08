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

    public AudioSource  PersistentSFX2D;

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


    public void PlayPersistentSFX2D(AudioClip clip = null)
    {
        if (this.PersistentSFX2D == null)
        {
            Debug.LogError("Tried to use PlayPersistentSFX2D but PersistentSFX2D is null");
            return;
        }

        if (clip != null)
        {
            this.PersistentSFX2D.clip = clip;
        }
        this.PersistentSFX2D.Play();
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
