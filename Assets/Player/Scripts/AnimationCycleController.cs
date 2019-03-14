using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class AnimationCycleController : MonoBehaviour {
	public GameObject playerGameObject;

	PlayerController playerController;
	AnimationAssignments animationAssignments;
	PlayerAnimationCycle activeCycle = PlayerAnimationCycle.Standing;
	GameObject animationChild;
	Animator animator;
	bool facingRight = true;
	MopState lastMopState = MopState.NoMop;

	public void Awake() {
		animationAssignments = GetComponent<AnimationAssignments>();
		playerController = playerGameObject.GetComponent<PlayerController>();

		animationChild = new GameObject("Animation Cycle");
		animationChild.transform.SetParent(transform);
		animationChild.AddComponent<SpriteRenderer>();
		animator = animationChild.AddComponent<Animator>();
	}

	public void Update() {
		transform.position = playerGameObject.transform.position;
		if (lastMopState != playerController.mopState) {
			lastMopState = playerController.mopState;
			ChangeToAnimation();
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

	public void SetActiveCycle(PlayerAnimationCycle cycle) {
		activeCycle = cycle;
		ChangeToAnimation();
		SetPlaybackSpeed(1);
	}

	void ChangeToAnimation() {
		AnimationAssignments.AnimationCycle currentAssignment = animationAssignments.standing;
		if (activeCycle == PlayerAnimationCycle.Standing) currentAssignment = animationAssignments.standing;
		else if (activeCycle == PlayerAnimationCycle.Walking) currentAssignment = animationAssignments.walking;
		else if (activeCycle == PlayerAnimationCycle.Falling) currentAssignment = animationAssignments.falling;
		else if (activeCycle == PlayerAnimationCycle.Mopping) currentAssignment = animationAssignments.mopping;
		else if (activeCycle == PlayerAnimationCycle.Dragging) currentAssignment = animationAssignments.dragging;
		else if (activeCycle == PlayerAnimationCycle.Climbing) currentAssignment = animationAssignments.climbing;
		else if (activeCycle == PlayerAnimationCycle.GrabbingButton) currentAssignment = animationAssignments.grabbingButton;
		else if (activeCycle == PlayerAnimationCycle.GrabbingLever) currentAssignment = animationAssignments.grabbingLever;

		AnimatorController currentController = currentAssignment.noMop;
		if (playerController.mopState == MopState.NoMop) currentController = currentAssignment.noMop;
		if (playerController.mopState == MopState.CleanMop) currentController = currentAssignment.cleanMop;
		if (playerController.mopState == MopState.OilMop) currentController = currentAssignment.oilMop;
		if (playerController.mopState == MopState.GlueMop) currentController = currentAssignment.glueMop;
		if (playerController.mopState == MopState.FerrofluidMop) currentController = currentAssignment.ferrofluidMop;

		Vector2 currentAlignment = currentAssignment.alignment;

		// Fallbacks
		if (currentController == null) {
			currentController = currentAssignment.noMop;
		}
		if (currentController == null) {
			currentController = animationAssignments.standing.noMop;
			currentAlignment = animationAssignments.standing.alignment;
		}
		if (currentController == null) {
			Debug.Log("No animation cycle assigned to the player.");
		}
		
		animator.runtimeAnimatorController = currentController as RuntimeAnimatorController;
		animationChild.transform.localPosition = currentAlignment;
	}

	public void SetPlaybackSpeed(float speed) {
		// Checking if it's 1 is sort of a hack, it would be better to check if "mirror" exists
		if (animator.parameterCount == 1) {
			if (speed < 0) animator.SetBool("mirror", true);
			if (speed > 0) animator.SetBool("mirror", false);
			animator.speed = Mathf.Abs(speed);
		}
	}
}
