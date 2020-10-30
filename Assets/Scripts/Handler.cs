using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Handler : MonoBehaviour {
    private enum FlyMode { FlyBy, FlyAway, FlyTo }
    private FlyMode flyMode = FlyMode.FlyAway;
    private Vector3 startPoint = Vector3.zero;
    private Vector3 destination = Vector3.zero;
    private int altitude = 2;
    private DroneController drone = null;

    [Header("Menu Controls")]
    [SerializeField] private Sprite menuOn = null;
    [SerializeField] private Sprite menuOff = null;
    [SerializeField] private Image menuOnOffImage = null;
    [SerializeField] private Slider progressBar = null;
    [SerializeField] private Image[] modeImages = null;
    [SerializeField] private Image[] altitudeImages = null;

    [Header("Spawnables Settings")]
    [SerializeField] private GameObject showDrones = null;
    [SerializeField] private GameObject showForms = null;
    [SerializeField] private Image currentSpawn = null;

    private bool isMenuShown = true;
    private bool isStartingUp = true;
    private float startCounter = 0;
    private int lastMenuItemShown = -1;

    [Header("Effects")]
    [SerializeField] private Animator menuAnimator = null;
    [SerializeField] private Transform dronePointer = null;

    void Update() {
        if (isStartingUp) {
            if (startCounter > 3) {
                startCounter += Time.deltaTime;
            } else {
                CheckVisibilityOfMenuParts();
                isStartingUp = false;
            }
        } else {
            if (drone != null && drone.GetIsDroneMoving()) {
                if (DestinationReached()) {
                    drone.ResetDrone(FlyModePosition(), FlyModeRotation());
                    drone.StartDrone();
                }
            }

            //if (drone != null) {
            //    UpdateDronePointer();
            //}
        }
    }

    void UpdateDronePointer() {
        Vector3 relativePosition = drone.GetRigidbody().position - dronePointer.position;
        float angle = Vector3.Angle(relativePosition, dronePointer.forward);
        dronePointer.rotation = Quaternion.Euler(0, angle, 50);
    }                       //Work in process

    void CheckVisibilityOfMenuParts() {
        if (lastMenuItemShown < 4) {
            lastMenuItemShown++;
            menuAnimator.SetInteger("menuShown", lastMenuItemShown);
        }
    }

    public void ToggleMenu() {
        CheckVisibilityOfMenuParts();
        isMenuShown = !isMenuShown;
        menuAnimator.SetBool("isShown", isMenuShown);
        menuOnOffImage.sprite = isMenuShown ? menuOn : menuOff;
    }

    public void ShowSpawnablesCategory(int category) {
        switch (category) {
            case 0:
                showDrones.SetActive(true);
                showForms.SetActive(false);
                break;
            case 1:
                showDrones.SetActive(false);
                showForms.SetActive(true);
                break;
        }
    }

    public void SpawnDrone(GameObject droneGO) {
        if (droneGO == null) return;
        showDrones.SetActive(false);
        showForms.SetActive(false);

        if (lastMenuItemShown == 0) {
            lastMenuItemShown++;
            menuAnimator.SetInteger("menuShown", lastMenuItemShown);
        }
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        drone = Instantiate(droneGO, transform).GetComponent<DroneController>();
    }

    public void SpawnDrone(Sprite spr) {
        if (spr == null) return;
        currentSpawn.sprite = spr;
    }

    public void StartDrone() {
        CheckVisibilityOfMenuParts();
        drone.StartDrone();
    }

    public void FlyAway(Image btn) {
        CheckVisibilityOfMenuParts();
        flyMode = FlyMode.FlyAway;
        drone.ResetDrone(FlyModePosition(), FlyModeRotation());
        destination = new Vector3(0, altitude, 120);
        ModeColors(btn);
    }

    public void FlyTowards(Image btn) {
        CheckVisibilityOfMenuParts();
        flyMode = FlyMode.FlyTo;
        drone.ResetDrone(FlyModePosition(), FlyModeRotation());
        destination = new Vector3(0, altitude, 0);
        ModeColors(btn);
    }

    public void FlyBy(Image btn) {
        CheckVisibilityOfMenuParts();
        flyMode = FlyMode.FlyBy;
        drone.ResetDrone(FlyModePosition(), FlyModeRotation());
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
                if (drone.transform.position.z > destination.z) { return true; }
                break;
            case FlyMode.FlyBy:
                if (drone.transform.position.x < destination.x) { return true; }
                break;
            case FlyMode.FlyTo:
                if (drone.transform.position.z < destination.z) { return true; }
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
        if (drone != null) drone.transform.position = 
                new Vector3(drone.transform.position.x, altitude, drone.transform.position.z);
    }

    public void SetAltitude(Image btn) {
        foreach (Image img in altitudeImages) {
            img.color = Color.white;
        }
        btn.color = Color.yellow;
    }
}