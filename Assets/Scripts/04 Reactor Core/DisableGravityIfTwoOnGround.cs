using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGravityIfTwoOnGround : MonoBehaviour {
	public GameObject slidySet;
	public PhysicsMaterial2D slippery;
	public int total = 0;
	int reenable = 0;

	public void Update() {
		if (reenable == 100) {
			GetComponent<BoxCollider2D>().enabled = true;
		}
		else reenable++;
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (!other.isTrigger && other.CompareTag("Crate")) total++;
		React();
	}

	public void OnTriggerLeave2D(Collider2D other) {
		if (!other.isTrigger && other.CompareTag("Create")) total--;
		React();
	}

	void React() {
		if (total == 2) {
			slidySet.GetComponent<Collider2D>().sharedMaterial = slippery;
		}
	}
}
