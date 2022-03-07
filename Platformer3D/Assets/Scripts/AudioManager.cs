using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager  Instance;

    public AudioClip[]  MusicClips;
    public AudioClip[]  SFXClips;

    public int ClipToPlay;

    private int PlayingClip;

    public AudioSource MusicPlayer;

    public AudioSource SFXPlayer;



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

    public void PlaySFX(AudioSource source)
    {

    }


    public void PlayDefaultSFX()
    {
        this.GetComponent<AudioSource>()?.Play();
    }

    public void OnValidate()
    {
        if (this.ClipToPlay != this.PlayingClip)
            PlayMusic(this.ClipToPlay);
    }
}
