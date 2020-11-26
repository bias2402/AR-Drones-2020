using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneEditor : MonoBehaviour {
    [SerializeField] private Transform dronePlacement = null;
    [SerializeField] private RectTransform partsView = null;
    [SerializeField] private GameObject modelPartPrefab = null;
    private Transform drone = null;
    private List<ModelPart> modelParts = new List<ModelPart>();
    private MeshRenderer currentlySelectedPart = null;

    void OnEnable() {
        drone = dronePlacement.GetChild(0);
        MakeModelPartsList();
    }

    void MakeModelPartsList() {
        List<GameObject> partsToSave = new List<GameObject>();
        Transform model = drone.GetComponent<DroneController>().GetModel();
        if (model == null) return;

        foreach (Transform c in model) {
            if (c.GetComponent<MeshRenderer>() != null) {
                partsToSave.Add(c.gameObject);
            }
        }

        if (modelParts.Count > 0) {
            for (int i = 0; i < modelParts.Count;) {
                Destroy(modelParts[i].gameObject);
                modelParts.RemoveAt(i);
            }
            modelParts.Clear();
        }

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
        partsView.sizeDelta = new Vector2(0, modelParts.Count * 30 + 4);

        foreach (ModelPart mp in modelParts) {
            mp.button.onClick.AddListener(delegate {
                SelectDronePart(mp.index);
            });
        }
        currentlySelectedPart = null;
    }

    public void RotateDroneLeft() => drone.localRotation = Quaternion.Euler(0, 270, 0);

    public void RotateDroneRight() => drone.localRotation = Quaternion.Euler(0, 90, 0);

    public void RotateDroneUp() => drone.localRotation = Quaternion.Euler(-90, 180, 180);

    public void RotateDroneDown() => drone.localRotation = Quaternion.Euler(-90, 180, 0);

    public void RotateDroneFront() => drone.localRotation = Quaternion.Euler(0, 180, 0);

    public void RotateDroneBack() => drone.localRotation = Quaternion.Euler(0, 0, 0);


    void SelectDronePart(int i) {
        foreach (ModelPart mp in modelParts) {
            mp.image.color = Color.white;
        }
        modelParts[i].image.color = Color.yellow;
        currentlySelectedPart = modelParts[i].meshRenderer;
    }

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