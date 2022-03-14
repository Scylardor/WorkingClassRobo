using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using Unity.VisualScripting;

using UnityEngine;

public class LevelTitleTrigger : MonoBehaviour
{
    public Animator LevelTitleAnimator;

    public AudioClip previewMusic;

    public float fadePreviewMusicDuration = 0.1f;

    public TextMeshPro  LevelText;

    public LevelLoader Loader;


    // Start is called before the first frame update
    void Start()
    {
        var gm = GameManager.Instance;
        // Fill the "blanks" in the text with what we know from the level.
        var collectedCoins = gm.GetIntPropertyOr(this.Loader.name + "_coins", 0);
        var maxCoinsProperty = this.Loader.LevelToLoad + "_maxCoins";
        var maxCoins = gm.GetIntPropertyOr(maxCoinsProperty, -1);
        string maxCoinsStr = maxCoins != -1 ? maxCoins.ToString() : "???";

        var collectedStars = gm.GetIntPropertyOr(this.Loader.name + "_stars", 0);
        var maxStarsProperty = this.Loader.LevelToLoad + "_maxStars";
        var maxStars = gm.GetIntPropertyOr(maxStarsProperty, -1);
        string maxStarStr = maxStars != -1 ? maxStars.ToString() : "???";

        this.LevelText.text = string.Format(this.LevelText.text, collectedCoins, maxCoinsStr, collectedStars, maxStarStr);
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
