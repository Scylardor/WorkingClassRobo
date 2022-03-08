using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Image    BlackScreen;

    public float    FadeToBlackDuration;
    public bool     FadingIn, FadingOut;

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
        if (FadingIn)
        {
            BlackScreen.color = new Color(BlackScreen.color.r, BlackScreen.color.g, BlackScreen.color.b, Mathf.MoveTowards(BlackScreen.color.a, 1f, FadeToBlackDuration * Time.deltaTime));
           // BlackScreen.CrossFadeAlpha(1, FadeToBlackDuration, false);

           if (BlackScreen.color.a == 1f)
           {
               FadingIn = false;
           }
        }

        if (FadingOut)
        {
            BlackScreen.color = new Color(BlackScreen.color.r, BlackScreen.color.g, BlackScreen.color.b, Mathf.MoveTowards(BlackScreen.color.a, 0f, FadeToBlackDuration * Time.deltaTime));
            // BlackScreen.CrossFadeAlpha(1, FadeToBlackDuration, false);

            if (BlackScreen.color.a == 0f)
            {
                FadingOut = false;
            }
        }
    }


    public void Resume()
    {
        PlayerController.Instance.TogglePause();

    }

    public void MainMenu()
    {

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
