using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CapsuleCollider : PhysicsCollider
{
    public float LengthOffset = 0.5f;
    public Vector2 Center => transform.position;
    public float Radius = .5f;

    public Vector2 TopPoint
    {
        get
        {
            return (Vector2) (transform.up * LengthOffset);
        }
    }
    
    public Vector2 BottomPoint
    {
        get
        {
            return -(Vector2) (transform.up * LengthOffset);
        }
    }

    public Vector2 ClosestPoint(Vector2 pos)
    {
        Vector3 distance = pos - Center;

        Vector3 up = transform.up;
        float localLength = Vector3.Dot(distance, up);

        localLength = Mathf.Clamp(localLength, -LengthOffset, LengthOffset);
        
        Vector2 closestPoint = (up * localLength);
        
        return closestPoint + Center;
    }

    public void AddTorque(Vector3 closestPoint, Vector3 force)
    {
        if (!TryGetComponent(out Particle2D particle)) return;
        //Vector2 forceB = normal * (acceleration * (invMass));
            
        particle.AddTorque(closestPoint, force);

        
        /*
        if (Vector2.Dot(normal, transform.right) >= 0)
        {
        }
        else
        {
            particle.AddTorque(closestPoint.magnitude, normal, force);
        }*/
    }
}
