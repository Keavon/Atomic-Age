using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Vanessa Lopez
public class FlowBehavior : MonoBehaviour {

    public GameObject mainWater;
    public GameObject sideWater;
    private bool overflow = false;
    private int count = 0;
    private float timer = 0.0f;
    private BoxCollider2D coll;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (overflow)
        {
            Vector3 u = sideWater.transform.position;
            Vector3 v = mainWater.transform.position;
            Vector3 w = mainWater.transform.localScale;
            if (count < 20)
            {
                v.y += 0.5f * Time.deltaTime/2;
                mainWater.transform.position = v;
                count++;
                timer = Time.time;

            }
            if(count >= 20 && (Time.time -timer) > 0.1)
            {
                if(count < 70)
                {
                    u.y += 0.5f * Time.deltaTime / 2;
                    gameObject.transform.position = u;
                    count++;
                }
                if(count >= 70)
                {
                    w.x = 15f;

                }
                if (count < 145)
                {
                    v.y += 0.5f * Time.deltaTime/2;
                    mainWater.transform.position = v;
                    mainWater.transform.localScale = w;
                    count++;


                }
            }
        }

    }

    public void Overflow()
    {
        overflow = true;
    }
}
