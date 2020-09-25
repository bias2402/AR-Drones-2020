using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlHandler : MonoBehaviour {
    private enum FlyMode { FlyBy, FlyAway, FlyTo }
    private FlyMode flyMode = FlyMode.FlyAway;
    private Rigidbody droneRb = null;
    private Vector3 startPoint = Vector3.zero;
    private Vector3 destination = Vector3.zero;
    private int altitude = 2;

    [Header("Menu Controls")]
    [SerializeField] private Sprite menuOn = null;
    [SerializeField] private Sprite menuOff = null;
    [SerializeField] private Image menuOnOffImage = null;
    [SerializeField] private Slider progressBar = null;
    [SerializeField] private Image[] spawnImages = null;
    [SerializeField] private Image[] modeImages = null;
    [SerializeField] private Image[] altitudeImages = null;

    private bool isMenuShown = true;
    private bool isDroneMoving = false;
    private bool isStartingUp = true;
    private float startCounter = 0;
    private int lastMenuItemShown = -1;

    [Header("Effects")]
    [SerializeField] private Animator menuAnimator = null;
    [SerializeField] private Transform dronePointer = null;

    private void Update() {
        if (isStartingUp) {
            if (startCounter > 3) {
                startCounter += Time.deltaTime;
            } else {
                CheckVisibilityOfMenuParts();
                isStartingUp = false;
            }
        } else {
            if (isDroneMoving) {
                if (DestinationReached()) {
                    droneRb.position = startPoint;
                }
                UpdateProgressBar();
            }

            if (droneRb != null) {
                UpdateDronePointer();
            }
        }
    }

    void UpdateProgressBar() {
        float value = Vector3.Distance(destination, droneRb.position) / Vector3.Distance(destination, startPoint) * 100;
        progressBar.value = value;
    }

    void UpdateDronePointer() {
        Vector3 relativePosition = droneRb.position - dronePointer.position;
        float angle = Vector3.Angle(relativePosition, dronePointer.forward);
        dronePointer.rotation = Quaternion.Euler(0, angle, 50);
    }

    void FixedUpdate() {
        if (droneRb == null) { return; }
        if (isDroneMoving) {
            if (droneRb.velocity.magnitude >= 15) { return; }
            droneRb.AddRelativeForce(Vector3.forward, ForceMode.Acceleration);
        } else {
            droneRb.velocity = Vector3.zero;
            droneRb.angularVelocity = Vector3.zero;
        }
    }

    void CheckVisibilityOfMenuParts() {
        if (lastMenuItemShown < 4) {
            lastMenuItemShown++;
            menuAnimator.SetInteger("menuShown", lastMenuItemShown);
        }
    }

    public void ToggleMenu() {
        isMenuShown = !isMenuShown;
        menuAnimator.SetBool("isShown", isMenuShown);
        menuOnOffImage.sprite = isMenuShown ? menuOn : menuOff;
    }

    public void SpawnDrone(GameObject droneGO) {
        if (lastMenuItemShown == 0) {
            lastMenuItemShown++;
            menuAnimator.SetInteger("menuShown", lastMenuItemShown);
        }
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        GameObject drone = Instantiate(droneGO, transform);
        droneRb = drone.GetComponent<Rigidbody>();
    }

    public void SpawnDrone(Image btn) {
        foreach (Image img in spawnImages) {
            img.color = Color.white;
        }
        btn.color = Color.yellow;
    }

    public void StartDrone() {
        CheckVisibilityOfMenuParts();
        isDroneMoving = true;
    }

    public void StopDrone() {
        CheckVisibilityOfMenuParts();
        isDroneMoving = false;
    }

    public void ResetDrone() {
        CheckVisibilityOfMenuParts();
        isDroneMoving = false;
        droneRb.transform.position = FlyModePosition();
        droneRb.transform.rotation = FlyModeRotation();
        droneRb.velocity = Vector3.zero;
        droneRb.angularVelocity = Vector3.zero;
    }

    public void FlyAway(Image btn) {
        CheckVisibilityOfMenuParts();
        flyMode = FlyMode.FlyAway;
        startPoint = droneRb.transform.position = FlyModePosition();
        droneRb.transform.rotation = FlyModeRotation();
        droneRb.velocity = Vector3.zero;
        droneRb.angularVelocity = Vector3.zero;
        destination = new Vector3(0, altitude, 120);
        ModeColors(btn);
    }

    public void FlyTowards(Image btn) {
        CheckVisibilityOfMenuParts();
        flyMode = FlyMode.FlyTo;
        startPoint = droneRb.transform.position = FlyModePosition();
        droneRb.transform.rotation = FlyModeRotation();
        droneRb.velocity = Vector3.zero;
        droneRb.angularVelocity = Vector3.zero;
        destination = new Vector3(0, altitude, 0);
        ModeColors(btn);
    }

    public void FlyBy(Image btn) {
        CheckVisibilityOfMenuParts();
        flyMode = FlyMode.FlyBy;
        startPoint = droneRb.transform.position = FlyModePosition();
        droneRb.transform.rotation = FlyModeRotation();
        droneRb.velocity = Vector3.zero;
        droneRb.angularVelocity = Vector3.zero;
        destination = new Vector3(-100, altitude, 50);
        ModeColors(btn);
    }

    void ModeColors(Image btn) {
        foreach (Image img in modeImages) {
            img.color = Color.white;
        }
        btn.color = Color.yellow;
    }

    bool DestinationReached() {
        switch (flyMode) {
            case FlyMode.FlyAway:
                if (droneRb.position.z > destination.z) { return true; }
                break;
            case FlyMode.FlyBy:
                if (droneRb.position.x < destination.x) { return true; }
                break;
            case FlyMode.FlyTo:
                if (droneRb.position.z < destination.z) { return true; }
                break;
        }
        return false;
    }

    Vector3 FlyModePosition() {
        switch (flyMode) {
            case FlyMode.FlyAway:
                return new Vector3(0, altitude, 0);
            case FlyMode.FlyBy:
                return new Vector3(100, altitude, 50);
            case FlyMode.FlyTo:
                return new Vector3(0, altitude, 100);
            default:
                return Vector3.zero;
        } 
    }

    Quaternion FlyModeRotation() {
        switch (flyMode) {
            case FlyMode.FlyAway:
                return Quaternion.Euler(0, 0, 0);
            case FlyMode.FlyBy:
                return Quaternion.Euler(0, -90, 0);
            case FlyMode.FlyTo:
                return Quaternion.Euler(0, 180, 0);
            default:
                return Quaternion.Euler(Vector3.zero);
        }
    }

    public void SetAltitude(int altitude) {
        CheckVisibilityOfMenuParts();
        this.altitude = altitude;
        if (droneRb != null) droneRb.position = new Vector3(droneRb.position.x, altitude, droneRb.position.z);
    }

    public void SetAltitude(Image btn) {
        foreach (Image img in altitudeImages) {
            img.color = Color.white;
        }
        btn.color = Color.yellow;
    }
}
