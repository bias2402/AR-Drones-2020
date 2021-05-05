using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneController : MonoBehaviour {
    [SerializeField] private Rigidbody droneRb = null;
    [SerializeField] private bool isDroneMoving = false;
    [SerializeField] private ParticleSystem[] droneParticles = null;
    [SerializeField] private AudioSource audioSource = null;
    [SerializeField] private Transform model = null;
    [SerializeField] private NavMeshAgent navAgent = null;
    [SerializeField] private float droneSpeed = 15;

    //Get/Set methods
    #region
    public bool GetIsDroneMoving() { return isDroneMoving; }

    public Rigidbody GetRigidbody() { return droneRb; }

    public Transform GetModel() { return model; }

    //Can be used to set the drone's position and rotation, e.g. to set the starting position
    public void SetPosition(Vector3 position, Quaternion rotation) {
        if (!navAgent.enabled) navAgent.enabled = true;
        isDroneMoving = false;
        droneRb.transform.position = position;
        droneRb.transform.rotation = rotation;
        droneRb.velocity = Vector3.zero;
        droneRb.angularVelocity = Vector3.zero;
        navAgent.SetDestination(transform.position - Vector3.up * transform.position.y);
        SetNavAnchor();

        if (droneParticles.Length > 0) foreach (ParticleSystem ps in droneParticles) ps.Stop();
    }
    
    //Offset the NavMesh collider, so the drone can be high up, while keeping its NavMesh collider on the NaxMesh
    void SetNavAnchor() => navAgent.baseOffset = transform.position.y;
    #endregion

    //Set the rigidbody to kinematic,so it doesn't fall down before started
    private void Start() => droneRb.isKinematic = true;

    //Start the drone by activating its rigidbody, set its speed, its destination, and start its effects
    public void StartDrone(Vector3 dest) {
        droneRb.isKinematic = false;
        isDroneMoving = true;
        navAgent.speed = droneSpeed;
        navAgent.SetDestination(dest);

        if (audioSource != null) audioSource.Play();

        if (droneParticles.Length == 0) return;
        foreach (ParticleSystem ps in droneParticles) ps.Play();
    }

    //Stop the drone by cancelling out all the physics and setting it to kinematic (setting it to kinematic
    //should cancel it all out, but setting the other things are merely a safety choice, so it doesn't 
    //somehow keep them for when it is started again)
    public void StopDrone() {
        droneRb.isKinematic = true;
        droneRb.velocity = Vector3.zero;
        droneRb.angularVelocity = Vector3.zero;
        isDroneMoving = false;
        navAgent.SetDestination(transform.position - Vector3.up * transform.position.y);
        if (audioSource != null) audioSource.Stop();

        if (droneParticles.Length == 0) return;
        foreach (ParticleSystem ps in droneParticles) ps.Stop();
    }
}
