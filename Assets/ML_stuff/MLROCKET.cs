using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;

public class MLROCKET : Agent
{
    private Rigidbody rBody;
    private properties properties;
    [SerializeField]
    private float smooth = 3f;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    Transform targetTransform;
    Vector3 originalPos;
    private GameObject[] oldTargets;
    public bool thrusting; // There has to be a better name for this.
    private Quaternion originalRot;
    void Start(){
        Debug.Log("Assigning variables");
        originalPos = gameObject.transform.position;
        originalRot = gameObject.transform.rotation;
        rBody = this.GetComponent<Rigidbody>();
        properties = this.GetComponent<properties>();
    }
    public override void OnEpisodeBegin()
    {
        // reset rocket and create new target
        rBody.velocity = Vector3.zero;
        gameObject.transform.position = originalPos;
        gameObject.transform.rotation = originalRot;
        Debug.Log("attempting to assign transform");
        targetTransform.localPosition = new Vector3(Random.Range(-2800,2800),Random.Range(1800,2800),0);
        Debug.Log(targetTransform.localPosition);
        Debug.Log("Attempting to instantiate");
        oldTargets = GameObject.FindGameObjectsWithTag("target");
        foreach(GameObject a in oldTargets)
        {
            Destroy(a);
        }
        Instantiate(target,targetTransform);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //*target and self positon
        sensor.AddObservation(targetTransform.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        //*current velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.y);
        sensor.AddObservation(rBody.velocity.z);
        //*current orientation
        sensor.AddObservation(this.transform.rotation.x);
        sensor.AddObservation(this.transform.rotation.y);
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
        float distanceToTarget = Vector3.Distance(this.transform.localPosition,targetTransform.localPosition);
        float distanceToHome = Vector3.Distance(this.transform.localPosition,Vector3.zero);
        if (distanceToTarget < 10f)
        {
            Debug.Log("hit target");
            SetReward(2f);
            EndEpisode();
        }
        else if (properties.contact == true){
            Debug.Log("Crashlanding");
            SetReward(-2.0f);
            EndEpisode();
        }
        else if (distanceToHome > 100000f)
        {
            Debug.Log("Lost to the void");
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
