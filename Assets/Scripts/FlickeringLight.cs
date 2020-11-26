using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour {
    [SerializeField] private bool isOnAtStart = false;
    [Range(0, 0.5f)] [SerializeField] private float delay = 0;
    private float flickeringTime = 0.5f;
    private float counter = 0;
    private Light lightSource = null;

    void Start() {
        lightSource = GetComponent<Light>();
        lightSource.enabled = isOnAtStart;
        counter += delay;
    }

    void Update() {
        counter += Time.deltaTime;
        if (counter >= flickeringTime) {
            counter = 0;
            lightSource.enabled = !lightSource.enabled;
        }
    }
}