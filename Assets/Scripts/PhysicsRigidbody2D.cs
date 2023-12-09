using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PhysicsRigidbody2D : MonoBehaviour
{
    public Vector3 Velocity;
    public float AngularVelocity;
    public float Damping = 0.999f;
    public float AngularDamping = 0.99f;
    public Vector3 Acceleration;
    public float AngularAcceleration;
    public Vector3 Gravity = new Vector3(0, -9.8f);
    public float InverseMass = 1.0f;
    public Vector3 AccumulatedForces { get; private set; }
    public float AccumulatedTorque { get; private set; }
    public float MomentOfInertia = 1;
    public float ImpartRatio = 0.2f;
    public Transform Bone;
    public List<PhysicsRigidbody2D> ChildCapsule;
    public PhysicsRigidbody2D ParentCapsule;
    public bool IsRootBone = false;

    public void FixedUpdate()
    {
        DoFixedUpdate(Time.deltaTime);
    }
    
    private void DoFixedUpdate(float dt)
    {
        Acceleration = Gravity + AccumulatedForces * InverseMass;
        Integrator.Integrate(this, dt);
        ClearForces();
        ClearTorque();
    }

    private void ClearForces()
    {
        AccumulatedForces = Vector3
            .zero;
    }

    private void ClearTorque()
    {
        AccumulatedTorque = 0;
    }

    public void AddForce(Vector3
            force)
    {
        AccumulatedForces += force;
    }

    public void AddTorque(float torque)
    {
        AccumulatedTorque += torque;
    }
    
    public Vector3 AddTorque(Vector3 radius, Vector3 force)
    {
        Vector3 cross = Vector3.Cross(radius, force);

        AccumulatedTorque += cross.z;

        return force.normalized * (force.magnitude - cross.z);
    }

    public Vector3 GetTotalVelocity()
    {
        PhysicsRigidbody2D prb = this;
        Vector3 totalVelocity = Vector3.zero;
        
        while (prb)
        {
            totalVelocity += (Vector3) prb.Velocity;
            prb = prb.ParentCapsule;
        }

        return totalVelocity;
    }
}
