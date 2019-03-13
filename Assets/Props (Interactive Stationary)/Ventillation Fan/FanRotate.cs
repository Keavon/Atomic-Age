using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanRotate : MonoBehaviour {

    public float fanPower;

	void FixedUpdate () {
        transform.Rotate(new Vector3(0, fanPower, 0));
	}
}
