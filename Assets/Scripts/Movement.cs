using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    [HideInInspector] public enum FlyMode { FlyAway, FlyTo, FlyBy, TakeOff, Crash, Orbit }
    [HideInInspector] public FlyMode flyMode;
    [HideInInspector] public Vector3 startPosition = Vector3.zero;
    [HideInInspector] public Quaternion startRotation = Quaternion.identity;
    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public float speed = 1;

    void Start() {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void FixedUpdate() {
        if (isMoving) {
            switch (flyMode) {
                case FlyMode.Crash:
                case FlyMode.FlyAway:
                case FlyMode.FlyBy:
                case FlyMode.FlyTo:
                case FlyMode.TakeOff:
                    transform.Translate(Vector3.forward * speed, Space.Self);
                    break;
                case FlyMode.Orbit:
                    transform.RotateAround(Vector3.zero, Vector3.up, speed);
                    break;
            }
        }
    }

    public void StartDrone() => isMoving = true;

    public void StopDrone() => isMoving = false;

    public void ResetDrone() {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}