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
    private float smooth = 3f;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    Transform targetTransform;
    Vector3 originalPos;
    private GameObject[] oldTargets;
    void Start(){
        originalPos = gameObject.transform.position;
        Debug.Log("Assigning variables");
        rBody = this.GetComponent<Rigidbody>();
        properties = this.GetComponent<properties>();
    }
    public override void OnEpisodeBegin()
    {
        rBody.velocity = Vector3.zero;
        gameObject.transform.position = originalPos;
        Debug.Log("attempting to assign transform");
        targetTransform.localPosition = new Vector3(Random.Range(-2800,2800),0,Random.Range(1800,2800));
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
            Thrust(actionBuffers.ContinuousActions[1]);
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
            //ResetEnv();
            EndEpisode();
        }
        else if (distanceToHome > 10000f)
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
    void Thrust(float Thrust)
    {
        rBody.AddForce(transform.up*Thrust*1000);
    }
}
