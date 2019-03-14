using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour {
	public bool ladderEnabled = true;

	public void OnTriggerEnter2D(Collider2D other) {
		if (ladderEnabled && other.CompareTag("Player")) {
			other.GetComponent<PlayerController>().EnteredClimbable(gameObject);
		}
	}

	public void OnTriggerExit2D(Collider2D other) {
		if (ladderEnabled && other.CompareTag("Player")) {
			other.GetComponent<PlayerController>().ExitedClimbable(gameObject);
		}
	}
}
