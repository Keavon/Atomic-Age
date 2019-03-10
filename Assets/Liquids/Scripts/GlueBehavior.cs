using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueBehavior : MonoBehaviour {

    private void OnTriggerStay2D(Collider2D collider2d)
    {
        if (collider2d.gameObject.CompareTag("Crate"))
        {
            Rigidbody2D crate = collider2d.gameObject.GetComponent<Rigidbody2D>();
            crate.velocity = Vector3.zero;
        }
    }
}
