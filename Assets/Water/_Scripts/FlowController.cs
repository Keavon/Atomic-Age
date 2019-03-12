using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class FlowController : MonoBehaviour {
    //public GameObject Water;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        transform.parent.GetComponent<FlowBehavior>().Overflow();
        //Water.GetComponent<FlowBehavior>().Overflow();
    }



}
