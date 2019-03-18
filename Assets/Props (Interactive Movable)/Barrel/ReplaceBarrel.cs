using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceBarrel : MonoBehaviour {

    public float spawnX;
    public float spawnY;

    
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("PropReplace"))
        {
            transform.localPosition = new Vector3(spawnX, spawnY, 0);
        }
    }
}
