using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public bool shouldOpen = false;
    public bool shouldClose = false;

    public float OpeningSpeed = 1;

    public Transform doorTransf, openDoorTransf;

    private float invSpeed;

    private Quaternion startRot;

    // Start is called before the first frame update
    void Start()
    {
        this.invSpeed = 1f / this.OpeningSpeed;
        this.startRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.shouldOpen)
        {
            this.doorTransf.rotation = Quaternion.Slerp(this.doorTransf.rotation, this.openDoorTransf.rotation, Time.deltaTime * invSpeed);
        }
        else if (this.shouldClose)
        {
            this.doorTransf.rotation = Quaternion.Slerp(this.doorTransf.rotation, this.startRot, Time.deltaTime * invSpeed);
        }
    }
}
