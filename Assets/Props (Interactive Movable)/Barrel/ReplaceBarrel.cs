using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Trace Rainbolt
public class ReplaceBarrel : MonoBehaviour {

    public float spawnX;
    public float spawnY;

    
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("PropReplace"))
        {
            transform.localPosition = new Vector3(spawnX, spawnY, 0);
            GetComponent<Rigidbody2D>().velocity = Vector3.down * 10;
        }
    }
}
