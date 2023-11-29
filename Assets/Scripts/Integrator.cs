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
}
