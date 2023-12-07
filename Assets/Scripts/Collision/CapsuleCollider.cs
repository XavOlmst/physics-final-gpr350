using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class CapsuleCollider : PhysicsCollider
{
    public float LengthOffset = 0.5f;
    public Vector3 Center => transform.position;
    public float Radius = .5f;
    public PivotBone ParentBone;
    public List<PivotBone> ChildBones; 

    public Vector2 TopPoint => (transform.up * LengthOffset);

    public Vector2 BottomPoint => -TopPoint;

    public Vector3 ClosestPoint(Vector3 pos)
    {
        Vector3 distance = pos - Center;

        Vector3 up = transform.up;

        float localLength = Vector3.Dot(distance, up);

        localLength = Mathf.Clamp(localLength, -LengthOffset, LengthOffset);
        
        Vector3 closestPoint = (up * localLength);
        
        return closestPoint + Center;
    }

    public Vector3 LocalClosestPoint(Vector3 normalizedVector)
    {
        Vector3 up = transform.up;

        float localLength = Vector3.Dot(normalizedVector, up);

        localLength = Mathf.Clamp(localLength, -LengthOffset, LengthOffset);
        
        return (up * localLength);
    }
    
    public Vector3 AddTorque(Vector3 closestPoint, Vector3 force)
    {
        /*if (ParentBone != null)
        {
            return ParentBone.AddTorque(closestPoint + Center, force);
        }*/

        if (TryGetComponent(out PhysicsRigidbody2D particle))
        {
            
            
            return particle.Bone ? particle.AddTorque((Center - particle.Bone.position) - closestPoint, force) 
                : particle.AddTorque(closestPoint, force);
        }

        return force;
    }
}
