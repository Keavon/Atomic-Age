using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Trace Rainbolt
public class CammandCenter : MonoBehaviour {

    public GameObject fallLadder;

    public string actionableScript;
    IActionable actionable;

    void Start()
    {
        actionable = fallLadder.GetComponent(actionableScript) as IActionable;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            StartCoroutine(StartCutscene());
        }
    }

    IEnumerator StartCutscene()
    {
        yield return new WaitForSeconds(2);
        actionable.Run();
        Camera.main.GetComponent<CameraController>().ShakeCamera(2, 2);
        Destroy(gameObject);
    }
}
