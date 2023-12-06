using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCollider : MonoBehaviour
{
    public float invMass
    {
        get
        {
            PhysicsRigidbody2D physicsRigidbody;
            if (TryGetComponent(out physicsRigidbody))
            {
                return physicsRigidbody.inverseMass;
            }

            return 0;
        }
        set
        {
            PhysicsRigidbody2D physicsRigidbody;
            if (TryGetComponent(out physicsRigidbody))
            {
                physicsRigidbody.inverseMass = value;
            }
        }
    }

    public Vector3 velocity
    {
        get
        {
            PhysicsRigidbody2D physicsRigidbody;
            if (TryGetComponent(out physicsRigidbody))
            {
                return physicsRigidbody.velocity;
            }

            return Vector3.zero;
        }
        set
        {
            PhysicsRigidbody2D physicsRigidbody;
            if (TryGetComponent(out physicsRigidbody))
            {
                physicsRigidbody.velocity = value;
            }
        }
    }

    public Vector3 position
    {
        get { return transform.position; }
        set
        {
            PhysicsRigidbody2D physicsRigidbody;
            if (TryGetComponent(out physicsRigidbody))
            {
                physicsRigidbody.transform.position = value;
            }
        }
    }

    public void AddForce(Vector3 force)
    {
        if (TryGetComponent(out PhysicsRigidbody2D particle))
        {
            particle.AddForce(force);
        }
    }
}