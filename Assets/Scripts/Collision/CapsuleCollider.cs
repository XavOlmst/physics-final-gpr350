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
        float localLengthX = Vector3.Dot(distance, up);
        float localLengthY = Vector3.Dot(distance, up);

        float uLocalClosest = BottomPoint.x < TopPoint.x ? Mathf.Clamp(localLengthX, BottomPoint.x, TopPoint.x)
            : Mathf.Clamp(localLengthX, TopPoint.x, BottomPoint.x);

        float vLocalClosest = BottomPoint.y < TopPoint.y ? Mathf.Clamp(localLengthY, BottomPoint.y, TopPoint.y)
            : Mathf.Clamp(localLengthY, TopPoint.y, BottomPoint.y);
        
        Vector2 closestPoint = up * uLocalClosest + up * vLocalClosest;
        
        return closestPoint + Center;
    }
}
