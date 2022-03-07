using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[]  MusicClips;
    public AudioClip[]  SFXClips;

    public int ClipToPlay;

    private int PlayingClip;

    public AudioSource Player;


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
        Player.clip = MusicClips[musicIdx];
        Player.Play();
        PlayingClip = musicIdx;
    }

    public void OnValidate()
    {
        if (this.ClipToPlay != this.PlayingClip)
            PlayMusic(this.ClipToPlay);
    }
}
