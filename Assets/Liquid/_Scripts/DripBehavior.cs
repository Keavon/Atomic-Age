using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DripBehavior : MonoBehaviour {

    // Movement speed in units/sec.
    public float speed = 1.0f;

    // Time when the movement started.
    private float startTime;

    private bool retracting = false;
    private bool extending = false;

    private void Start()
    {
        // Keep a note of the time the movement started.
        startTime = Time.time;
        transform.localScale = Vector3.zero;
    }

    void Update () {
        float distCovered = (Time.time - startTime) * speed;

        if (extending)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, distCovered);
        }
        else if (retracting)
        {
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, distCovered);
        } 
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    public void Retract()
    {
        retracting = true;
        extending = false;
        startTime = Time.time;
        speed *= 3;
    }

    public void Extend(){
        extending = true;
    }
}
