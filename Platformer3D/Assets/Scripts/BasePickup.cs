using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickup : MonoBehaviour
{

    public GameObject PickedUpEffect;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Instantiate(PickedUpEffect, gameObject.transform.position, Quaternion.identity);

            this.GetComponent<Renderer>().enabled = false;
            this.GetComponent<Collider>().enabled = false;

            var audio = this.GetComponent<AudioSource>();
            if (audio != null && audio.clip != null)
            {
                audio.Play();
                Destroy(gameObject, audio.clip.length);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
