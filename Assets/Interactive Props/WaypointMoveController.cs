using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WaypointMoveController : MonoBehaviour, IActionable {
	public bool autoplay = false;
	public bool ignoreY = false;
	public Vector2[] waypoints;
	public float[] intervals;

	Rigidbody2D rb;
	int currentWaypoint = 0;
	float timeIntoWaypoint = 0;
	bool playing = false;

	public void OnValidate() {
		
	}

	public void Start() {
		if (autoplay) Run();
	}

	public void Run(params int[] inputs) {
		Assert.AreEqual(waypoints.Length - 1, intervals.Length);

		rb = GetComponent<Rigidbody2D>();
		playing = true;
	}

	public void Update() {
		if (!playing) return;

		float duration = intervals[currentWaypoint];

		if (timeIntoWaypoint >= duration && currentWaypoint < waypoints.Length - 1) {
			currentWaypoint++;
			timeIntoWaypoint = 0;
		}

		if (timeIntoWaypoint < duration && currentWaypoint < waypoints.Length - 1) {
			Vector2 moveVector = (waypoints[currentWaypoint + 1] - waypoints[currentWaypoint]) / duration;
			rb.velocity = ignoreY ? new Vector2(moveVector.x, rb.velocity.y) : moveVector; // TODO: TEMP
			timeIntoWaypoint += Time.deltaTime;
		}
		else {
			rb.velocity = ignoreY ? new Vector2(0, rb.velocity.y) : Vector2.zero; // TODO: TEMP
			currentWaypoint = 0;
			timeIntoWaypoint = 0;
			playing = false;
		}
	}

	// void PushAtVelocity(Vector2 velocity, float acceleration) {
	// 	Vector2 difference = velocity - new Vector2(rb.velocity.x, rb.velocity.y);
	// 	Vector2 momentaryForce = difference * acceleration;
	// 	Debug.Log(momentaryForce);
	// 	rb.AddForce(momentaryForce);
	// }
}
