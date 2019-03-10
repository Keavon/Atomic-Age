using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerroFluidBehavior : MonoBehaviour {

    public float maxDistance = 1.0f;

    private void FixedUpdate()
    {
        foreach(GameObject crate in GameObject.FindGameObjectsWithTag("Crate"))
        {
            float distanceMiddle = (crate.transform.position - GetComponent<LiquidBehavior>().location).magnitude;
            float distanceLeft = (crate.transform.position - GetComponent<LiquidBehavior>().leftLocation).magnitude;
            float distanceRight = (crate.transform.position - GetComponent<LiquidBehavior>().rightLocation).magnitude;

            float distance = Mathf.Min(distanceLeft, distanceRight, distanceMiddle);

            float distanceClamped = Mathf.Min(distance, 1.0f);
            float force = 1.0f / distanceClamped - 1;

            crate.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 5000f * force));
        }
    }
}
