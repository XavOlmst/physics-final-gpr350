using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Integrator
{
    public static void Integrate(PhysicsRigidbody2D physicsRigidbody, float dt)
    {
        // Overall this logic works
        // But I think you should have treated the root a a regular object and just handled it as a rigid body
        // Collisions inside the ragdoll could have solely angular velocity/acceleration and rotation
        // the only difference is that  1 - it does not move linearly
        //                              2 - it has to rotate around a pivot
        // there was an opportunity here for cleaner design
        // something like IntegrateLinear() and IntegrateAngular()
        // IntegrateLinear() can be done on everything including the root of the ragdoll
        // IntegrateAngular() can be generalized to rotate always around a pivot (that is generally 0)
        // Rotation result could be applied on the pivot later on.

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
        Debug.Assert(physicsRigidbody.MomentOfInertia > 0); // good use of assert
        physicsRigidbody.AngularAcceleration = physicsRigidbody.AccumulatedTorque / physicsRigidbody.MomentOfInertia;
        
        // Adjust velocity by acceleration
        physicsRigidbody.Velocity += physicsRigidbody.Acceleration * dt;
        physicsRigidbody.Velocity *= Mathf.Pow(physicsRigidbody.Damping, dt);

        // Adjust angular velocity by angular acceleration
        physicsRigidbody.AngularVelocity += physicsRigidbody.AngularAcceleration * dt;
        physicsRigidbody.AngularVelocity *= Mathf.Pow(physicsRigidbody.AngularDamping, dt);
    }
}
