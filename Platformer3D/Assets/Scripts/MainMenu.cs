using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string firstLevel;

    public string levelSelect;

    public GameObject continueButton;

    // Start is called before the first frame update
    void Start()
    {
        this.continueButton.SetActive(PlayerPrefs.HasKey("Continue"));

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NewGame()
    {
        PlayerPrefs.SetInt("Continue", 1);
        SceneManager.LoadScene(firstLevel);
    }
    public void Continue()
    {
        SceneManager.LoadScene(levelSelect);
    }
    public void Options()
    {

    }
    public void Quit()
    {
        Application.Quit();
    }
}
