using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class OrbitalProperties : MonoBehaviour
{
    [SerializeField]
    private Vector3 velocity;
    [SerializeField]
    private float zenith;
    [SerializeField]
    private float GM;
    [SerializeField]
    private float gravitaionalConstant;
    [SerializeField]
    private GravWell gravWell;
    private GameObject[] celestials;
    private Rigidbody rBody;
    [SerializeField]
    private float apogee;
    [SerializeField]
    private float perigee;
    [SerializeField]
    private float angularMomentum;
    private Vector3 perigeeVector;
    [SerializeField]
    private float distanceToHome;
    [SerializeField]
    private float twoDVelocity;


    void Awake()
    {
        celestials = GameObject.FindGameObjectsWithTag("Celestial");
        rBody = this.GetComponent<Rigidbody>();
        gravitaionalConstant =  gravWell.gravitaionalConstant;
        GM = FindGM();
    }

    void Update()
    {
        twoDVelocity = Math.Abs(Mathf.Sqrt((float)Math.Pow(velocity.x,2)+(float)Math.Pow(velocity.y,2)));
        velocity = rBody.velocity;
        zenith = Vector3.Angle(this.transform.position,velocity.normalized);
        distanceToHome = Vector3.Distance(Vector3.zero, this.transform.position);
        angularMomentum = distanceToHome*rBody.mass*twoDVelocity;
        FindApogee();
    }
    float FindGM()
    {
        // Find the parent (closest) body of the rocket
        var currentParent = FindParent();
        // Find GM
        float gm = 0f;
        float mass = 0f;
        mass = currentParent.GetComponent<Rigidbody>().mass;
        gm = gravitaionalConstant * mass;
        return gm;
    }
    GameObject FindParent()
    {
        var distance = 1000000000000000f;
        GameObject currentParent = null;
        var newDistance = 0f;
        foreach(GameObject a in celestials)
        {
            if(a == gameObject)
            {
                continue;
            }
            else{
                newDistance = Vector3.Distance(a.transform.position, this.transform.position);
                if(newDistance < distance)
                {
                    currentParent = a;
                }
            }
        }
        return currentParent;
    }
    void FindApogee()
    {
        var r1 = distanceToHome;
        var v1 = twoDVelocity;
        var gamma = zenith;
        var C  = 2 * GM / (r1*(float)Math.Pow(v1,2));
        apogee = ((-C - (float)Math.Sqrt((float)Math.Pow(C,2) - 4 * (1-C) * - (float)Math.Pow((float)Mathf.Sin(gamma),2)))/ (2*(1-C))) * r1;
        perigee = ((-C + (float)Math.Sqrt((float)Math.Pow(C,2) - 4 * (1-C) * - (float)Math.Pow((float)Mathf.Sin(gamma),2)))/ (2*(1-C))) * r1;
    }
}
