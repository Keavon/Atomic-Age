using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverGrabController : MonoBehaviour, IGrabbable {
	public GameObject actionObject;
	public string actionableScript;

	IActionable actionable;
	GameObject arm;
	bool settle = true;
	float angle = 0;

	public void Start() {
		actionable = actionObject.GetComponent(actionableScript) as IActionable;
		arm = transform.Find("Arm").gameObject;
	}

	public void Update() {
		if (settle) {
			angle *= 0.9f;
			if (Mathf.Abs(angle) < 0.1f) angle = 0;

			Vector3 rotation = arm.transform.eulerAngles;
			rotation.z = angle;
			arm.transform.eulerAngles = -rotation;
		}
		settle = true;

		if (angle < -20) {
			actionable.Run(-1);
		}
		else if (angle > 20) {
			actionable.Run(1);
		}
	}

	public void Force(Vector2 force) {
		if (force.x <= -0.1 || force.x >= 0.1) settle = false;
		angle = Mathf.Clamp(angle + force.x, -22.5f, 22.5f);

		Vector3 rotation = arm.transform.eulerAngles;
		rotation.z = angle;
		arm.transform.eulerAngles = -rotation;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag("Player")) other.GetComponent<PlayerController>().EnteredGrabbable(gameObject);
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.CompareTag("Player")) other.GetComponent<PlayerController>().ExitedGrabbable(gameObject);
	}
}
