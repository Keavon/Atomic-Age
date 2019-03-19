using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Trace Rainbolt
public class FerroFluidBehavior : MonoBehaviour {

    public float maxDistance = 1.0f;

    private void FixedUpdate()
    {
        foreach(GameObject crate in GameObject.FindGameObjectsWithTag("Crate"))
        {
            float distanceMiddle = (crate.transform.position - GetComponent<LiquidBehavior>().location).magnitude;
            float distanceLeft = (crate.transform.position - GetComponent<LiquidBehavior>().leftLocation).magnitude;
            float distanceRight = (crate.transform.position - GetComponent<LiquidBehavior>().rightLocation).magnitude;

            float yVal = GetComponent<LiquidBehavior>().location.y;
            float distance = Mathf.Min(distanceLeft, distanceRight, distanceMiddle);

            float distanceClamped = Mathf.Min(distance, 1.0f);
            float force = 1.0f / distanceClamped - 1;

            if (crate.transform.position.y > yVal)
            {
                crate.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 1700000f * force));
            }
            else
            {
                crate.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -1700000f * force));

            }
        }
    }
}
