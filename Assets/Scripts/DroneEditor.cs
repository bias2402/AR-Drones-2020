using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneEditor : MonoBehaviour {
    [SerializeField] private OnClickSpecialEffects onClickSpecialEffectsHandler = null;
    [SerializeField] private Transform dronePlacement = null;
    [SerializeField] private RectTransform partsView = null;
    [SerializeField] private GameObject modelPartPrefab = null;
    private Transform drone = null;
    private List<ModelPart> modelParts = new List<ModelPart>();
    private MeshRenderer currentlySelectedPart = null;

    //Set the drone reference to the drone placed in the editor window
    void OnEnable() {
        drone = dronePlacement.GetChild(0);
        MakeModelPartsList();
    }

    //Iterate the drone's model and create a list of parts, which can then be picked in the editor window
    void MakeModelPartsList() {
        List<GameObject> partsToSave = new List<GameObject>();

        //Get the model
        Transform model = drone.GetComponent<DroneController>().GetModel();
        if (model == null) return;

        //Get all the meshrenderer of each part in the drone's model
        foreach (Transform c in model) {
            if (c.GetComponent<MeshRenderer>() != null) {
                partsToSave.Add(c.gameObject);
            }
        }

        //Clear the modelParts list if it already contains something (if another drone has been edited)
        if (modelParts.Count > 0) {
            for (int i = 0; i < modelParts.Count;) {
                Destroy(modelParts[i].gameObject);
                modelParts.RemoveAt(i);
            }
            modelParts.Clear();
        }

        //Create a button for each part of the model and place them correctly in a scrollview
        foreach (GameObject go in partsToSave) {
            GameObject newBtn = Instantiate(modelPartPrefab, partsView);
            newBtn.transform.localPosition = new Vector3(0, -2 - (30 * modelParts.Count), 0);
            ModelPart newPart = new ModelPart(
                newBtn.GetComponent<Button>(),
                newBtn.GetComponent<Image>(),
                go.GetComponent<MeshRenderer>(),
                modelParts.Count,
                newBtn
            );
            newBtn.GetComponentInChildren<Text>().text = go.name;
            modelParts.Add(newPart);
        }
        //Resize the scrollview's content transform, so the user can actually scroll through and see all parts
        partsView.sizeDelta = new Vector2(0, modelParts.Count * 30 + 4);

        //Add some events to play effects and sounds, when the new buttons are clicked
        foreach (ModelPart mp in modelParts) {
            mp.button.onClick.AddListener(delegate {
                SelectDronePart(mp.index);
            });
            mp.button.onClick.AddListener(onClickSpecialEffectsHandler.TriggerFlash);
            mp.button.onClick.AddListener(onClickSpecialEffectsHandler.PlayClickSound);
        }
        currentlySelectedPart = null;
    }

    //These six methods set the rotation of the drone, so the user can see it from different set angles
    public void RotateDroneLeft() => drone.localRotation = Quaternion.Euler(0, 270, 0);

    public void RotateDroneRight() => drone.localRotation = Quaternion.Euler(0, 90, 0);

    public void RotateDroneUp() => drone.localRotation = Quaternion.Euler(-90, 180, 180);

    public void RotateDroneDown() => drone.localRotation = Quaternion.Euler(-90, 180, 0);

    public void RotateDroneFront() => drone.localRotation = Quaternion.Euler(0, 180, 0);

    public void RotateDroneBack() => drone.localRotation = Quaternion.Euler(0, 0, 0);

    //Method called by one of the button events from before, which remove highlight from all buttons and set the
    //clicked button as highlighted (changing its color to yellow)
    void SelectDronePart(int i) {
        foreach (ModelPart mp in modelParts) {
            mp.image.color = Color.white;
        }
        modelParts[i].image.color = Color.yellow;
        currentlySelectedPart = modelParts[i].meshRenderer;
    }

    //When a part is selected, this method can be called to change the color of the part to the clicked color
    public void SetColor(string color) {
        if (currentlySelectedPart == null) return;
        Color c = currentlySelectedPart.material.color;

        switch (color) {
            case "Black":
                c = Color.black;
                break;
            case "Blue":
                c = Color.blue;
                break;
            case "Green":
                c = Color.green;
                break;
            case "Red":
                c = Color.red;
                break;
            case "White":
                c = Color.white;
                break;
            case "Yellow":
                c = Color.yellow;
                break;
        }

        currentlySelectedPart.material.color = c;
    }
}

//Container for keeping references to the different sub-parts of a model part
public struct ModelPart {
    public Button button;
    public Image image;
    public MeshRenderer meshRenderer;
    public int index;
    public GameObject gameObject;

    public ModelPart(Button btn, Image img, MeshRenderer mr, int i, GameObject go) {
        button = btn;
        image = img;
        meshRenderer = mr;
        index = i;
        gameObject = go;
    }
}