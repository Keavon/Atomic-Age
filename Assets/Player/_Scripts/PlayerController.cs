using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float accelerationToTargetSpeed = 25.0f;
	public float walkSpeed = 10.0f;
	public float airStrafeSpeed = 3.0f;
	public float dragSpeed = 1.0f;
	public float climbSpeed = 1.0f;
	public float jumpVelocity = 7.5f;

    public GameObject liquidPrefab;

	Rigidbody2D playerRigidbody;
	GameObject activeInteractionTarget;
    GameObject activeClimbTarget;
	GameObject currentMoppingTarget;

	List<GameObject> touchingGrounds = new List<GameObject>();
	List<GameObject> touchingDraggablesDisqualified = new List<GameObject>();
	List<GameObject> touchingDraggables = new List<GameObject>();
	List<GameObject> touchingGrabbables = new List<GameObject>();
    List<GameObject> touchingClimbables = new List<GameObject>();
	List<GameObject> liquidsPlaced = new List<GameObject>();

	Animator stateMachine;

    private Fluid currentMoppingLiquid = Fluid.None;

	void Start() {
		stateMachine = GetComponent<Animator>();
		playerRigidbody = GetComponent<Rigidbody2D>();
	}

	void Update() {
		// Set input states for player
		stateMachine.SetFloat("Input_XMotion", InputManager.motion().x);
		stateMachine.SetFloat("Input_YMotion", InputManager.motion().y);
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
        int total = GetEligibleDraggables().Count;
        stateMachine.SetBool("Touching_Draggable", total > 0);

        if (stateMachine.GetCurrentAnimatorStateInfo(0).IsName("Mopping")) PerformMopping();
        if (stateMachine.GetCurrentAnimatorStateInfo(0).IsName("Climbing")) PerformClimbing();
        if (stateMachine.GetBool("Input_MopPress")) StartMopping();
    }

    void FixedUpdate() {


        // Perform the functionality of the active player state
        if (stateMachine.GetCurrentAnimatorStateInfo(0).IsName("Walking")) PerformWalking();
        if (stateMachine.GetCurrentAnimatorStateInfo(0).IsName("Falling")) PerformFalling();
        if (stateMachine.GetCurrentAnimatorStateInfo(0).IsName("Grabbing")) PerformGrabbing();
        if (stateMachine.GetCurrentAnimatorStateInfo(0).IsName("Dragging")) PerformDragging();

    }

    void PushPlayerAtSpeedHorizontally(float speed, float acceleration) {
		float difference = speed - playerRigidbody.velocity.x;
		float momentaryForce = difference * acceleration;
		Vector2 movementForce = new Vector2(momentaryForce, 0);
		playerRigidbody.AddForce(movementForce);
	}

	void PushPlayerAtSpeedVertically(float speed, float acceleration) {
		float difference = speed - playerRigidbody.velocity.y;
		float momentaryForce = difference * acceleration;
		Vector2 movementForce = new Vector2(0, momentaryForce);
		playerRigidbody.AddForce(movementForce);
	}

	void PushPlayerAtSpeed(float xSpeed, float ySpeed, float acceleration) {
		PushPlayerAtSpeedHorizontally(xSpeed, acceleration);
		PushPlayerAtSpeedVertically(ySpeed, acceleration);
	}

	void PushPlayerToPlaceHorizontally(float xCoordinate, float speed, float acceleration) {
		float difference = xCoordinate - transform.position.x;
		float pushSpeed = difference * speed;
		PushPlayerAtSpeedHorizontally(pushSpeed, acceleration);
	}

	void PushPlayerToPlaceVertically(float yCoordinate, float speed, float acceleration) {
		float difference = yCoordinate - transform.position.y;
		float pushSpeed = difference * speed;
		PushPlayerAtSpeedVertically(pushSpeed, acceleration);
	}

	void PushPlayerToPlace(float xCoordinate, float yCoordinate, float speed, float acceleration) {
		PushPlayerToPlaceHorizontally(xCoordinate, speed, acceleration);
		PushPlayerToPlaceVertically(yCoordinate, speed, acceleration);
	}

    void StartMopping(){
        if (touchingGrounds.Any((ground) => ground.layer == 0)) {

            // We have a liquid we can place.
            if (currentMoppingLiquid != Fluid.None) {
                PlaceLiquid(transform.position, currentMoppingLiquid);
                currentMoppingLiquid = Fluid.None;
            }
            // We don't have a liquid, so lets see if we can pick any up
            else {
                CheckPickupLiquid(transform.position);
            }
        } else
        {
            CheckPickupLiquid(transform.position);
        }
    }

	void PerformWalking() {
		Vector2 motion = InputManager.motion();

		// Ensure there is no active interaction target
		activeInteractionTarget = null;

		// Ensure gravity acts upon player
		playerRigidbody.gravityScale = 1;

		SelectClimbTarget();
		bool tryingToClimbNotJump = activeClimbTarget != null && motion.y > 0.5f;

		// Jump
		if (!tryingToClimbNotJump && stateMachine.GetBool("Input_JumpPressWait")) {
			stateMachine.ResetTrigger("Input_JumpPressWait");
			
			Vector2 velocity = playerRigidbody.velocity;
			velocity.y = jumpVelocity;
			playerRigidbody.velocity = velocity;
		}

		// Horizontal walk movement
		if (motion.x <= -0.5 || motion.x >= 0.5) {
			float targetSpeed = walkSpeed * motion.x;
			PushPlayerAtSpeedHorizontally(targetSpeed, accelerationToTargetSpeed);
		}
	}

	void PerformFalling() {
		// Ensure gravity acts upon player
		playerRigidbody.gravityScale = 1;

		// Horizontal air strafe movement
		float targetSpeed = airStrafeSpeed * InputManager.motion().x;
		PushPlayerAtSpeedHorizontally(targetSpeed, accelerationToTargetSpeed);
	}

	void PerformMopping() {
        Vector3 moppingLocation = transform.position;

        // Read player motion input
        Vector2 motion = InputManager.motion();

        // Read player velocity
        Vector2 velocity = playerRigidbody.velocity;
        velocity.x = motion.x * walkSpeed;

        // Write player velocity
        playerRigidbody.velocity = velocity;
    }

	void PerformClimbing() {
        if (stateMachine.GetBool("Input_JumpPress")){
            activeClimbTarget = null;
            stateMachine.SetBool("Trigger_BreakClimbing", true);
            return;
        }

		// Ensure there is a climb target
		SelectClimbTarget();
		if (activeClimbTarget == null) return;

		// Stop gravity acting on player
		playerRigidbody.gravityScale = 0;

		// Read player motion input
		Vector2 motion = InputManager.motion();

		// Vertical climbing motion
		Bounds climbableBounds = activeClimbTarget.GetComponent<Collider2D>().bounds;
		float distanceAboveBottom = gameObject.transform.position.y - climbableBounds.min.y;
		float distanceBelowTop = climbableBounds.max.y - gameObject.transform.position.y - 1.0f;

		// Leaning and centering while not grounded
		if (touchingGrounds.Count == 0) {
			if (motion.x < -0.5) Debug.Log("Lean left");
			else if (motion.x > 0.5) Debug.Log("Lean right");
			PushPlayerToPlaceHorizontally(climbableBounds.center.x, 10, 20);
		}

		float targetSpeed = motion.y * climbSpeed;
		if (targetSpeed < 0 && distanceAboveBottom > 0) {
			PushPlayerAtSpeedVertically(targetSpeed, accelerationToTargetSpeed);
		}
		else if (targetSpeed > 0 && distanceBelowTop > 0) {
			PushPlayerAtSpeedVertically(targetSpeed, accelerationToTargetSpeed);
		}
		else {
			PushPlayerAtSpeedVertically(0, accelerationToTargetSpeed);
		}
	}

	void PerformGrabbing() {
		// Ensure there is a drag target
		SelectInteractionTarget();

		// Halt player motion
		playerRigidbody.velocity = Vector2.zero;

		// Read player motion input
		Vector2 motion = InputManager.motion();

		// Redirect motion into grab target
		SendForceToInteractionTarget(motion);
	}

	void PerformDragging() {
		// Ensure there is a drag target
		SelectInteractionTarget();

		// Read player motion input
		Vector2 motion = InputManager.motion();

		// Read player velocity
		Vector2 velocity = playerRigidbody.velocity;
		velocity.x = motion.x * dragSpeed;

		// Write player velocity
		playerRigidbody.velocity = velocity;

		// Redirect motion into drag target
		SendMotionToInteractionTarget(velocity);
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
	}

	void OnTriggerExit2D(Collider2D other) {
		// Decrement the total objects touched by the player's feet
		if (!other.isTrigger) {
			touchingGrounds.Remove(other.gameObject);

			if (touchingDraggablesDisqualified.Contains(other.gameObject)) {
				touchingDraggablesDisqualified.Remove(other.gameObject);
			}
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
	}

    private void PlaceLiquid(Vector3 position, Fluid fluid) {
        GameObject liquid = Instantiate(liquidPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        liquid.GetComponent<LiquidBehavior>().PlaceLiquid(position, fluid, 1.5f, 1.5f);
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

            if (hit.collider.CompareTag("Oil")) {
                currentMoppingLiquid = Fluid.Oil;
            }

            else if(hit.collider.CompareTag("Glue")) {
                currentMoppingLiquid = Fluid.Glue;
            }

            else if(hit.collider.CompareTag("Ferrofluid")) {
                currentMoppingLiquid = Fluid.Ferro;
            }
            else {

            }
            if (liquidBehavior) {
                liquidBehavior.RemoveLiquid();
            }
            else {
                Destroy(hit.collider.gameObject);

            }
        }
    }

}
