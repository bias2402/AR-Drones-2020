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
    
    void SetNavAnchor() => navAgent.baseOffset = transform.position.y;
    #endregion

    private void Start() {
        droneRb.isKinematic = true;
    }

    public void StartDrone(Vector3 dest) {
        droneRb.isKinematic = false;
        isDroneMoving = true;
        navAgent.speed = droneSpeed;
        navAgent.SetDestination(dest);

        if (audioSource != null) audioSource.Play();

        if (droneParticles.Length == 0) return;
        foreach (ParticleSystem ps in droneParticles) ps.Play();
    }

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

    /*void FixedUpdate() {
        if (isDroneMoving) {
            if (IsAnObstaclesInFront(out bool turnLeft, out bool turnRight)) {
                Debug.Log("Found " + turnLeft + " " + turnRight);
                if (turnLeft && turnRight) {
                    transform.Rotate(Vector3.up, 10 * Time.deltaTime);
                } else if (turnLeft) {
                    transform.Rotate(Vector3.up, -10 * Time.deltaTime);
                } else if (turnRight) {
                    transform.Rotate(Vector3.up, 10 * Time.deltaTime);
                }
            }
            if (droneRb.velocity.magnitude >= 15) return;
            droneRb.AddRelativeForce(Vector3.forward, ForceMode.VelocityChange);
        } else {
            droneRb.velocity = Vector3.zero;
            droneRb.angularVelocity = Vector3.zero;
        }
    }

    bool IsAnObstaclesInFront(out bool turnLeft, out bool turnRight) {
        Physics.Raycast(transform.position, transform.forward + new Vector3(0.15f, 0, 0), out RaycastHit hitL, 150, ~LayerMask.NameToLayer("Drone"));
        Physics.Raycast(transform.position, transform.forward + new Vector3(-0.15f, 0, 0), out RaycastHit hitR, 150, ~LayerMask.NameToLayer("Drone"));
        turnLeft = hitL.collider == null ? false : true;
        turnRight = hitR.collider == null ? false : true;
        return hitR.collider != null || hitL.collider != null ? true : false;
    }*/
}
