using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public GameObject liquidPrefab;
	public MopState mopState = MopState.NoMop;
	public float walkSpeed = 10.0f;
	public float airStrafeSpeed = 3.0f;
	public float dragSpeed = 1.0f;
	public float climbSpeed = 1.0f;
	public float jumpVelocity = 7.5f;
	public Vector2 windImpulse = Vector2.zero;

	List<GameObject> touchingGrounds = new List<GameObject>();
	List<GameObject> touchingDraggablesDisqualified = new List<GameObject>();
	List<GameObject> touchingDraggables = new List<GameObject>();
	List<GameObject> touchingGrabbables = new List<GameObject>();
	List<GameObject> touchingClimbables = new List<GameObject>();
	List<GameObject> liquidsPlaced = new List<GameObject>();

	Rigidbody2D playerRigidbody;
	GameObject activeInteractionTarget;
	GameObject activeClimbTarget;
	GameObject currentMoppingTarget;
	Animator stateMachine;
	AnimationCycleController animationController;
	GameObject playerVisuals;
	Vector2 deltaV;
	Rigidbody2D rigidbodySupportingPlayer;
	Vector2 inputMotion;
	float moppingTime = 0f;
	bool climbingTemporarilyProhibited = false;
	SpringJoint2D springJoint;

	void Start() {
		playerRigidbody = GetComponent<Rigidbody2D>();
		stateMachine = GetComponent<Animator>();
		springJoint = GetComponent<SpringJoint2D>();

		playerVisuals = new GameObject("Player [VISUALS]");
		playerVisuals.SetActive(false);
		playerVisuals.AddComponent<AnimationAssignments>();
		animationController = playerVisuals.AddComponent<AnimationCycleController>();
		animationController.playerGameObject = gameObject;
		playerVisuals.SetActive(true);
		GetComponent<AnimationAssignments>().CopyValueAssignmentsToAnotherObject(playerVisuals);
	}

	void Update() {
		// Save the latest input direction motion
		inputMotion = InputManager.motion();

		// Set input states for player
		stateMachine.SetFloat("Input_XMotion", inputMotion.x);
		stateMachine.SetFloat("Input_YMotion", inputMotion.y);
		stateMachine.SetBool("Input_JumpPress", InputManager.jumpPress());
		stateMachine.SetBool("Input_JumpRelease", InputManager.jumpRelease());
		stateMachine.SetBool("Input_InteractPress", InputManager.interactPress());
		stateMachine.SetBool("Input_InteractRelease", InputManager.interactRelease());
		stateMachine.SetBool("Input_MopPress", InputManager.mopPress());
		stateMachine.SetBool("Input_MopRelease", InputManager.mopRelease());

		// Set input states for single-use buttons that are used on demand and then consumed
		if (InputManager.jumpPress()) stateMachine.SetTrigger("Input_JumpPressWait");
		if (InputManager.jumpRelease()) stateMachine.ResetTrigger("Input_JumpPressWait");
		if (InputManager.interactPress()) stateMachine.SetTrigger("Input_InteractPressWait");
		if (InputManager.interactRelease()) stateMachine.ResetTrigger("Input_InteractPressWait");
		if (InputManager.mopPress()) stateMachine.SetTrigger("Input_MopPressWait");
		if (InputManager.mopRelease()) stateMachine.ResetTrigger("Input_MopPressWait");

		// Set touching trigger states for player
		stateMachine.SetBool("Touching_Ground", touchingGrounds.Count > 0);
		stateMachine.SetBool("Touching_Climbable", touchingClimbables.Count > 0);
		stateMachine.SetBool("Touching_Grabbable", touchingGrabbables.Count > 0);
		stateMachine.SetBool("Touching_Draggable", GetEligibleDraggables().Count > 0);
	}

	void FixedUpdate() {
		stateMachine.Update(Time.fixedDeltaTime);
		// Reset the change in player velocity sum
		deltaV = Vector2.zero;

		// Reenable gravity in case it was disabled for climbing
		playerRigidbody.gravityScale = 1;

		// Perform the functionality of the active player state
		bool didNothing = true;

		if (stateMachine.GetCurrentAnimatorStateInfo(0).IsName("Climbing") && !climbingTemporarilyProhibited) { PerformClimbing(); didNothing = false; }

		if (stateMachine.GetCurrentAnimatorStateInfo(0).IsName("Walking")) { PerformWalking(); didNothing = false; }

		if (stateMachine.GetCurrentAnimatorStateInfo(0).IsName("Falling")) { PerformFalling(); didNothing = false; }

		if (stateMachine.GetCurrentAnimatorStateInfo(0).IsName("Grabbing")) { PerformGrabbing(); didNothing = false; }

		if (stateMachine.GetCurrentAnimatorStateInfo(0).IsName("Dragging")) { PerformDragging(); didNothing = false; }

		if (stateMachine.GetCurrentAnimatorStateInfo(0).IsName("Mopping")) { PerformMopping(); didNothing = false; }
		else { moppingTime = 0; }


		if (didNothing) { deltaV = new Vector2(-playerRigidbody.velocity.x, 0); }

		deltaV += windImpulse;

		// Apply the change in player velocity as an impulse (force * deltaT = impulse = mass * deltaV)
		playerRigidbody.AddForce(deltaV * playerRigidbody.mass, ForceMode2D.Impulse);
	}

	void PerformWalking() {
		// Ensure there is no active interaction target
		activeInteractionTarget = null;

		// Jump
		if (stateMachine.GetBool("Input_JumpPressWait")) {
			JumpMotion();
		}

		// Horizontal walk movement
		if (inputMotion.x <= -0.5 || inputMotion.x >= 0.5) {
			WalkMotion();

			animationController.SetActiveCycle(PlayerAnimationCycle.Walking);
			if (inputMotion.x > 0) animationController.SetFacingRight();
			else animationController.SetFacingLeft();
		}
		else {
			animationController.SetActiveCycle(PlayerAnimationCycle.Standing);
		}
	}

	void PerformFalling() {
		// Horizontal air strafe movement
		FallMotion();

		animationController.SetActiveCycle(PlayerAnimationCycle.Falling);
		if (inputMotion.x >= 0.1) animationController.SetFacingRight();
		else if (inputMotion.x <= -0.1) animationController.SetFacingLeft();
	}

	void PerformMopping() {
		if (moppingTime == 0) {
			Debug.Log("Mopping");
			animationController.SetActiveCycle(PlayerAnimationCycle.Mopping);
			
			bool playerTouchingMoppableSurface = touchingGrounds.Any(ground => ground.layer == 0);
			bool playerHasLiquidOnMop = mopState != MopState.CleanMop && mopState != MopState.NoMop;

			// Check if we have a liquid we can place
			if (playerTouchingMoppableSurface && playerHasLiquidOnMop) {
				Fluid fluidToPlace;
				if (mopState == MopState.OilMop) fluidToPlace = Fluid.Oil;
				else if (mopState == MopState.GlueMop) fluidToPlace = Fluid.Glue;
				else fluidToPlace = Fluid.Ferro;

				PlaceLiquid(transform.position, fluidToPlace);
				mopState = MopState.CleanMop;
			}
			// If we don't have a liquid, let's see if we can pick any up
			else {
				CheckPickupLiquid(transform.position);
			}
		}

		moppingTime += Time.fixedDeltaTime;

		if (moppingTime > 1) stateMachine.SetBool("Trigger_BreakMopping", true);
	}

	void PerformClimbing() {
		// Jump off ladder
		if (stateMachine.GetBool("Input_JumpPressWait")) {
			climbingTemporarilyProhibited = true;
			JumpMotion();
			return;
		}

		// Disable gravity
		playerRigidbody.gravityScale = 0;

		// Ensure there is a climb target
		SelectClimbTarget();
		if (!activeClimbTarget) return;

		// Face direction
		if (inputMotion.x < -0.5) animationController.SetFacingLeft();
		else if (inputMotion.x > 0.5) animationController.SetFacingRight();
		animationController.SetActiveCycle(PlayerAnimationCycle.Climbing);

		// Allow walking near base of ladder
		Bounds climbableBounds = activeClimbTarget.GetComponent<Collider2D>().bounds;

		// Climb
		float verticalVelocity = ClimbMotion(climbableBounds);
		animationController.SetPlaybackSpeed(verticalVelocity);
	}

	void PerformGrabbing() {
		// Ensure there is a drag target
		SelectInteractionTarget();

		// Redirect motion into grab target
		SendForceToInteractionTarget(inputMotion);

		animationController.SetActiveCycle(PlayerAnimationCycle.GrabbingLever);
	}

	void PerformDragging() {
		// Ensure there is a drag target
		SelectInteractionTarget();
		if (!activeInteractionTarget) return;
		Debug.Log(activeInteractionTarget);

		Rigidbody2D targetRigidbody = activeInteractionTarget.GetComponent<Rigidbody2D>();
		springJoint.enabled = true;
		springJoint.connectedBody = targetRigidbody;


		// // Walk slowly with box
		// WalkMotion(dragSpeed);

		// Send that slow motion into drag target
		// SendMotionToInteractionTarget(deltaV);

		// Update animation

		PerformWalking();
		
		if (playerRigidbody.position.x > targetRigidbody.position.x) animationController.SetFacingLeft();
		else animationController.SetFacingRight();
		animationController.SetActiveCycle(PlayerAnimationCycle.Dragging);
	}

	void WalkMotion(float speed = -1) {
		if (speed == -1) speed = walkSpeed;
		
		Vector2 targetVelocity = inputMotion * speed;
		Vector2 relativeTargetVelocity = targetVelocity;
		if (rigidbodySupportingPlayer) {
			relativeTargetVelocity += rigidbodySupportingPlayer.velocity;
		}

		deltaV += new Vector2(relativeTargetVelocity.x, 0) - playerRigidbody.velocity;
	}

	void JumpMotion() {
		stateMachine.ResetTrigger("Input_JumpPressWait");
		deltaV += Vector2.up * jumpVelocity;

		activeClimbTarget = null;
		stateMachine.SetBool("Trigger_BreakClimbing", true);
	}

	void FallMotion() {
		Vector2 targetVelocity = inputMotion * airStrafeSpeed;
		deltaV.x += targetVelocity.x - playerRigidbody.velocity.x;
	}

	float ClimbMotion(Bounds climbableBounds) {
		float thresholdHeight = 0.5f;

		float distanceAboveBottom = playerRigidbody.position.y - climbableBounds.min.y;
		float distanceBelowTop = climbableBounds.max.y - playerRigidbody.position.y - 1.75f;
		float distanceOffCenter = climbableBounds.center.x - playerRigidbody.position.x;

		Vector2 targetVelocity = new Vector2(distanceOffCenter * climbSpeed, inputMotion.y * climbSpeed);
		if (distanceAboveBottom <= 0 && targetVelocity.y < 0) targetVelocity.y = 0;
		if (distanceBelowTop <= 0 && targetVelocity.y > 0) targetVelocity.y = 0;

		Vector2 climbDeltaV = targetVelocity - playerRigidbody.velocity;
		if (distanceAboveBottom < thresholdHeight) {
			WalkMotion();
			climbDeltaV.x = deltaV.x;
		}
		
		deltaV = climbDeltaV;

		return targetVelocity.y;
	}

	void SendMotionToInteractionTarget(Vector2 motion) {
		// Send motion to grab target
		if (activeInteractionTarget != null) {
			activeInteractionTarget.gameObject.GetComponent<IDraggable>().Move(motion.x);
		}
	}

	void SendForceToInteractionTarget(Vector2 motion) {
		// Send motion to grab target
		if (activeInteractionTarget != null) {
			activeInteractionTarget.gameObject.GetComponent<IGrabbable>().Force(motion);
		}
	}

	public void SelectInteractionTarget() {
		if (touchingGrabbables.Count > 0) {
			activeInteractionTarget = touchingGrabbables[0];
			return;
		}

		List<GameObject> eligibleDraggables = GetEligibleDraggables();
		if (eligibleDraggables.Count > 0) {
			activeInteractionTarget = eligibleDraggables[0];
			return;
		}

		activeInteractionTarget = null;
	}

	public void SelectClimbTarget() {
		if (touchingClimbables.Count > 0) {
			activeClimbTarget = touchingClimbables[0];
			return;
		}

		activeClimbTarget = null;
	}

	List<GameObject> GetEligibleDraggables() {
		IEnumerable<GameObject> eligible = touchingDraggables.Where(item => !touchingDraggablesDisqualified.Contains(item));
		return eligible.ToList();
	}

	void OnTriggerEnter2D(Collider2D other) {
		// Increment the total objects touched by the player's feet
		if (!other.isTrigger) {
			touchingGrounds.Add(other.gameObject);

			if (!touchingDraggablesDisqualified.Contains(other.gameObject)) {
				touchingDraggablesDisqualified.Add(other.gameObject);
			}
		}

		Rigidbody2D rb = other.gameObject.GetComponentInParent<Rigidbody2D>();
		if (rb) rigidbodySupportingPlayer = rb;
	}

	void OnTriggerExit2D(Collider2D other) {
		// Decrement the total objects touched by the player's feet
		if (!other.isTrigger) {
			touchingGrounds.Remove(other.gameObject);

			if (touchingDraggablesDisqualified.Contains(other.gameObject)) {
				touchingDraggablesDisqualified.Remove(other.gameObject);
			}
		}

		if (rigidbodySupportingPlayer && other.gameObject == rigidbodySupportingPlayer.gameObject) {
			rigidbodySupportingPlayer = null;
		}
	}

	public void EnteredDraggable(GameObject draggable) {
		if (!touchingDraggables.Contains(draggable)) touchingDraggables.Add(draggable);
	}

	public void ExitedDraggable(GameObject draggable) {
		if (touchingDraggables.Contains(draggable)) touchingDraggables.Remove(draggable);
	}

	public void EnteredGrabbable(GameObject grabbable) {
		if (!touchingGrabbables.Contains(grabbable)) touchingGrabbables.Add(grabbable);
	}

	public void ExitedGrabbable(GameObject grabbable) {
		if (touchingGrabbables.Contains(grabbable)) touchingGrabbables.Remove(grabbable);
	}

	public void EnteredClimbable(GameObject climbable) {
		if (!touchingClimbables.Contains(climbable)) touchingClimbables.Add(climbable);
	}

	public void ExitedClimbable(GameObject climbable) {
		if (touchingClimbables.Contains(climbable)) touchingClimbables.Remove(climbable);
		climbingTemporarilyProhibited = false;
	}

	private void PlaceLiquid(Vector3 position, Fluid fluid) {
		GameObject liquid = Instantiate(liquidPrefab, new Vector3(0, 0, -1), Quaternion.identity);
		liquid.GetComponent<LiquidBehavior>().PlaceLiquid(position, fluid, true);
		currentMoppingTarget = liquid;
		liquidsPlaced.Add(liquid);
	}

	private void CheckPickupLiquid(Vector3 position) {
		Vector2 hitSize = new Vector2(1, 0.5f);
		float distance = 1;
		LayerMask moppableLayer = LayerMask.GetMask("Liquids");

		Vector2 origin = new Vector2(position.x, position.y + 0.6f);

		RaycastHit2D hit = Physics2D.BoxCast(origin, hitSize, 0, Vector2.down, distance, moppableLayer);

		// We found some lqiuid, so set our mop liquid
		if (hit.collider != null) {
			LiquidBehavior liquidBehavior = hit.collider.GetComponent<LiquidBehavior>();

			if (hit.collider.CompareTag("Oil")) mopState = MopState.OilMop;
			else if (hit.collider.CompareTag("Glue")) mopState = MopState.GlueMop;
			else if (hit.collider.CompareTag("Ferrofluid")) mopState = MopState.FerrofluidMop;
			
			if (liquidBehavior) {
				liquidBehavior.RemoveLiquid();
			} else {
				Destroy(hit.collider.gameObject);
			}
		}
	}
}
