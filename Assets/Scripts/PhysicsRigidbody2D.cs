using System;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRigidbody2D : MonoBehaviour
{
    public Vector2 velocity;
    public float angularVelocity;
    public float damping = 0.999f;
    public float angularDamping = 0.99f;
    public Vector2 acceleration;
    public float angularAcceleration;
    public Vector2 gravity = new Vector2(0, -9.8f);
    public float inverseMass = 1.0f;
    public Vector2 accumulatedForces { get; private set; }
    public float accumulatedTorque { get; private set; }
    public float mass => 1 / inverseMass;
    public float momentOfInertia = 1;
    public float impartRatio = 0.2f;
    public Transform Bone;
    public PhysicsRigidbody2D ChildCapsule;
    public PhysicsRigidbody2D ParentCapsule;
    public bool IsRootBone = false;
    private Vector3 _initialLocalPos;

    private void Start()
    {
        if (Bone)
        {
            _initialLocalPos = transform.localPosition;
        }
    }

    public void FixedUpdate()
    {
        /*if (Bone)
        {
            foreach (var child in Bone.ChildParticles)
            {
                DoFixedUpdate(Time.deltaTime, child);
            }
        }*/
        
        DoFixedUpdate(Time.deltaTime);
    }

    public Vector3 GetInitPos() => _initialLocalPos;
    
    public void DoFixedUpdate(float dt)
    {
        acceleration = gravity + accumulatedForces * inverseMass;
        Integrator.Integrate(this, dt);
        ClearForces();
        ClearTorque();
    }

    /*public void DoFixedUpdate(float dt, PhysicsRigidbody2D childPhysicsRigidbody)
    {
        acceleration = gravity + accumulatedForces * inverseMass;
        Integrator.Integrate(this, dt, childPhysicsRigidbody, Bone);
        ClearForces();
        ClearTorque();
    }*/

    public void ClearForces()
    {
        accumulatedForces = Vector2.zero;
    }

    public void ClearTorque()
    {
        accumulatedTorque = 0;
    }

    public void AddForce(Vector2 force)
    {
        accumulatedForces += force;
    }

    public void AddTorque(float torque)
    {
        accumulatedTorque += torque;
    }
    
    public Vector3 AddTorque(Vector3 radius, Vector3 force)
    {
        Vector3 cross = Vector3.Cross(radius, force);

        accumulatedTorque += cross.z;

        return force.normalized * (force.magnitude - cross.z);
    }

    public Vector3 GetTotalVelocity()
    {
        PhysicsRigidbody2D prb = this;
        Vector3 totalVelocity = Vector3.zero;
        
        while (prb)
        {
            totalVelocity += (Vector3) prb.velocity;
            prb = prb.ParentCapsule;
        }

        return totalVelocity;
    }
}
