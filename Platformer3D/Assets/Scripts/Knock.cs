using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Knock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void OnCollisionEnter(Collision other)
    {
        var knockable = other.gameObject.GetComponent<Knockable>();
        if (knockable)
            knockable.Knockback();
    }

    public void OnTriggerEnter(Collider other)
    {
        var knockable = other.GetComponent<Knockable>();
        if (knockable)
            knockable.Knockback();
    }
}
