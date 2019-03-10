using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreplacedLiquid : MonoBehaviour {
	//Manny Jimenez, needed this for being able to preset liquid in levels

	public Fluid fluid;
    public GameObject liquidPrefab;
    public SpriteRenderer dripsRenderer;

	// Use this for initialization
	void Awake() {
		GameObject liquid = Instantiate(liquidPrefab, new Vector3(0, 0, -1), Quaternion.identity);
		liquid.GetComponent<LiquidBehavior>().PlaceLiquid(transform.position, fluid,  false);
	}
}
