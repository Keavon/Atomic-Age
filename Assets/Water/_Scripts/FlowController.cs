using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class FlowController : MonoBehaviour {
    public GameObject poolWater;
    public GameObject sideWater;
    private float timer = 0.0f;
    private bool overflow_all = false;
    private BoxCollider2D bcol;

    // Use this for initialization
    void Start () {
        bcol = GetComponent<BoxCollider2D>();
    }
	
	// Update is called once per frame
	void Update () {
        //timer = Time.time;

        if(overflow_all)
        {
            timer = Time.time;
            Flood();  
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        poolWater.GetComponent<FlowBehavior>().Overflow();
        sideWater.GetComponent<FlowBehavior2>().Overflow();
        overflow_all = true;
    }

    private void Flood()
    {
        sideWater.GetComponent<FlowBehavior2>().Hide();
        poolWater.GetComponent<FlowBehavior>().Overflow();
        overflow_all = false;

    }

}
