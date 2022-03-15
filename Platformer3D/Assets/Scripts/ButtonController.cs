using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public float DepressDuration = 0f;
    public float DepressAmount = -2f;

    public Material pressedMaterial;

    private bool isPressed = false;

    private Material originalMat;

    private MeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {
        this.mesh = this.GetComponent<MeshRenderer>();
        if (this.mesh)
            this.originalMat = mesh.material;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!this.isPressed && this.DepressDuration != 0f)
        {
            StartCoroutine(PressCo());
        }
    }

    private IEnumerator PressCo()
    {
       this.isPressed = true;
       transform.Translate(Vector3.up * this.DepressAmount);

       if (this.mesh)
           this.mesh.material = this.pressedMaterial;

       yield return new WaitForSeconds(this.DepressDuration);

       this.isPressed = false;
       transform.Translate(Vector3.up * -this.DepressAmount);

       if (this.mesh)
       {
           this.mesh.material = this.originalMat;
       }
    }
}
