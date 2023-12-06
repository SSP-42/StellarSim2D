using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;

public class Orbiter : Agent
{
    private Rigidbody rBody;
    private properties properties;
    private OrbitalProperties orbitalProperties;
    [SerializeField]
    private float smooth = 3f;
    Vector3 originalPos;
    public bool thrusting; // There has to be a better name for this.
    private float apoapsis;
    private float periapsis;
    private Quaternion originalRot;
    private bool acheivedTargetOrbit;
    void Start(){
        Debug.Log("Assigning variables");
        originalPos = gameObject.transform.position;
        originalRot = gameObject.transform.rotation;
        rBody = this.GetComponent<Rigidbody>();
        properties = this.GetComponent<properties>();
        orbitalProperties = this.GetComponent<OrbitalProperties>();
    }
    public override void OnEpisodeBegin()
    {
        // reset rocket and create new target apoapsis and periapsis
        rBody.velocity = Vector3.zero;
        gameObject.transform.position = originalPos;
        gameObject.transform.rotation = originalRot;
        apoapsis = Random.Range(2000,3000);
        periapsis = Random.Range(2000,3000);

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //*target and self positon
        sensor.AddObservation(orbitalProperties.apogee);
        sensor.AddObservation(orbitalProperties.perigee);
        sensor.AddObservation(apoapsis);
        sensor.AddObservation(periapsis);
        //*current velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.y);
        //*current orientation
        sensor.AddObservation(this.transform.rotation.z);
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //*actions
        Quaternion controlRotation = Quaternion.Euler(0,0,actionBuffers.ContinuousActions[0]);
        transform.rotation = Quaternion.Slerp(this.transform.rotation,controlRotation,Time.deltaTime * smooth);  

        if (actionBuffers.ContinuousActions[1] > 0f)
        {
            rBody.AddForce(transform.up*actionBuffers.ContinuousActions[1]*1000);
            //thrusting = true;
        }
        else if (actionBuffers.ContinuousActions[1] == 0f)
        {
            //thrusting = false;
        }
        //* Rewards
        float distanceToHome = Vector3.Distance(this.transform.localPosition,Vector3.zero);
        if (orbitalProperties.apogee == apoapsis)
        {
            acheivedTargetOrbit = true;
            SetReward(2.0f);
        }
        else if (properties.contact == true || acheivedTargetOrbit == false){
            Debug.Log("Crashlanding");
            SetReward(-1.0f);
            EndEpisode();
        }
        else if (properties.contact == true || acheivedTargetOrbit)
        {
            EndEpisode();
        }
        else if (distanceToHome > 100000f)
        {
            Debug.Log("Lost to the void");
            SetReward(-0.5f);
            EndEpisode();
        }
    }
    public float z = 50f;
    public float test_thrust = 100f;
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = z;
        continuousActionsOut[1] = test_thrust;

    }
}
