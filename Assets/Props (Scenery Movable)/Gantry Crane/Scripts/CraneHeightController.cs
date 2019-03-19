using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneHeightController : MonoBehaviour, IActionable {
	public float verticalSpeed = 1.0f;
	public float minMove;
	public float maxMove;

	Rigidbody2D craneRigidbody;
	GameObject pulley;
	GameObject cables;
	float initialHeight;
	float initialHeightPulley;
	bool moving = false;

	public void Start() {
		craneRigidbody = gameObject.GetComponent<Rigidbody2D>();

		pulley = transform.Find("Pulley").gameObject;
		cables = transform.Find("Cables").gameObject;

		initialHeight = craneRigidbody.transform.position.y;
		initialHeightPulley = pulley.transform.localPosition.y;
	}

	public void Run(params int[] inputs) {
		float requestedMove = inputs[0];
		if (craneRigidbody.transform.position.y - initialHeight <= minMove) requestedMove = Mathf.Max(0, requestedMove);
		if (craneRigidbody.transform.position.y - initialHeight >= maxMove) requestedMove = Mathf.Min(0, requestedMove);

		Vector3 raisableVelocity = craneRigidbody.velocity;
		raisableVelocity.y = requestedMove * verticalSpeed;
		craneRigidbody.velocity = raisableVelocity;
		moving = true;

		repositionPulleysAndRope();
	}

	public void Update() {
		if (!moving || craneRigidbody.transform.position.y - initialHeight <= minMove || craneRigidbody.transform.position.y - initialHeight >= maxMove) {
			Vector3 raisableVelocity = craneRigidbody.velocity;
			raisableVelocity.y = 0;
			craneRigidbody.velocity = raisableVelocity;

			repositionPulleysAndRope();
		}

		if (moving) moving = false;
	}

	void repositionPulleysAndRope() {
		float offset = craneRigidbody.transform.position.y - initialHeight;
		Vector3 pulleyPosition = pulley.transform.localPosition;
		pulleyPosition.y = initialHeightPulley - offset;
		pulley.transform.localPosition = pulleyPosition;
	}
}
