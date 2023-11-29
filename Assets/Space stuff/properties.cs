using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0219 // disable message: variable is assigned but never used
#pragma warning disable 0414 // disable message: private field is assigned but never used

public class properties : MonoBehaviour
{
    public bool IsStableOrbital = true;
    public bool CentralOrbit = false;
    public bool DrawOrbit = false;
    public bool contact = false;
    void OnCollisionExit(Collision collision)
    {
        contact = false;
    }
    void OnCollisionEnter(Collision collision)
    {
        contact = true;
        Debug.Log(contact);
    }
}
