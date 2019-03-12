using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainOrLoseMop : MonoBehaviour {
	public void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			other.GetComponent<PlayerController>().mopState = MopState.CleanMop;
			Destroy(gameObject);
		}
	}
}
