using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Handler : MonoBehaviour {
    private enum FlyMode { FlyBy, FlyAway, FlyTo }
    private FlyMode flyMode = FlyMode.FlyAway;
    private Vector3 destination = Vector3.zero;
    private int altitude = 2;
    private DroneController drone = null;

    [Header("Menu Controls")]
    [SerializeField] private GameObject menu = null;
    [SerializeField] private GameObject editor = null;
    [SerializeField] private Sprite menuOn = null;
    [SerializeField] private Sprite menuOff = null;
    [SerializeField] private Image menuOnOffImage = null;
    [SerializeField] private Image[] modeImages = null;
    [SerializeField] private Image[] altitudeImages = null;
    [SerializeField] private Text droneStatusText = null;
    [SerializeField] private GameObject[] helpWindows = null;
    [SerializeField] private GameObject postStartAnimHelpWindow = null;
    [SerializeField] private GameObject tutorialPrompt = null;

    [Header("Drone Editor")]
    [SerializeField] private Button droneEditorBtn = null;
    [SerializeField] private Transform dronePlacementInEditor = null;

    [Header("Spawnables Settings")]
    [SerializeField] private Vector3 spawnPosition = new Vector3(3.5f, -2.5f, 18);
    [SerializeField] private Vector3 spawnRotation = new Vector3(0, 180, 0);
    [SerializeField] private GameObject showDrones = null;
    [SerializeField] private GameObject showForms = null;
    [SerializeField] private Image currentSpawn = null;

    [Header("Animation Settings")]
    private bool isMenuShown = true;
    private bool isStartingUp = true;
    private bool isThroughPrompt = false;
    private float startCounter = 0;
    private float animEndCounter = 0;
    private int startupAnimationIndex = -1;

    [Header("Effects")]
    [SerializeField] private Animator menuAnimator = null;
    [SerializeField] private Transform dronePointer = null;
    //[SerializeField] private Text testText = null;

    void Start() {
        menuAnimator.enabled = false;
        editor.SetActive(false);
        menu.SetActive(false);
        currentSpawn.gameObject.SetActive(false);
        menuOnOffImage.gameObject.SetActive(false);
        droneStatusText.text = "Drone status: no drone";
    }

    void Update() {
        //Start-up prompt and animation handling
        #region
        if (!isThroughPrompt) return;
        if (isStartingUp) {
            startCounter += Time.deltaTime;
            if (menuAnimator.GetInteger("menuShown") == -1) CheckVisibilityOfMenuParts();
            if (startCounter > 1) {
                menu.SetActive(true);
                isStartingUp = false;
            }
            return;
        }

        if (startupAnimationIndex == 3) {
            animEndCounter += Time.deltaTime;
            if (animEndCounter > 8) {
                CheckVisibilityOfMenuParts();
            }
        }

        if (startupAnimationIndex == 4) {
            startupAnimationIndex = 10;
            ShowHelpWindow(-1);
            postStartAnimHelpWindow.SetActive(true);
            menuAnimator.SetInteger("menuShown", startupAnimationIndex);
        }
        #endregion

        //Drone control
        if (startupAnimationIndex >= 10) {
            if (drone != null) {
                if (drone.GetIsDroneMoving()) {
                    if (Vector3.Distance(FlyModeDestination(), drone.transform.position - Vector3.up * altitude) < 2) {
                        drone.SetPosition(FlyModeStartPosition(), FlyModeRotation());
                        drone.StartDrone(FlyModeDestination());
                    }
                }

                //    UpdateDronePointer();
            }
        }
    }

    public void WantToSeeStartUpAnimation(bool doAnimate) {
        tutorialPrompt.SetActive(false);
        isThroughPrompt = true;
        menuAnimator.enabled = true;

        if (!doAnimate) {
            isStartingUp = false;
            startupAnimationIndex = 10;
            menuAnimator.SetInteger("menuShown", startupAnimationIndex);
            menu.SetActive(true);
            postStartAnimHelpWindow.SetActive(true);
        }
    }

    void UpdateDronePointer() {
        Vector3 relativePosition = drone.GetRigidbody().position - dronePointer.position;
        float angle = Vector3.Angle(relativePosition, dronePointer.forward);
        dronePointer.rotation = Quaternion.Euler(0, angle, 50);
    }                       //Work in process

    void CheckVisibilityOfMenuParts() {
        if (menuAnimator.GetInteger("menuShown") == 10) return;
        if (startupAnimationIndex < 10) {
            startupAnimationIndex++;
            if (startupAnimationIndex < helpWindows.Length) ShowHelpWindow(startupAnimationIndex);
            menuAnimator.SetInteger("menuShown", startupAnimationIndex);
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
            case -1:
                showDrones.SetActive(false);
                showForms.SetActive(false);
                break;
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

        if (startupAnimationIndex == 0) CheckVisibilityOfMenuParts();
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        drone = Instantiate(droneGO, transform).GetComponent<DroneController>();
        drone.transform.position = spawnPosition;
        drone.transform.rotation = Quaternion.Euler(spawnRotation);
        droneStatusText.text = "Drone status: stationary";
        if (!droneEditorBtn.interactable) droneEditorBtn.interactable = true;
    }

    public void SpawnDrone(Sprite spr) {
        if (spr == null) return;
        currentSpawn.sprite = spr;
    }

    public void StartDrone() {
        if (drone == null) return;
        drone.StartDrone(FlyModeDestination());
        droneStatusText.text = "Drone status: flying";
    }

    public void StopDrone() {
        if (drone == null) return;
        drone.StopDrone();
        droneStatusText.text = "Drone status: stationary";
    }

    public void ResetDrone() {
        if (drone == null) return;
        drone.SetPosition(FlyModeStartPosition(), FlyModeRotation());
        droneStatusText.text = "Drone status: stationary";
    }

    public void FlyAway(Image btn) {
        if (drone == null) return;
        if (startupAnimationIndex == 2) CheckVisibilityOfMenuParts();
        flyMode = FlyMode.FlyAway;
        drone.SetPosition(FlyModeStartPosition(), FlyModeRotation());
        ModeColors(btn);
    }

    public void FlyTowards(Image btn) {
        if (drone == null) return;
        if (startupAnimationIndex == 2) CheckVisibilityOfMenuParts();
        flyMode = FlyMode.FlyTo;
        drone.SetPosition(FlyModeStartPosition(), FlyModeRotation());
        ModeColors(btn);
    }

    public void FlyBy(Image btn) {
        if (drone == null) return;
        if (startupAnimationIndex == 2) CheckVisibilityOfMenuParts();
        flyMode = FlyMode.FlyBy;
        drone.SetPosition(FlyModeStartPosition(), FlyModeRotation());
        ModeColors(btn);
    }

    void ModeColors(Image btn) {
        if (drone == null) return;
        foreach (Image img in modeImages) {
            img.color = Color.white;
        }
        btn.color = Color.yellow;
    }

    Vector3 FlyModeStartPosition() {
        switch (flyMode) {
            case FlyMode.FlyAway:
                return new Vector3(0, altitude, -100);
            case FlyMode.FlyBy:
                return new Vector3(100, altitude, -50);
            case FlyMode.FlyTo:
                return new Vector3(0, altitude, 150);
            default:
                return Vector3.zero;
        }
    }
    Vector3 FlyModeDestination() {
        switch (flyMode) {
            case FlyMode.FlyAway:
                return new Vector3(0, 0, 150);
            case FlyMode.FlyBy:
                return new Vector3(-100, 0, 50);
            case FlyMode.FlyTo:
                return new Vector3(0, 0, -100);
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
        if (drone == null) return;
        if (startupAnimationIndex == 1) CheckVisibilityOfMenuParts();
        this.altitude = altitude;
        drone.SetPosition(FlyModeStartPosition(), FlyModeRotation());
    }

    public void SetAltitude(Image btn) {
        if (drone == null) return;
        foreach (Image img in altitudeImages) {
            img.color = Color.white;
        }
        btn.color = Color.yellow;
    }

    public void OpenDroneEditor(bool isOpening) {
        menuAnimator.SetBool("isUsingEditor", isOpening);

        if (isOpening) {
            drone.transform.parent = dronePlacementInEditor;
            drone.transform.localPosition = Vector3.zero;
        } else {
            drone.transform.parent = transform;
            drone.transform.localPosition = spawnPosition;
            drone.transform.rotation = Quaternion.Euler(spawnRotation);
        }
    }

    public void ShowHelpWindow(int index) {
        postStartAnimHelpWindow.SetActive(false);

        if (index > -1 && helpWindows[index].activeSelf) {
            foreach (GameObject go in helpWindows) {
                go.SetActive(false);
            }
        } else {
            foreach (GameObject go in helpWindows) {
                go.SetActive(false);
            }
            if (index == -1) return;
            Debug.Log("Here");
            helpWindows[index].SetActive(true);
        }

    }
}