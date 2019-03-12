using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class FlowController : MonoBehaviour {


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || (other.CompareTag("Crate")))
        {
            transform.parent.GetComponent<FlowBehavior>().Overflow();
        }
    }



}
