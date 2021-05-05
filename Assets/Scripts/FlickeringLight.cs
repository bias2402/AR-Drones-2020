using UnityEngine;

public class FlickeringLight : MonoBehaviour {
    [SerializeField] private bool isOnAtStart = false;
    [Range(0, 0.5f)] [SerializeField] private float delay = 0;
    private float flickeringTime = 0.5f;
    private float counter = 0;
    private Light lightSource = null;

    //Prepare the light effect, and add the delay if it's set in the inspector
    void Start() {
        lightSource = GetComponent<Light>();
        lightSource.enabled = isOnAtStart;
        counter += delay;
    }

    //Whenever the counter hits the timer, turn the light on or off
    void Update() {
        counter += Time.deltaTime;
        if (counter >= flickeringTime) {
            counter = 0;
            lightSource.enabled = !lightSource.enabled;
        }
    }
}