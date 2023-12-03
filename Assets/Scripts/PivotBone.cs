using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PivotBone : MonoBehaviour
{
    public Particle2D Particle2D;
    public Particle2D ParentParticle;
    public List<Particle2D> ChildParticles;
    [HideInInspector] public List<Vector3> ChildLocalPositions = new();
    
    private void Start()
    {
        foreach (var capsule in ChildParticles)
        {
            ChildLocalPositions.Add(capsule.transform.localPosition);
        }
    }

    public Vector3 AddTorque(Vector3 contactPoint, Vector3 force)
    {
        float torque = Vector3.Cross(contactPoint, force).z;
        Particle2D.AddTorque(torque);
        
        Vector3 movementForce = force.normalized * (force.magnitude - torque);
        
        if (ParentParticle)
        {
            ParentParticle.AddForce(movementForce);
            ParentParticle.AddTorque(transform.localPosition - ParentParticle.transform.localPosition, movementForce);
        }

        return movementForce;
    }
}
