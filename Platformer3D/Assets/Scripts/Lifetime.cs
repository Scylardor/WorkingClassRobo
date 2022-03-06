using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [Min(0f)]
    public float LifetimeDuration = 0f;
    // Start is called before the first frame update
    void Start()
    {
        if (LifetimeDuration != 0f)
        {
            Destroy(gameObject, LifetimeDuration);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
