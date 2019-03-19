using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GentleForce : MonoBehaviour, IActionable {
	public bool isEnabled = false;
	public Vector2 forceAmount;

	public void Run(params int[] inputs) {
		isEnabled = true;
	}

	public void OnTriggerStay2D(Collider2D other) {
		if (isEnabled && !other.isTrigger && other.CompareTag("Crate")) {
			Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
			rb.AddForce(forceAmount * rb.mass);
			Debug.Log(forceAmount);
		}
	}
}
