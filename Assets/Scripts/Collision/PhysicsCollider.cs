using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCollider : MonoBehaviour
{
    public Particle2D ParentBone;
    
    public float invMass
    {
        get
        {
            Particle2D particle;
            if (TryGetComponent(out particle))
            {
                return particle.inverseMass;
            }

            return 0;
        }
        set
        {
            Particle2D particle;
            if (TryGetComponent(out particle))
            {
                particle.inverseMass = value;
            }
        }
    }

    public Vector3 velocity
    {
        get
        {
            Particle2D particle;
            if (TryGetComponent(out particle))
            {
                return particle.velocity;
            }

            return Vector3.zero;
        }
        set
        {
            Particle2D particle;
            if (TryGetComponent(out particle))
            {
                particle.velocity = value;
            }
        }
    }

    public Vector3 position
    {
        get { return transform.position; }
        set
        {
            Particle2D particle;
            if (TryGetComponent(out particle))
            {
                particle.transform.position = value;
            }
        }
    }
}