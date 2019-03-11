using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
	// Left stick motion
	public static Vector2 motion() {
		float left = (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) ? -1 : 0;
		float right = (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) ? 1 : 0;
		float horizontal = Input.GetAxisRaw("Horizontal");
		float x = Mathf.Clamp(left + right + horizontal, -1, 1);

		float up = (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) ? 1 : 0;
		float down = (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) ? -1 : 0;
		float vertical = Input.GetAxisRaw("Vertical");
		float y = Mathf.Clamp(up + down + vertical, -1, 1);
		Vector2 vector = new Vector2(x, y);
		
		if (vector.magnitude > 1) return vector.normalized;
		return new Vector3(x, y);
	}

	// Jump button presses
	public static bool jumpPress() {
		bool keyboard = Input.GetKeyDown(KeyCode.Space);
		bool winGamepad = Input.GetKeyDown(KeyCode.Joystick1Button0);
		bool macGamepad = Input.GetKeyDown(KeyCode.Joystick1Button16);
		return keyboard || winGamepad || macGamepad;
	}

	public static bool jumpPressed() {
		bool keyboard = Input.GetKey(KeyCode.Space);
		bool winGamepad = Input.GetKey(KeyCode.Joystick1Button0);
		bool macGamepad = Input.GetKey(KeyCode.Joystick1Button16);
		return keyboard || winGamepad || macGamepad;
	}

	public static bool jumpRelease() {
		bool keyboard = Input.GetKeyUp(KeyCode.Space);
		bool winGamepad = Input.GetKeyUp(KeyCode.Joystick1Button0);
		bool macGamepad = Input.GetKeyUp(KeyCode.Joystick1Button16);
		return keyboard || winGamepad || macGamepad;
	}

	// Interact button presses
	public static bool interactPress() {
		bool keyboard = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.E);
		bool winGamepad = Input.GetKeyDown(KeyCode.Joystick1Button1);
		bool macGamepad = Input.GetKeyDown(KeyCode.Joystick1Button17);
		return keyboard || winGamepad || macGamepad;
	}

	public static bool interactPressed() {
		bool keyboard = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.E);
		bool winGamepad = Input.GetKey(KeyCode.Joystick1Button1);
		bool macGamepad = Input.GetKey(KeyCode.Joystick1Button17);
		return keyboard || winGamepad || macGamepad;
	}

	public static bool interactRelease() {
		bool keyboard = Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift) || Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl) || Input.GetKeyUp(KeyCode.E);
		bool winGamepad = Input.GetKeyUp(KeyCode.Joystick1Button1);
		bool macGamepad = Input.GetKeyUp(KeyCode.Joystick1Button17);
		return keyboard || winGamepad || macGamepad;
	}

	// Mop button presses
	public static bool mopPress() {
		bool keyboard = Input.GetKeyDown(KeyCode.Tab);
		bool winGamepad = Input.GetKeyDown(KeyCode.Joystick1Button2);
		bool macGamepad = Input.GetKeyDown(KeyCode.Joystick1Button18);
		return keyboard || winGamepad || macGamepad;
	}

	public static bool mopPressed() {
		bool keyboard = Input.GetKey(KeyCode.Tab);
		bool winGamepad = Input.GetKey(KeyCode.Joystick1Button2);
		bool macGamepad = Input.GetKey(KeyCode.Joystick1Button18);
		return keyboard || winGamepad || macGamepad;
	}

	public static bool mopRelease() {
		bool keyboard = Input.GetKeyUp(KeyCode.Tab);
		bool winGamepad = Input.GetKeyUp(KeyCode.Joystick1Button2);
		bool macGamepad = Input.GetKeyUp(KeyCode.Joystick1Button18);
		return keyboard || winGamepad || macGamepad;
	}
}
