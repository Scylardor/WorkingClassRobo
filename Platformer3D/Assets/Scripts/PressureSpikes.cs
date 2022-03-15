using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

public class PressureSpikes : MonoBehaviour
{
    public float GracePeriod = .5f;

    private RectractSpikes retractSpikesScript;

    private IEnumerator goUpRoutine = null;

    // Start is called before the first frame update
    void Start()
    {
        this.retractSpikesScript = this.GetComponentInChildren<RectractSpikes>();
        if (this.retractSpikesScript != null)
        {
            this.retractSpikesScript.paused = true;
            this.retractSpikesScript.automaticMoveUp = false;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.retractSpikesScript && this.goUpRoutine == null)
        {
            goUpRoutine = this.GoUpCo();
            StartCoroutine(this.goUpRoutine);
        }
    }

    private IEnumerator GoUpCo()
    {
        yield return new WaitForSeconds(this.GracePeriod);

        this.retractSpikesScript.GoUp();
        this.goUpRoutine = null;
    }
}
