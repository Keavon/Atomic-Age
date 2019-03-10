using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonGrabController : MonoBehaviour, IGrabbable {
	public GameObject actionObject;
	public string actionableScript;

	IActionable actionable;

	public void Start() {
		actionable = actionObject.GetComponent(actionableScript) as IActionable;
	}

	public void Force(Vector2 force) {
		actionable.Run();
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			other.GetComponent<PlayerController>().EnteredGrabbable(gameObject);
		}
	}

	public void OnTriggerExit2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			other.GetComponent<PlayerController>().ExitedGrabbable(gameObject);
		}
	}
}
