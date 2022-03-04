using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Vector3 PlayerRespawnPosition;

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


    public IEnumerator  RespawnRoutine()
    {
        PlayerController.Instance.gameObject.SetActive(false);

        // Necessary to avoid a weird camera warping on respawn
        CameraController.Instance.CameraBrain.enabled = false;

        UIManager.Instance.FadingIn = true;

        yield return new WaitForSeconds(2f);

        UIManager.Instance.FadingOut = true;

        PlayerController.Instance.transform.position = PlayerRespawnPosition;
        PlayerController.Instance.gameObject.SetActive(true);

        CameraController.Instance.CameraBrain.enabled = true;


    }
}
