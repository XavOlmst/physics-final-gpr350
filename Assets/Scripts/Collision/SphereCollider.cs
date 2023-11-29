using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCollider : PhysicsCollider
{
    public Vector3 Center => transform.position;
    public float Radius = .5f;
}
