using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class AnimationAssignments : MonoBehaviour {
	[System.Serializable]
	public struct AnimationCycle {
		public Vector2 alignment;
		public AnimatorController noMop;
		public AnimatorController cleanMop;
		public AnimatorController oilMop;
		public AnimatorController glueMop;
		public AnimatorController ferrofluidMop;
	}

	public AnimationCycle standing;
	public AnimationCycle walking;
	public AnimationCycle falling;
	public AnimationCycle mopping;
	public AnimationCycle dragging;
	public AnimationCycle climbing;
	public AnimationCycle grabbingButton;
	public AnimationCycle grabbingLever;

	public void CopyValueAssignmentsToAnotherObject(GameObject source) {
		AnimationAssignments controller = source.GetComponent<AnimationAssignments>();
		controller.standing = standing;
		controller.walking = walking;
		controller.falling = falling;
		controller.mopping = mopping;
		controller.dragging = dragging;
		controller.climbing = climbing;
		controller.grabbingButton = grabbingButton;
		controller.grabbingLever = grabbingLever;
	}
}
