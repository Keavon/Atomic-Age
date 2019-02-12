using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverGrabController : MonoBehaviour, IGrabbable {
	public GameObject leverArm;

	public void Force(Vector2 force) {
		leverArm.transform.eulerAngles = new Vector3(0, 0, -force.x * 30);
		Debug.Log("Lever given  " + force);
	}
}
