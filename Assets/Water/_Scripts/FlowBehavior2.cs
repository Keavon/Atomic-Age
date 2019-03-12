using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowBehavior2 : MonoBehaviour {

    private bool overflow = false;
    private bool hide = false;
    private int count = 0;
    private float timer = 0.0f;

    // Use this for initialization
    void Start () {
		
	}

    void FixedUpdate()
    {

        if (overflow)
        {
            Vector3 v = gameObject.transform.position;
            if (count < 20)
            {
                count++;
            }
            if (count >= 20 && count < 70)
            {
                v.y += 0.5f * Time.deltaTime/2;
                gameObject.transform.position = v;
                count++;
                timer = Time.time;

            }

            if (count >= 70) // && (Time.time -timer) > 0.5)
            {
                gameObject.SetActive(false);

            }

        }
       
    }


    public void Overflow()
    {
        overflow = true;
    }

    public void Hide()
    {
        hide = true;
    }
}
