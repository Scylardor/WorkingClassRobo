using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

public class RectractSpikes : MonoBehaviour
{
    [Tooltip("The amount of seconds spikes are going to stay up/down before switching state")]
    public float Period = 1f;

    public float MovementSpeed = 1f;

    public float MovementOffset = 3f;

    public bool automaticMoveUp = true;

    private bool shouldGetDown = false;

    private bool shouldGetUp = true;

    private Vector3 targetPosition;

    private Vector3 downPosition;

    public bool paused = false;

    private IEnumerator positionSwitchRoutine = null;


    // Start is called before the first frame update
    void Start()
    {
        this.downPosition = transform.position;

        if (this.shouldGetUp)
        {
            this.targetPosition = this.transform.position + (Vector3.up * this.MovementOffset);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.paused)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            this.targetPosition,
            Time.deltaTime * this.MovementSpeed);

        if (transform.position.Equals(this.targetPosition) && this.positionSwitchRoutine == null)
        {
            positionSwitchRoutine = this.SwitchPosition();
            StartCoroutine(this.positionSwitchRoutine);
        }
    }

    public void GoUp()
    {
        this.paused = false;
        this.shouldGetUp = true;
        this.shouldGetDown = false;
        this.targetPosition = this.transform.position + (Vector3.up * this.MovementOffset);
    }

    private IEnumerator SwitchPosition()
    {
        this.paused = true;

        yield return new WaitForSeconds(this.Period);

         this.shouldGetDown = !this.shouldGetDown;
        this.shouldGetUp = !this.shouldGetUp;

        if (this.shouldGetDown)
        {
            this.targetPosition = downPosition;
        }
        else
        {
            this.targetPosition = downPosition + (Vector3.up * this.MovementOffset);
        }

        // In case automaticMoveUp was set to false, moving up again will be controlled by someone else.
        // If its not true, stay down and wait for the signal.
        if (!(this.shouldGetUp && !this.automaticMoveUp))
            this.paused = false;

        this.positionSwitchRoutine = null;
    }
}
