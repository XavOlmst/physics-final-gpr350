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
}
