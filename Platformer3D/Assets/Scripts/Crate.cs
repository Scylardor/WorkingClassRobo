using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public GameObject lootObject;

    public int lootAmount = 1;

    public float lootRadius = 0.5f;

    public float lootHeightOffset = 0.5f;

    public GameObject destroyedParticles;

    public AudioClip destroyedSound;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {

        // spawn the particles
        Instantiate(this.destroyedParticles, other.transform.position, Quaternion.identity);

        // make the audio play
        AudioManager.Instance.Play3DSound(other.transform.position, this.destroyedSound);

        // spawn the loot in a radius around the object;
        float angle = 0f;
        for (int i = 0; i < this.lootAmount; ++i)
        {
            var objPos = other.transform.position;
            objPos.y -= this.lootHeightOffset;
            objPos.x += this.lootRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
            objPos.z += this.lootRadius * Mathf.Sin(angle * Mathf.Deg2Rad);

            Instantiate(this.lootObject, objPos, Quaternion.identity);

            angle += 360f / (float)this.lootAmount;
        }

        // our work here is done
        Destroy(this.gameObject);
    }
}
