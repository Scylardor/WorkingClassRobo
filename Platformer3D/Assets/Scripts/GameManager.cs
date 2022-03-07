using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Vector3 PlayerRespawnPosition;

    private int CoinAmount;

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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        PlayerRespawnPosition = PlayerController.Instance.transform.position;
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


    public IEnumerator  RespawnRoutine()
    {
        PlayerController.Instance.gameObject.SetActive(false);

        // Play death sound

        AudioManager.Instance.PlayDefaultSFX();

        // Spawn death fx
        Instantiate(PlayerDeathEffect, PlayerController.Instance.transform.position + new Vector3(0f, 1f, 0f), PlayerController.Instance.transform.rotation);

        // Necessary to avoid a weird camera warping on respawn
        CameraController.Instance.CameraBrain.enabled = false;

        UIManager.Instance.FadingIn = true;

        yield return new WaitForSeconds(2f);

        UIManager.Instance.FadingOut = true;

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
}
