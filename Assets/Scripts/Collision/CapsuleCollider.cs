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
            return -(Vector2) (transform.up * LengthOffset);
        }
    }
    
    public Vector2 BottomPoint
    {
        get
        {
            return (Vector2) (transform.up * LengthOffset);
        }
    }

    public Vector2 ClosestPoint(Vector2 pos)
    {
        Vector2 closestPoint = new();
        
        Vector3 distance = pos - Center;

        float localLengthX = Vector3.Dot(distance, transform.up);
        float localLengthY = Vector3.Dot(distance, transform.up);
        
        float uLocalClosest = Mathf.Clamp(localLengthX, BottomPoint.x, TopPoint.x);
        float vLocalClosest = Mathf.Clamp(localLengthY, BottomPoint.y, TopPoint.y);

        closestPoint = uLocalClosest * transform.up + vLocalClosest * transform.up;
        
        return closestPoint + Center;
    }
}
