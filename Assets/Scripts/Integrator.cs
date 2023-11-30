using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Integrator
{
    public static void Integrate(Particle2D particle, float dt)
    {
        particle.transform.position += new Vector3(particle.velocity.x * dt, particle.velocity.y * dt);

        particle.acceleration = particle.accumulatedForces * particle.inverseMass + particle.gravity;
        
        particle.velocity += particle.acceleration * dt;
        particle.velocity *= Mathf.Pow(particle.damping, dt);
    }

    public static void Integrate(float radius, Particle2D particle, float dt)
    {
        particle.transform.position += new Vector3(particle.velocity.x * dt, particle.velocity.y * dt);
        particle.transform.Rotate(Vector3.forward, particle.angularVelocity * dt);

        particle.acceleration = particle.accumulatedForces * particle.inverseMass + particle.gravity;
        
        float momentOfInertia = radius * radius / particle.inverseMass;
        particle.angularAcceleration = particle.accumulatedTorque / momentOfInertia; //TODO: Ask Kevin for help (might be doing right now?)
        
        particle.velocity += particle.acceleration * dt;
        particle.velocity *= Mathf.Pow(particle.damping, dt);

        particle.angularVelocity += particle.angularAcceleration * dt;
        particle.angularVelocity *= Mathf.Pow(particle.angularDamping, dt);
    }
    
    public static void Integrate(float radius, Particle2D particle, float dt, Particle2D childParticle, PivotBone bone)
    {
        particle.transform.position += new Vector3(particle.velocity.x * dt, particle.velocity.y * dt);
        particle.transform.Rotate(Vector3.forward, particle.angularVelocity * dt);

        particle.acceleration = particle.accumulatedForces * particle.inverseMass + particle.gravity;
        
        float momentOfInertia = radius * radius / particle.inverseMass;
        particle.angularAcceleration = particle.accumulatedTorque / momentOfInertia;
        
        particle.velocity += particle.acceleration * dt;
        particle.velocity *= Mathf.Pow(particle.damping, dt);

        particle.angularVelocity += particle.angularAcceleration * dt;
        particle.angularVelocity *= Mathf.Pow(particle.angularDamping, dt);
        
        int childIndex = bone.ChildCapsules.IndexOf(childParticle);
        

        var transformPoint = particle.transform.TransformPoint(childParticle.transform.localPosition);
        bone.ChildCapsules[childIndex].transform.localPosition = bone.ChildLocalPositions[childIndex];
        particle.transform.position = transformPoint + particle.transform.up * bone.ChildLocalPositions[childIndex].magnitude;
        //This is definitely the wrong way to do this
        
        
        /*//Start by setting all the child particle data to the parent bone 
        particle.velocity = childParticle.velocity;
        particle.acceleration = childParticle.acceleration;
        particle.angularVelocity += childParticle.angularVelocity;
        childParticle.angularVelocity = 0;
        //particle.gravity = childParticle.gravity;
        //particle.AddForce(childParticle.accumulatedForces);

        int childIndex = bone.ChildCapsules.IndexOf(childParticle);
        
        bone.ChildCapsules[childIndex].transform.localPosition = bone.ChildLocalPositions[childIndex];
        var transformPoint = particle.transform.TransformPoint(childParticle.transform.localPosition);
        particle.transform.position = transformPoint + particle.transform.up * bone.ChildLocalPositions[childIndex].magnitude;*/
        //This is definitely the wrong way to do this
    }
}
