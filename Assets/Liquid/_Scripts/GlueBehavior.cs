using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueBehavior : MonoBehaviour {

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Crate") && collider is BoxCollider)
        {
            Rigidbody2D crate = collider.gameObject.GetComponentInParent<Rigidbody2D>();
            crate.Sleep();
        }
    }
}
