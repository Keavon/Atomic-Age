using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class AnimationCycleController : MonoBehaviour {
	public GameObject playerGameObject;

	AnimationAssignments animationAssignments;
	PlayerAnimationCycle activeCycle = PlayerAnimationCycle.Standing;
	GameObject animationChild;
	Animator animator;
	bool facingRight = true;
	PlayerMopState mopState = PlayerMopState.NoMop;

	public void Awake() {
		animationAssignments = GetComponent<AnimationAssignments>();

		animationChild = new GameObject("Animation Cycle");
		animationChild.transform.SetParent(transform);
		animationChild.AddComponent<SpriteRenderer>();
		animator = animationChild.AddComponent<Animator>();
	}

	public void Update() {
		transform.position = playerGameObject.transform.position;
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
		switch (cycle) {
			case PlayerAnimationCycle.Standing: ChangeToAnimation(animationAssignments.standing); break;
			case PlayerAnimationCycle.Walking: ChangeToAnimation(animationAssignments.walking); break;
			case PlayerAnimationCycle.Falling: ChangeToAnimation(animationAssignments.falling); break;
			case PlayerAnimationCycle.Mopping: ChangeToAnimation(animationAssignments.mopping); break;
			case PlayerAnimationCycle.Dragging: ChangeToAnimation(animationAssignments.dragging); break;
			case PlayerAnimationCycle.Climbing: ChangeToAnimation(animationAssignments.climbing); break;
			case PlayerAnimationCycle.GrabbingButton: ChangeToAnimation(animationAssignments.grabbingButton); break;
			case PlayerAnimationCycle.GrabbingLever: ChangeToAnimation(animationAssignments.grabbingLever); break;
		}
	}

	void ChangeToAnimation(AnimationAssignments.AnimationCycle cycle) {
		AnimatorController controller = animationAssignments.standing.noMop;
		Vector2 alignment = cycle.alignment;

		switch (mopState) {
			case PlayerMopState.NoMop: controller = cycle.noMop != null ? cycle.noMop : animationAssignments.standing.noMop; break;
			case PlayerMopState.CleanMop: controller = cycle.cleanMop != null ? cycle.cleanMop : animationAssignments.standing.cleanMop; break;
			case PlayerMopState.OilMop: controller = cycle.oilMop != null ? cycle.oilMop : animationAssignments.standing.oilMop; break;
			case PlayerMopState.GlueMop: controller = cycle.glueMop != null ? cycle.glueMop : animationAssignments.standing.glueMop; break;
			case PlayerMopState.FerrofluidMop: controller = cycle.ferrofluidMop != null ? cycle.ferrofluidMop : animationAssignments.standing.ferrofluidMop; break;
		}
		animator.runtimeAnimatorController = controller as RuntimeAnimatorController;
		animationChild.transform.localPosition = cycle.alignment;
	}
}
