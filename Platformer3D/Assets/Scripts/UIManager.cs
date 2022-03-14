using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Image    BlackScreen;

    public float    FadeToBlackDuration;
    private bool     FadingIn, FadingOut;

    public GameObject PauseScreen;
    public GameObject OptionsScreen;

    public Slider MusicSlider;

    public Slider SFXSlider;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        PlayerController.Instance.OnPauseTriggered += this.TogglePauseUI;
        this.PauseScreen.SetActive(false);
        this.OptionsScreen.SetActive(false);

        // Start the level with fading out from the black screen
        BlackScreen.color = new Color(0f, 0f, 0f, 1f);

        this.FadingOut = true;
    }

    private void TogglePauseUI(bool paused)
    {
        this.PauseScreen.SetActive(paused);
        if (paused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            this.CloseOptions(); // in case the options screen was open, close it
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (this.FadingIn && this.FadingOut)
        {
            Debug.LogError("Error: UI manager cannot fade the black screen in and out at the same time!");
            return;
        }

        // use unscaled delta time because sometimes we wanna be able to fade out during pause ... (ie. when timeScale = 0f)
        if (FadingIn)
        {
            BlackScreen.color = new Color(0,0,0, Mathf.MoveTowards(BlackScreen.color.a, 1f, FadeToBlackDuration * Time.unscaledDeltaTime));

            if (BlackScreen.color.a == 1f)
            {
               FadingIn = false;
            }
        }
        else if (FadingOut)
        {
            BlackScreen.color = new Color(0,0,0, Mathf.MoveTowards(BlackScreen.color.a, 0f, FadeToBlackDuration * Time.unscaledDeltaTime));

            if (BlackScreen.color.a == 0f)
            {
                StopFadingOutBlackScreen();
            }
        }
    }

    public void StartFadingInBlackScreen()
    {
        this.FadingIn = true;
        this.BlackScreen.gameObject.SetActive(true);
    }
    public void StartFadingOutBlackScreen()
    {
        this.FadingOut = true;
    }

    public void StopFadingOutBlackScreen()
    {
        this.FadingOut = false;
        this.BlackScreen.gameObject.SetActive(false);
    }

    public void Resume()
    {
        PlayerController.Instance.TogglePause();

    }

    public void MainMenu()
    {
        GameManager.Instance.LoadLevel("MainMenu");
    }

    public void Options()
    {
        this.OptionsScreen.SetActive(true);
    }

    public void CloseOptions()
    {
        this.OptionsScreen.SetActive(false);
    }


    public void Quit()
    {
        Application.Quit();
    }

    public void SetMusicLevel()
    {
        AudioManager.Instance.SetMusicLevel(this.MusicSlider.value);
    }
    public void SetSFXLevel()
    {
        AudioManager.Instance.SetSFXLevel(this.SFXSlider.value);

    }
}
