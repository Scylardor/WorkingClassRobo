using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerCoins : MonoBehaviour
{
    // Start is called before the first frame update

    public Text CoinsText;


    void Start()
    {
        var manager = GameManager.Instance;
        if (manager != null)
        {
            manager.CoinChangeEvent += this.OnCoinAmountChanged;
            CoinsText.text = manager.GetCoins().ToString();
        }
    }

    private void OnCoinAmountChanged(int newcoinamount)
    {
        CoinsText.text = newcoinamount.ToString();
    }


    // Update is called once per frame
    void Update()
    {

    }
}
