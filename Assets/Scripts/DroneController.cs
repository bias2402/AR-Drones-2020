using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour {
    [SerializeField] private Rigidbody droneRb = null;
    [SerializeField] private bool isDroneMoving = false;
    [SerializeField] private ParticleSystem[] droneParticles = null;
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private Transform model = null;

    //Get/Set methods
    #region
    public bool GetIsDroneMoving() { return isDroneMoving; }

    public Rigidbody GetRigidbody() { return droneRb; }

    public Transform GetModel() { return model; }
    #endregion

    private void Start() {
        droneRb.isKinematic = true;
    }

    public void StartDrone() {
        droneRb.isKinematic = false;
        isDroneMoving = true;
        if (audioSource != null) audioSource.Play();

        if (droneParticles.Length == 0) return;
        foreach (ParticleSystem ps in droneParticles) ps.Play();
    }

    public void StopDrone() {
        droneRb.isKinematic = true;
        isDroneMoving = false;
        if (audioSource != null) audioSource.Stop();

        if (droneParticles.Length == 0) return;
        foreach (ParticleSystem ps in droneParticles) ps.Stop();
    }

    public void ResetDrone(Vector3 position, Quaternion rotation) {
        isDroneMoving = false;
        droneRb.transform.position = position;
        droneRb.transform.rotation = rotation;
        droneRb.velocity = Vector3.zero;
        droneRb.angularVelocity = Vector3.zero;

        if (droneParticles.Length == 0) return;
        foreach (ParticleSystem ps in droneParticles) ps.Stop();
    }

    void FixedUpdate() {
        if (isDroneMoving) {
            if (droneRb.velocity.magnitude >= 15) { return; }
            droneRb.AddRelativeForce(Vector3.forward, ForceMode.Acceleration);
        } else {
            droneRb.velocity = Vector3.zero;
            droneRb.angularVelocity = Vector3.zero;
        }
    }
}
