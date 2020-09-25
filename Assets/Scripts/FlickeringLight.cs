using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour {
    [SerializeField] private bool isOnAtStart = false;
    private float flickeringTime = 0.3f;
    private float counter = 0;
    private Light lightSource = null;

    void Start() {
        lightSource = GetComponent<Light>();
        lightSource.enabled = isOnAtStart;
    }

    void Update() {
        counter += Time.deltaTime;
        if (counter >= flickeringTime) {
            counter = 0;
            lightSource.enabled = !lightSource.enabled;
        }
    }
}