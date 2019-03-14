using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class PreplacedLiquid : MonoBehaviour {
	//Manny Jimenez, needed this for being able to preset liquid in levels

	public Fluid fluid;
	public GameObject liquidPrefab;

	// Use this for initialization
	void Start() {
		GameObject liquid = Instantiate(liquidPrefab, new Vector3(0, 0, -1), Quaternion.identity);
		liquid.transform.parent = gameObject.transform.parent;
		liquid.GetComponent<LiquidBehavior>().PlaceLiquid(transform.position, fluid, false);
	}
}
