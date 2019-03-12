using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableStretcher : MonoBehaviour {
	public GameObject top;
	public float topOffset = 0;
	public GameObject bottom;
	public float bottomOffset = 0;

	Renderer thisRenderer;
	Vector2 lastTop;
	Vector2 lastBottom;

	void Awake() {
		thisRenderer = GetComponent<Renderer>();
		Vector3 scale = transform.localScale;
		scale.y = 1;
		transform.localScale = scale;
	}

	void Update() {
		Vector2 topPosition = top.transform.position;
		Vector2 bottomPosition = bottom.transform.position;

		if (topPosition != lastTop || bottomPosition != lastBottom) {
			Bounds bounds = thisRenderer.bounds;
			float max = bounds.max.y;
			float min = bounds.min.y;
			float height = max - min;
			float origin = transform.position.y;

			float targetTop = top.transform.position.y + topOffset;
			float targetBottom = bottom.transform.position.y + bottomOffset;
			float targetHeight = targetTop - targetBottom;

			Vector3 scale = transform.localScale;
			float naturalHeight = height / scale.y;
			scale.y = targetHeight / naturalHeight;
			transform.localScale = scale;

			Vector3 position = transform.position;
			float originPercentFromBottom = (origin - min) / height;
			position.y = targetBottom + targetHeight * originPercentFromBottom;
			transform.position = position;
		}
	}
}
