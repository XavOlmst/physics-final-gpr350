using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Plastic.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Assertions;

public static class CollisionDetection
{
    public static void GetNormalAndPenetration(Sphere s1, Sphere s2, out Vector3 normal, out float penetration)
    {
        Vector3 offset = s1.position - s2.position;
        normal = offset / offset.magnitude;
        penetration = (s1.Radius + s2.Radius) - offset.magnitude;
    }

    public static void GetNormalAndPenetration(Sphere s, PlaneCollider p, out Vector3 normal, out float penetration)
    {
        float distance = Vector3.Dot(s.position, p.Normal);
        
        if(distance >= p.Offset)
        {
            penetration = (s.Radius + p.Offset) - distance;
            normal = p.Normal;
        }
        else
        {
            penetration = (s.Radius - p.Offset) + distance;
            normal = -p.Normal;
        }
    }

    public static void GetNormalAndPenetration(PlaneCollider p, CapsuleCollider c, out Vector3 normal, out float penetration)
    {
        //TODO: rework this, not functional
        normal = p.Normal;
        penetration = 0;

        float offsetLength = Mathf.Min(Vector2.Dot(c.TopPoint, p.Normal), Vector2.Dot(c.BottomPoint, p.Normal));
        
        
        
        penetration = c.Radius - offsetLength;
    }
    
    public static void GetNormalAndPenetration(Sphere s, CapsuleCollider c, out Vector3 normal, out float penetration)
    {
        normal = Vector2.zero;
        penetration = 0;

        Vector2 offset = (Vector2) s.Center - c.ClosestPoint(s.Center);
        normal = offset.normalized;

        penetration = (s.Radius + c.Radius) - offset.magnitude;

    }
    
    public static void ApplyCollisionResolution(Sphere s1, Sphere s2)
    {
        GetNormalAndPenetration(s1, s2, out Vector3 contactNormal, out float penetration);
        
        //Resolve interpenetration
        float totalInverseMass = s1.invMass + s2.invMass;
        float inverseTotalInverseMass = 1 / (totalInverseMass + 0.00000001f);

        if (penetration < 0)
        {
            return;
        }
        
        s1.position += contactNormal * penetration * s1.invMass * inverseTotalInverseMass;
        s2.position -= contactNormal * penetration * s2.invMass * inverseTotalInverseMass;
        
        
        Vector3 relativeVelocity = (s2.velocity - s1.velocity);
        
        float closingVelocity = Vector3.Dot(relativeVelocity, contactNormal);

        if (closingVelocity < 0.0f)
        {
            return;
        }

        const float restitution = 1;
        float newClosingVelocity = -closingVelocity * restitution;
        float deltaClosingVelocity = newClosingVelocity - closingVelocity;

        float deltaVelA = deltaClosingVelocity * inverseTotalInverseMass * s1.invMass;
        float deltaVelB = deltaClosingVelocity * inverseTotalInverseMass * s2.invMass;

        s1.velocity -= deltaVelA * contactNormal;
        s2.velocity += deltaVelB * contactNormal;
        
        if(s2.TryGetComponent(out Particle2D particle))
        {
            s1.Invoke(s2, particle);
        }
    
        if(s1.TryGetComponent(out particle))
        {
            s2.Invoke(s1, particle);
        }
    }

    public static void ApplyCollisionResolution(Sphere s, PlaneCollider p)
    {
        GetNormalAndPenetration(s, p, out Vector3 normal, out float penetration);

        if (penetration < 0)
        {
            return;
        }

        float totalInverseMass = p.invMass + s.invMass;
        float inverseTotalInvertedMass = 1.0f / (totalInverseMass + Mathf.Epsilon);
        float deltaPosA = penetration * s.invMass * inverseTotalInvertedMass;
        float deltaPosB = penetration * p.invMass * inverseTotalInvertedMass;

        s.position += deltaPosA * normal;
        p.position -= deltaPosB * normal;
        
        
        Vector3 relativeVelocity = (p.velocity - s.velocity);
        
        float closingVelocity = Vector3.Dot(relativeVelocity, normal);

        if (closingVelocity < 0.0f)
        {
            return;
        }

        const float restitution = 1;
        float newClosingVelocity = -closingVelocity * restitution;
        float deltaClosingVelocity = newClosingVelocity - closingVelocity;

        float deltaVelA = deltaClosingVelocity * inverseTotalInvertedMass * s.invMass;
        float deltaVelB = deltaClosingVelocity * inverseTotalInvertedMass * p.invMass;

        s.velocity -= deltaVelA * normal;
        p.velocity += deltaVelB * normal;
        
    }
    
    public static void ApplyCollisionResolution(Sphere s, CapsuleCollider c)
    {
        GetNormalAndPenetration(s, c, out Vector3 normal, out float penetration);

        if (penetration < 0)
        {
            return;
        }

        float totalInverseMass = c.invMass + s.invMass;
        float inverseTotalInvertedMass = 1.0f / (totalInverseMass + Mathf.Epsilon);
        float deltaPosA = penetration * s.invMass * inverseTotalInvertedMass;
        float deltaPosB = penetration * c.invMass * inverseTotalInvertedMass;

        s.position += deltaPosA * normal;
        c.position -= deltaPosB * normal;
        
        
        Vector3 relativeVelocity = (c.velocity - s.velocity);
        
        float closingVelocity = Vector3.Dot(relativeVelocity, normal);

        if (closingVelocity < 0.0f)
        {
            return;
        }

        const float restitution = 1;
        float newClosingVelocity = -closingVelocity * restitution;
        float deltaClosingVelocity = newClosingVelocity - closingVelocity;

        float deltaVelA = deltaClosingVelocity * inverseTotalInvertedMass * s.invMass;
        float deltaVelB = deltaClosingVelocity * inverseTotalInvertedMass * c.invMass;

        s.velocity -= deltaVelA * normal;
        c.velocity += deltaVelB * normal;
    }
    
    public static void ApplyCollisionResolution(PlaneCollider p, CapsuleCollider c)
    {
        GetNormalAndPenetration(p, c, out Vector3 normal, out float penetration);

        if (penetration < 0)
        {
            return;
        }

        float totalInverseMass = c.invMass + p.invMass;
        float inverseTotalInvertedMass = 1.0f / (totalInverseMass + Mathf.Epsilon);
        float deltaPosA = penetration * p.invMass * inverseTotalInvertedMass;
        float deltaPosB = penetration * c.invMass * inverseTotalInvertedMass;

        p.position += deltaPosA * normal;
        c.position -= deltaPosB * normal;
        
        
        Vector3 relativeVelocity = (c.velocity - p.velocity);
        
        float closingVelocity = Vector3.Dot(relativeVelocity, normal);

        if (closingVelocity < 0.0f)
        {
            return;
        }

        const float restitution = 1;
        float newClosingVelocity = -closingVelocity * restitution;
        float deltaClosingVelocity = newClosingVelocity - closingVelocity;

        float deltaVelA = deltaClosingVelocity * inverseTotalInvertedMass * p.invMass;
        float deltaVelB = deltaClosingVelocity * inverseTotalInvertedMass * c.invMass;

        p.velocity -= deltaVelA * normal;
        c.velocity += deltaVelB * normal;
    }
}
