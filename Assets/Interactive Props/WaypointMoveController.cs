using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WaypointMoveController : MonoBehaviour, IActionable {
	public bool autoplay = false;
	public Vector2[] waypoints;
	public float[] intervals;

	Rigidbody2D rb;
	int currentWaypoint = 0;
	float timeIntoWaypoint = 0;
	bool playing = false;

	public void OnValidate() {
		
	}

	public void Start() {
		if (autoplay) Play();
	}

	public void Play() {
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
			Vector2 moveVector = waypoints[currentWaypoint + 1] - waypoints[currentWaypoint];
			rb.velocity = moveVector / duration;
			timeIntoWaypoint += Time.deltaTime;
		}
		else {
			rb.velocity = Vector2.zero;
			playing = false;
			currentWaypoint = 0;
			timeIntoWaypoint = 0;
		}
	}

	// void PushAtVelocity(Vector2 velocity, float acceleration) {
	// 	Vector2 difference = velocity - new Vector2(rb.velocity.x, rb.velocity.y);
	// 	Vector2 momentaryForce = difference * acceleration;
	// 	Debug.Log(momentaryForce);
	// 	rb.AddForce(momentaryForce);
	// }
}
