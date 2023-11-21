using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneCollider : PhysicsCollider
{
    public Vector3 Normal
    {
        get
        {
            return transform.up;
        }
    }
    public float Offset
    {
        get
        {
            return transform.position.magnitude;
        }
    }
}
