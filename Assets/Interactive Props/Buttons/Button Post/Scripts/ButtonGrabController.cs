using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonGrabController : MonoBehaviour, IGrabbable {
	public GameObject actionObject;
	IActionable action;

	public void Start() {
		action = actionObject.GetComponent<IActionable>();
	}

	public void Force(Vector2 force) {
		action.Play();
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
