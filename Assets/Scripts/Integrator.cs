using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Integrator
{
    public static void Integrate(PhysicsRigidbody2D physicsRigidbody, float dt)
    {
        if (physicsRigidbody.Bone)
        {
            if(physicsRigidbody.IsRootBone)
            {
                physicsRigidbody.Bone.position += new Vector3(physicsRigidbody.Velocity.x * dt, physicsRigidbody.Velocity.y * dt);
            }

            physicsRigidbody.Bone.Rotate(Vector3.forward, physicsRigidbody.AngularVelocity * dt);

            if (physicsRigidbody.ChildCapsule)
            {
                physicsRigidbody.Bone.Rotate(Vector3.forward, physicsRigidbody.ChildCapsule.AngularVelocity * dt * physicsRigidbody.ImpartRatio);
                physicsRigidbody.Velocity += physicsRigidbody.ChildCapsule.Velocity;
                physicsRigidbody.ChildCapsule.Velocity = Vector3.zero;
            }
        }
        else
        {
            physicsRigidbody.transform.position += new Vector3(physicsRigidbody.Velocity.x * dt, physicsRigidbody.Velocity.y * dt);
            physicsRigidbody.transform.Rotate(Vector3.forward, physicsRigidbody.AngularVelocity * dt);
        }

        physicsRigidbody.Acceleration = physicsRigidbody.AccumulatedForces * physicsRigidbody.InverseMass + physicsRigidbody.Gravity;
        
        Debug.Assert(physicsRigidbody.MomentOfInertia > 0);
        physicsRigidbody.AngularAcceleration = physicsRigidbody.AccumulatedTorque / physicsRigidbody.MomentOfInertia;
        
        physicsRigidbody.Velocity += physicsRigidbody.Acceleration * dt;
        physicsRigidbody.Velocity *= Mathf.Pow(physicsRigidbody.Damping, dt);

        physicsRigidbody.AngularVelocity += physicsRigidbody.AngularAcceleration * dt;
        physicsRigidbody.AngularVelocity *= Mathf.Pow(physicsRigidbody.AngularDamping, dt);
    }
}
