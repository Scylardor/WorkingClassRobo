using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int HealAmount = 1;

    public GameObject PickedUpEffect;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        var hp = other.GetComponent<Health>();
        hp.Heal(HealAmount);

        Instantiate(PickedUpEffect, gameObject.transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
