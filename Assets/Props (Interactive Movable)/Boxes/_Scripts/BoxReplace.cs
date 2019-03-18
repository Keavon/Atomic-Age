using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxReplace : MonoBehaviour {

    public float spawnX;
    public float spawnY;


    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("PropReplace"))
        {
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            transform.localPosition = new Vector3(spawnX, spawnY, 0);
            transform.rotation = Quaternion.identity;
        }
    }
}
