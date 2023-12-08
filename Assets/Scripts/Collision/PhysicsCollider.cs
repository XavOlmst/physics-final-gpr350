using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCollider : MonoBehaviour
{
    public float InverseMass
    {
        get
        {
            if (TryGetComponent(out PhysicsRigidbody2D physicsRigidbody))
            {
                return physicsRigidbody.InverseMass;
            }

            return 0;
        }
        set
        {
            if (TryGetComponent(out PhysicsRigidbody2D physicsRigidbody))
            {
                physicsRigidbody.InverseMass = value;
            }
        }
    }

    public Vector3 Velocity
    {
        get
        {
            if (TryGetComponent(out PhysicsRigidbody2D physicsRigidbody))
            {
                return physicsRigidbody.Velocity;
            }

            return Vector3.zero;
        }
        set
        {
            if (TryGetComponent(out PhysicsRigidbody2D physicsRigidbody))
            {
                physicsRigidbody.Velocity = value;
            }
        }
    }

    public Vector3 Position
    {
        get { return transform.position; }
        set
        {
            if (TryGetComponent(out PhysicsRigidbody2D physicsRigidbody))
            {
                physicsRigidbody.transform.position = value;
            }
        }
    }
}