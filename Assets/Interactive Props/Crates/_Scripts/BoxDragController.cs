using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDragController : MonoBehaviour, IDraggable {
	public void Move(float dragSpeed) {
		GetComponent<Rigidbody2D>().velocity = new Vector2(dragSpeed, 0);
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			other.GetComponent<PlayerController>().EnteredDraggable(gameObject);
		}
	}

	public void OnTriggerExit2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			other.GetComponent<PlayerController>().ExitedDraggable(gameObject);
		}
	}
}
