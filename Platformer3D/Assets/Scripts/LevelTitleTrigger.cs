using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

public class LevelTitleTrigger : MonoBehaviour
{
    public Animator LevelTitleAnimator;

    public AudioClip previewMusic;

    public float fadePreviewMusicDuration = 0.1f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        this.LevelTitleAnimator.SetTrigger("ShowTitle");

        AudioManager.Instance.Crossfade(this.fadePreviewMusicDuration, this.fadePreviewMusicDuration, this.previewMusic);
    }

    private void OnTriggerExit(Collider other)
    {
        this.LevelTitleAnimator.SetTrigger("HideTitle");
        AudioManager.Instance.Crossfade(this.fadePreviewMusicDuration, this.fadePreviewMusicDuration, null);

    }
}
