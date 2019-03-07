using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueBehavior : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collider2d)
    {
        if (collider2d.gameObject.CompareTag("Crate") && collider2d is BoxCollider2D)
        {
            Rigidbody2D crate = collider2d.gameObject.GetComponentInParent<Rigidbody2D>();
            crate.Sleep();
        }
    }
}
