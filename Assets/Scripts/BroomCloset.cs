using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Trace Rainbolt
public class BroomCloset : MonoBehaviour {

    public GameObject door;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Rigidbody2D>().isKinematic = true;
        StartCoroutine(StartCutscene());
    }

    IEnumerator StartCutscene()
    {
        yield return new WaitForSeconds(2);
        Camera.main.GetComponent<CameraController>().ShakeCamera(2, 2);

        Vector3 doorPosition = door.transform.position;
        Vector3 topOfDoor = new Vector3(doorPosition.x, doorPosition.y + 0.1f, doorPosition.z);
        door.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(20, 100, -100) , topOfDoor);

        player.GetComponent<Rigidbody2D>().isKinematic = false;
    }
}
