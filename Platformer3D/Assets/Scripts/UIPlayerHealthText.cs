using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealthText : MonoBehaviour
{
    // Start is called before the first frame update

    public Text HealthText;

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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
