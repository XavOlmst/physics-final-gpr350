using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Integrator
{
    public static void Integrate(PhysicsRigidbody2D physicsRigidbody, float dt)
    {
        // Objects with pivots
        if (physicsRigidbody.Bone)
        {
            // Only the root bone can move as a normal particle
            if(physicsRigidbody.IsRootBone)
            {
                physicsRigidbody.Bone.position += new Vector3(physicsRigidbody.Velocity.x * dt, physicsRigidbody.Velocity.y * dt);
            }

            // Rotate bone with angular velocity
            physicsRigidbody.Bone.Rotate(Vector3.forward, physicsRigidbody.AngularVelocity * dt);

            // Apply a portion of the rotation to children
            if (physicsRigidbody.ChildCapsule.Count > 0)
            {
                foreach (var childCapsule in physicsRigidbody.ChildCapsule)
                {
                    physicsRigidbody.Bone.Rotate(Vector3.forward, childCapsule.AngularVelocity * dt * physicsRigidbody.ImpartRatio);
                    physicsRigidbody.Velocity += childCapsule.Velocity;  
                    childCapsule.Velocity = Vector3.zero;
                }
            }
        }
        else // Normal objects
        {
            physicsRigidbody.transform.position += new Vector3(physicsRigidbody.Velocity.x * dt, physicsRigidbody.Velocity.y * dt);
            physicsRigidbody.transform.Rotate(Vector3.forward, physicsRigidbody.AngularVelocity * dt);
        }

        // Adjust acceleration
        physicsRigidbody.Acceleration = physicsRigidbody.AccumulatedForces * physicsRigidbody.InverseMass + physicsRigidbody.Gravity;
        
        // Adjust angular acceleration
        Debug.Assert(physicsRigidbody.MomentOfInertia > 0);
        physicsRigidbody.AngularAcceleration = physicsRigidbody.AccumulatedTorque / physicsRigidbody.MomentOfInertia;
        
        // Adjust velocity by acceleration
        physicsRigidbody.Velocity += physicsRigidbody.Acceleration * dt;
        physicsRigidbody.Velocity *= Mathf.Pow(physicsRigidbody.Damping, dt);

        // Adjust angular velocity by angular acceleration
        physicsRigidbody.AngularVelocity += physicsRigidbody.AngularAcceleration * dt;
        physicsRigidbody.AngularVelocity *= Mathf.Pow(physicsRigidbody.AngularDamping, dt);
    }
}
