using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDragController : MonoBehaviour, IDraggable {
	Rigidbody2D rb;

	public void Awake() {
		rb = GetComponent<Rigidbody2D>();
	}

	public void Move(float dragSpeed) {
		rb.AddForce(new Vector2(dragSpeed, 0), ForceMode2D.Impulse);
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
