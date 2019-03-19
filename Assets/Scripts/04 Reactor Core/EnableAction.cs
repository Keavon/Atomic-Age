using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAction : MonoBehaviour, IActionable {
	public void Run(params int[] inputs) {
		GetComponent<BoxCollider2D>().enabled = true;
	}
}
