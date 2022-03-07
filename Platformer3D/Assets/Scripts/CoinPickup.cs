using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public GameObject PickupEffect;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.AddCoins(1);

            Instantiate(PickupEffect, gameObject.transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

    }
}
