using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealth : MonoBehaviour
{
    // Start is called before the first frame update

    public Text HealthText;

    public GameObject HealthBar;

    public Sprite[] HealthBars;

    void Start()
    {
        var playerHP = PlayerController.Instance.GetComponent<Health>();
        if (playerHP != null)
        {
            HealthText.text = playerHP.CurrentHP.ToString();

            playerHP.HealEvent += this.OnPlayerHPUpdate;
            playerHP.HurtEvent += this.OnPlayerHPUpdate;
        }
    }

    private void OnPlayerHPUpdate(int newhp)
    {
        HealthText.text = newhp.ToString();
        var barImg = HealthBar.GetComponent<Image>();

        if (!(newhp >= 1 && newhp <= 5))
        {
            Debug.Log("Unsupported HP value, cannot update health bar !");
            barImg.enabled = false;
        }
        else
        {
            barImg.enabled = true;
            barImg.sprite = HealthBars[newhp - 1];
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
