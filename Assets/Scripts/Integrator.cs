using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Integrator
{
    /*public static void Integrate(Particle2D particle, float dt)
    {
        particle.transform.position += new Vector3(particle.velocity.x * dt, particle.velocity.y * dt);

        particle.acceleration = particle.accumulatedForces * particle.inverseMass + particle.gravity;
        
        particle.velocity += particle.acceleration * dt;
        particle.velocity *= Mathf.Pow(particle.damping, dt);
    }*/

    public static void Integrate(PhysicsRigidbody2D physicsRigidbody, float dt)
    {
        if (physicsRigidbody.Bone)
        {
            if(physicsRigidbody.IsRootBone)
            {
                physicsRigidbody.Bone.position += new Vector3(physicsRigidbody.velocity.x * dt, physicsRigidbody.velocity.y * dt);
            }

            physicsRigidbody.Bone.Rotate(Vector3.forward, physicsRigidbody.angularVelocity * dt);
        }
        else
        {
            physicsRigidbody.transform.position += new Vector3(physicsRigidbody.velocity.x * dt, physicsRigidbody.velocity.y * dt);
            physicsRigidbody.transform.Rotate(Vector3.forward, physicsRigidbody.angularVelocity * dt);
        }

        physicsRigidbody.acceleration = physicsRigidbody.accumulatedForces * physicsRigidbody.inverseMass + physicsRigidbody.gravity;
        
        Debug.Assert(physicsRigidbody.momentOfInertia > 0);
        physicsRigidbody.angularAcceleration = physicsRigidbody.accumulatedTorque / physicsRigidbody.momentOfInertia;
        
        physicsRigidbody.velocity += physicsRigidbody.acceleration * dt;
        physicsRigidbody.velocity *= Mathf.Pow(physicsRigidbody.damping, dt);

        physicsRigidbody.angularVelocity += physicsRigidbody.angularAcceleration * dt;
        physicsRigidbody.angularVelocity *= Mathf.Pow(physicsRigidbody.angularDamping, dt);
    }
    
    public static void Integrate(PhysicsRigidbody2D physicsRigidbody, float dt, PhysicsRigidbody2D childPhysicsRigidbody, PivotBone bone)
    {
        physicsRigidbody.transform.position += new Vector3(physicsRigidbody.velocity.x * dt, physicsRigidbody.velocity.y * dt);
        physicsRigidbody.transform.Rotate(Vector3.forward, physicsRigidbody.angularVelocity * dt);

        physicsRigidbody.acceleration = physicsRigidbody.accumulatedForces * physicsRigidbody.inverseMass + physicsRigidbody.gravity;
        
        physicsRigidbody.angularAcceleration = physicsRigidbody.accumulatedTorque / physicsRigidbody.momentOfInertia;
        
        physicsRigidbody.velocity += physicsRigidbody.acceleration * dt;
        physicsRigidbody.velocity *= Mathf.Pow(physicsRigidbody.damping, dt);

        physicsRigidbody.angularVelocity += physicsRigidbody.angularAcceleration * dt;
        physicsRigidbody.angularVelocity *= Mathf.Pow(physicsRigidbody.angularDamping, dt);
        
        int childIndex = bone.ChildParticles.IndexOf(childPhysicsRigidbody);
        
        var transformPoint = physicsRigidbody.transform.TransformPoint(childPhysicsRigidbody.transform.localPosition);
        bone.ChildParticles[childIndex].transform.localPosition = bone.ChildLocalPositions[childIndex];
        physicsRigidbody.transform.position = transformPoint + physicsRigidbody.transform.up * bone.ChildLocalPositions[childIndex].magnitude;
    }
}
