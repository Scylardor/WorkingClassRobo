
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

public class CoinPickup : BasePickup
{
    public int CoinsGained = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.tag == "Player")
        {
            GameManager.Instance.AddCoins(CoinsGained);
        }
    }
}
