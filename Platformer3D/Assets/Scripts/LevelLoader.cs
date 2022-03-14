using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public string LevelToLoad;

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
        StartCoroutine(LevelLoadCo());
    }

    private IEnumerator LevelLoadCo()
    {
        UIManager.Instance.FadingIn = true;

        yield return new WaitForSeconds(UIManager.Instance.FadeToBlackDuration);

        SceneManager.LoadScene(this.LevelToLoad);
    }
}
