using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerroFluidBehavior : MonoBehaviour {

    public float yPushScalar = 1;
    public float xPushMultiplier = 1.5f;
    public float yPushMultiplier = -0.7f;


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Crate"))
        {
            Rigidbody2D metalObject = collider.gameObject.GetComponentInParent<Rigidbody2D>();
            float x = metalObject.velocity.x;
            float y = metalObject.velocity.y;

            Vector2 push = new Vector2(xPushMultiplier * x, yPushScalar + yPushMultiplier * y);
            metalObject.velocity = push;
        }
    }
}
