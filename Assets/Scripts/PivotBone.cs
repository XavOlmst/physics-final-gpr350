using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PivotBone : MonoBehaviour
{
    [FormerlySerializedAs("Particle2D")] public PhysicsRigidbody2D PhysicsRigidbody2D;
    [FormerlySerializedAs("ParentParticle")] public PhysicsRigidbody2D ParentPhysicsRigidbody;
    public List<PhysicsRigidbody2D> ChildParticles;
    [HideInInspector] public List<Vector3> ChildLocalPositions = new();
    
    private void Start()
    {
        foreach (var capsule in ChildParticles)
        {
            ChildLocalPositions.Add(capsule.transform.localPosition);
        }
    }

    public Vector3 AddTorque(Vector3 localPos, Vector3 contactPoint, Vector3 force)
    {
        float torque = Vector3.Cross(contactPoint + localPos, force).z;
        PhysicsRigidbody2D.AddTorque(torque);
        
        Vector3 movementForce = force.normalized * (force.magnitude - torque);
        
        if (ParentPhysicsRigidbody)
        {
            ParentPhysicsRigidbody.AddForce(movementForce);
            ParentPhysicsRigidbody.AddTorque(transform.localPosition - ParentPhysicsRigidbody.transform.localPosition, movementForce);
        }
        
        return movementForce;
    }
    
    public Vector3 AddTorque(Vector3 contactPoint, Vector3 force)
    {
        float torque = Vector3.Cross(contactPoint, force).z;
        PhysicsRigidbody2D.AddTorque(torque);
        
        Vector3 movementForce = force.normalized * (force.magnitude - torque);
        
        if (ParentPhysicsRigidbody)
        {
            ParentPhysicsRigidbody.AddForce(movementForce);
            ParentPhysicsRigidbody.AddTorque(transform.position - ParentPhysicsRigidbody.transform.position, movementForce);
        }

        foreach (var child in ChildParticles)
        {
            child.AddTorque(transform.position - child.transform.position, movementForce);
        }
        
        return movementForce;
    }
}
