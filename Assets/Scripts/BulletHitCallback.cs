using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitCallback : MonoBehaviour
{
    private Person rootPerson;

    public void SetPerson(Person person)
    {
        rootPerson = person;
    }
    
    public void GetShot(Vector3 vector3)
    {
        rootPerson.GetShot(vector3);
    }
}
