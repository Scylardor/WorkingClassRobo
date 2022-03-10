using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Editor;

public class BasePickup : MonoBehaviour
{

    public GameObject PickedUpEffect;

    public AudioClip PickedUpSound;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            Instantiate(PickedUpEffect, gameObject.transform.position, Quaternion.identity);

            this.GetComponent<Renderer>().enabled = false;
            this.GetComponent<Collider>().enabled = false;

            if (this.PickedUpSound != null)
            {
                AudioManager.Instance.Play2DSound(this.PickedUpSound);
                Destroy(gameObject);
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Instantiate(PickedUpEffect, gameObject.transform.position, Quaternion.identity);

            this.GetComponent<Renderer>().enabled = false;
            this.GetComponent<Collider>().enabled = false;

            if (this.PickedUpSound != null)
            {
                AudioManager.Instance.Play2DSound(this.PickedUpSound);
                Destroy(gameObject);
            }
        }
    }
}
