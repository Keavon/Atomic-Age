using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBoxBehavior : MonoBehaviour {

    public Collider2D fallArea;
    public GameObject fallLadder;

    public string actionableScript;
    IActionable actionable;

    void Start()
    {
        actionable = fallLadder.GetComponent(actionableScript) as IActionable;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.Equals(fallArea)){
            actionable.Run();
            Camera.main.GetComponent<CameraController>().ShakeCamera(1, 1);
        }
    }
}
