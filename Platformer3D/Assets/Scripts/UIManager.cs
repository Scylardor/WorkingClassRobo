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

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

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
}
