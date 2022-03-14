using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Vector3 PlayerRespawnPosition;

    private int CoinAmount;

    private int StarAmount;

    public GameObject   PlayerDeathEffect;

    public delegate void RespawnEvent();
    public event RespawnEvent   OnPlayerDeath;
    public event RespawnEvent   OnPlayerRespawning;

    public delegate void CoinAmountChange(int newCoinAmount);
    public event CoinAmountChange CoinChangeEvent;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerRespawnPosition = PlayerController.Instance.transform.position;

        PlayerController.Instance.OnPauseTriggered += this.ToggleWorldPause;

        Time.timeScale = 1f; // in case we went to main menu directly from pause menu previously

        // Count the number of coins and update player preferences with it if it hasn't already been done
        var curSceneName = SceneManager.GetActiveScene().name;

        var maxCoinsProperty = curSceneName + "_maxCoins";
        if (GetIntPropertyOr(maxCoinsProperty, -1) == -1)
        {
            int levelTotalCoinAmount = CalculateMaxCoinsInLevel();
            this.SetIntProperty(maxCoinsProperty, levelTotalCoinAmount);
        }

        // same with stars
        var maxStarsProperty = curSceneName + "_maxStars";
        this.RegisterLevelMaxPickupCount<Star>(maxStarsProperty);
    }

    public int GetIntPropertyOr(string prop, int defaultValue)
    {
        int val = PlayerPrefs.GetInt(prop, defaultValue);
        return val;
    }

    public void SetIntProperty(string prop, int value)
    {
        PlayerPrefs.SetInt(prop, value);
    }


    private int CalculateMaxCoinsInLevel()
    {
        // First, calculate the total value of the coin pickups in the level...
        CoinPickup[] objs = GameObject.FindObjectsOfType<CoinPickup>();
        var totalCoins = 0;
        foreach (CoinPickup cp in objs)
        {
            totalCoins += cp.CoinsGained;
        }

        // But enemies can also drop coins. analyze each enemy to evaluate their total coin value.
        EnemyAnimator[] enemies = GameObject.FindObjectsOfType<EnemyAnimator>();
        foreach (EnemyAnimator ea in enemies)
        {
            CoinPickup prefabPickup = ea.DroppedPickup.GetComponentInChildren<CoinPickup>();
            if (prefabPickup != null)
                totalCoins += prefabPickup.CoinsGained;
        }

        // we now have the total coin amount of the level
        return totalCoins;
    }

    private void RegisterLevelMaxPickupCount<T>(string property)
    {
        if (!PlayerPrefs.HasKey(property))
        {
            Object[] objs = GameObject.FindObjectsOfType(typeof(T));

            PlayerPrefs.SetInt(property, objs.Length);
        }
    }

    private void ToggleWorldPause(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void RespawnPlayer()
    {
        StartCoroutine(RespawnRoutine());
    }


    public void AddCoins(int nbCoinAdded)
    {
        CoinAmount += nbCoinAdded;
        CoinChangeEvent(CoinAmount);
    }

    public int GetCoins()
    {
        return CoinAmount;
    }

    public void CollectStar()
    {
        this.StarAmount++;
    }


    public IEnumerator  RespawnRoutine()
    {
        PlayerController.Instance.gameObject.SetActive(false);

        // Spawn death fx
        Instantiate(PlayerDeathEffect, PlayerController.Instance.transform.position + new Vector3(0f, 1f, 0f), PlayerController.Instance.transform.rotation);

        // Necessary to avoid a weird camera warping on respawn
        CameraController.Instance.CameraBrain.enabled = false;

        UIManager.Instance.StartFadingInBlackScreen();

        yield return new WaitForSeconds(2f);

        UIManager.Instance.StartFadingOutBlackScreen();

        PlayerController.Instance.transform.position = PlayerRespawnPosition;
        PlayerController.Instance.gameObject.SetActive(true);

        CameraController.Instance.CameraBrain.enabled = true;

        // Reset all Health components to default
        Health[] hps = FindObjectsOfType<Health>();
        foreach (Health hp in hps)
        {
            hp.ResetHealth();
        }

        PlayerController.Instance.GetComponent<Knockable>().IsKnockedBack = false;
    }

    public void SetSpawnPoint(Vector3 newSpawnPoint)
    {
        PlayerRespawnPosition = newSpawnPoint;
    }

    public void LoadLevel(string levelName)
    {
        StartCoroutine(LevelLoadCo(levelName));
    }

    private IEnumerator LevelLoadCo(string levelName)
    {
        // Save our number of collected coins and collected stars
        var curSceneName = SceneManager.GetActiveScene().name;
        var coinsProp = curSceneName + "_coins";
        if (this.CoinAmount > GetIntPropertyOr(coinsProp, 0))
        {
            SetIntProperty(coinsProp, this.CoinAmount);
        }

        // same with stars
        var starsProp = curSceneName + "_stars";
        if (this.StarAmount > GetIntPropertyOr(starsProp, 0))
        {
            SetIntProperty(starsProp, this.StarAmount);
        }

        UIManager.Instance.StartFadingInBlackScreen();

        // Use WaitForSecondsRealtime because we don't wanna be affected by pause time scale...
        yield return new WaitForSecondsRealtime(UIManager.Instance.FadeToBlackDuration);

        SceneManager.LoadScene(levelName);
    }
}
