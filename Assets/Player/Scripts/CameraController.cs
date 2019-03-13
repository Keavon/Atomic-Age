using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameObject player;

	void Start () {
	}
	
	void Update () {
		Vector3 playerPosition = player.transform.position;

		Vector3 cameraPosition = transform.position;
		cameraPosition.x = playerPosition.x;
		cameraPosition.y = playerPosition.y + 2;
		transform.position = cameraPosition;
	}
}
