using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneCollider : PhysicsCollider
{
    public Vector3 Normal => transform.up;

    public float Offset
    {
        get
        {
            Vector3 n = Normal;
            float d = Vector3.Dot(n, transform.position);
            return d;
        }
    }
}
