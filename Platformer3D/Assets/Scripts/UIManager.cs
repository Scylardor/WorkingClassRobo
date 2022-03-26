using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Image    BlackScreen;

    public float    FadeToBlackDuration;
    private bool     FadingIn, FadingOut;

    public GameObject PauseScreen;
    public GameObject OptionsScreen;

    public GameObject pauseFirstSelectedButton, optionsFirstSelectedButton;

    public Slider MusicSlider;

    public Slider SFXSlider;

    public bool lockCursor = true;

    // Sometimes (e.g. at scene loading) unscaled delta time can become huge.
    // In these cases, check if it's huge, and if so, use regular delta time instead.
    private float UnscaledTimeTolerance = 0.5f;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (this.lockCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        PlayerController.Instance.OnPauseTriggered += this.TogglePauseUI;

        if (this.PauseScreen)
            this.PauseScreen.SetActive(false);

        if (this.OptionsScreen)
            this.OptionsScreen.SetActive(false);

        if (this.BlackScreen)
        {
            // Start the level with fading out from the black screen
            this.BlackScreen.gameObject.SetActive(true);
            BlackScreen.color = new Color(0f, 0f, 0f, 1f);
            this.StartFadingOutBlackScreen();
        }


        // Preset the music level sliders with the correct values
        float playerVolume = PlayerPrefs.GetFloat("MaxMusicVolume", 0f);
        this.MusicSlider.SetValueWithoutNotify(playerVolume);

        float sfxVolume = PlayerPrefs.GetFloat("MaxSFXVolume", 0f);
        this.SFXSlider.SetValueWithoutNotify(sfxVolume);
    }

    private void TogglePauseUI(bool paused)
    {
        this.PauseScreen.SetActive(paused);
        if (paused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // clear selected object
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(this.pauseFirstSelectedButton);
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
            float dt = Time.unscaledDeltaTime < UnscaledTimeTolerance ? Time.unscaledDeltaTime : Time.deltaTime;
            BlackScreen.color = new Color(0,0,0, Mathf.MoveTowards(BlackScreen.color.a, 1f, FadeToBlackDuration * dt));

            if (BlackScreen.color.a == 1f)
            {
               FadingIn = false;
            }
        }
        else if (FadingOut)
        {
            float dt = Time.unscaledDeltaTime < UnscaledTimeTolerance ? Time.unscaledDeltaTime : Time.deltaTime;
            BlackScreen.color = new Color(0,0,0, Mathf.MoveTowards(BlackScreen.color.a, 0f, FadeToBlackDuration * dt));

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

        // clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(this.optionsFirstSelectedButton);
    }

    public void CloseOptions()
    {
        this.OptionsScreen.SetActive(false);
        // clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(this.pauseFirstSelectedButton);
    }


    public void Quit()
    {
        Application.Quit();
    }

    public void SetMusicLevel()
    {
        AudioManager.Instance.SetMusicLevel(this.MusicSlider.value);
        PlayerPrefs.SetFloat("MaxMusicVolume", this.MusicSlider.value);
    }
    public void SetSFXLevel()
    {
        AudioManager.Instance.SetSFXLevel(this.SFXSlider.value);
        PlayerPrefs.SetFloat("MaxSFXVolume", this.SFXSlider.value);

    }
}
