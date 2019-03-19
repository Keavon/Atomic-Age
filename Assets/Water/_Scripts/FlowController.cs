using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

//Author: Vanessa Lopez
public class FlowController : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger && (other.CompareTag("Player") || (other.CompareTag("Crate"))))
        {
            transform.parent.GetComponent<FlowBehavior>().Overflow();
        }
    }
}
