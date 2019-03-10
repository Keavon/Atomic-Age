using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCycleController : MonoBehaviour {
	public GameObject standing;
	public GameObject walking;
	public GameObject falling;
	public GameObject mopping;
	public GameObject dragging;
	public GameObject climbing;
	public GameObject grabbingButton;
	public GameObject grabbingLever;

	PlayerAnimationCycle activeCycle = PlayerAnimationCycle.Standing;
	bool facingRight = true;

	public void SetActiveCycle(PlayerAnimationCycle cycle) {
		if (activeCycle != cycle) {
			SetCycleState(activeCycle, false);
			activeCycle = cycle;
			SetCycleState(activeCycle, true);
		}
	}

	public void SetFacingLeft() {
		facingRight = false;

		Vector3 scale = transform.localScale;
		scale.x = -Mathf.Abs(scale.x);
		transform.localScale = scale;
	}

	public void SetFacingRight() {
		facingRight = true;

		Vector3 scale = transform.localScale;
		scale.x = Mathf.Abs(scale.x);
		transform.localScale = scale;
	}

	void SetCycleState(PlayerAnimationCycle cycle, bool state) {
		switch (cycle) {
			case PlayerAnimationCycle.Standing: standing.SetActive(state); break;
			case PlayerAnimationCycle.Walking: walking.SetActive(state); break;
			case PlayerAnimationCycle.Falling: falling.SetActive(state); break;
			case PlayerAnimationCycle.Mopping: mopping.SetActive(state); break;
			case PlayerAnimationCycle.Dragging: dragging.SetActive(state); break;
			case PlayerAnimationCycle.Climbing: climbing.SetActive(state); break;
			case PlayerAnimationCycle.GrabbingButton: grabbingButton.SetActive(state); break;
			case PlayerAnimationCycle.GrabbingLever: grabbingLever.SetActive(state); break;
		}
	}
}
