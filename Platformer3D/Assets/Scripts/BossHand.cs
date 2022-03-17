using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHand : MonoBehaviour
{
    public GameObject brokenParticles;

    public Material brokenMaterial;

    public GameObject emitter;

    // Start is called before the first frame update
    void Start()
    {

        var HP = this.GetComponentInChildren<Health>();
        if (HP)
            HP.HurtEvent += this.OnHurt;
    }

    private void OnHurt(int newhp)
    {
        if (newhp == 0)
        {
            Instantiate(this.brokenParticles, this.transform);
            var mesh = this.GetComponent<MeshRenderer>();
            if (mesh)
            {
                Material[] mats = mesh.materials;
                mats[0] = this.brokenMaterial;
                mesh.materials = mats;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
