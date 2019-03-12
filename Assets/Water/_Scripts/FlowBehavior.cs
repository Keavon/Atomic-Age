using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowBehavior : MonoBehaviour {

    public GameObject deathReactor;
    public float moveVertical = 0.5f;
    private bool overflow = false;
    private bool widen = false;
    private int count = 0;
    private float timer = 0.0f;
    private BoxCollider2D coll;

    // Use this for initialization
    void Start () {
        coll = deathReactor.GetComponent<BoxCollider2D>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (overflow)
        {
            Vector3 v = gameObject.transform.position;
            Vector3 w = gameObject.transform.localScale;
            if (count < 20)
            {
                v.y += 0.5f * Time.deltaTime/2;
                gameObject.transform.position = v;
                count++;
                timer = Time.time;

            }
            if(count >= 20 && (Time.time -timer) > 0.1)
            {
                w.x = 1.45f;
                if (count < 125)
                {
                    v.y += 0.5f * Time.deltaTime/2;
                    gameObject.transform.position = v;
                    gameObject.transform.localScale = w;
                    coll.size += new Vector2(0.07f, 0.038f);
                    count++;


                }

                /*v.y = -5.5f;
                    w.x = 1.53f;
                    gameObject.transform.position = v;
                    gameObject.transform.localScale = w;
                    deathReactor.GetComponent<BoxCollider2D>().size = new Vector2(16.64f, 5.4f);*/
            }
        }

    }

    public void Overflow()
    {
        overflow = true;
    }

    public void Widen()
    {
        widen = true;
    }
}
