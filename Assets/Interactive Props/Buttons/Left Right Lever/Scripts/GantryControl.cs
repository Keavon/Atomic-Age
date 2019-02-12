using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GantryControl : MonoBehaviour {
    public GameObject crane;
    public GameObject lever;
    bool triggered = false;

    private Animation craneAnimation;
    private Animation leverAnimation;

    // Use this for initialization
    void Start() {
        craneAnimation = crane.GetComponent<Animation>();
        leverAnimation = lever.GetComponent<Animation>();
        craneAnimation["Crane"].speed = -1;
        leverAnimation["Lever"].speed = -1;
    }

    void Update() {
        if (InputManager.interactPress() && triggered) {
            AnimationState clip = craneAnimation["Crane"];
            if (clip.speed == 1) {
                if (!craneAnimation.isPlaying) clip.time = clip.length;
                clip.speed = -1;
                craneAnimation.Play();
            } else {
                if (!craneAnimation.isPlaying) clip.time = 0;
                clip.speed = 1;
                craneAnimation.Play();
            }

            clip = leverAnimation["Lever"];
            if (clip.speed == 1) {
                if (!leverAnimation.isPlaying) clip.time = clip.length;
                clip.speed = -1;
                leverAnimation.Play();
            } else {
                if (!leverAnimation.isPlaying) clip.time = 0;
                clip.speed = 1;
                leverAnimation.Play();
            }
        }
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) triggered = true;
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) triggered = false;
    }
}
