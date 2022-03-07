using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public MeshRenderer     Renderer;
    public Material         TurnedOnMaterial;
    public Material         TurnedOffMaterial;
    public ParticleSystem   ActivationParticles;

    private bool            IsActivated = false;

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
        if (!IsActivated && other.tag == "Player")
        {
            // Deactivate all other checkpoints
            Checkpoint[] allCheckpoints = FindObjectsOfType<Checkpoint>();

            foreach (Checkpoint cp in allCheckpoints)
            {
                if (!ReferenceEquals(cp, this) && cp.IsActivated)
                {
                    cp.IsActivated = false;
                    var mats = cp.Renderer.materials;
                    mats[1] = cp.TurnedOffMaterial;
                    cp.Renderer.materials = mats;

                    cp.ActivationParticles.Clear(); // instantly kills all particles
                    cp.ActivationParticles.Stop();
                }
            }

            // Activate this one
            IsActivated = true;

            Debug.Log("Check point activated!");
            GameManager.Instance.SetSpawnPoint(transform.position);

            // Turn on the light of teh checkpoint (activated)
            var thisMats = Renderer.materials;
            thisMats[1] = TurnedOnMaterial;
            Renderer.materials = thisMats;

            ActivationParticles.Play();

            GetComponent<AudioSource>()?.Play();

        }
    }
}
