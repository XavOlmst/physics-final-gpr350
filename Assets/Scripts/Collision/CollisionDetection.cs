using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Plastic.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class Contact
{
    public Vector3 Normal;
    public float Penetration;
    public PhysicsCollider Collider1;
    public PhysicsCollider Collider2;

    public Contact(PhysicsCollider collider1, PhysicsCollider collider2, Vector3 normal, float penetration)
    {
        Collider1 = collider1;
        Collider2 = collider2;
        Normal = normal;
        Penetration = penetration;
    }
}


public static class CollisionDetection
{
    const float restitution = 0.5f;
    
    public static void GetNormalAndPenetration(CircleCollider s1, CircleCollider s2, out Vector3 normal, out float penetration)
    {
        Vector3 offset = s1.position - s2.position;
        normal = offset / offset.magnitude;
        penetration = (s1.Radius + s2.Radius) - offset.magnitude;
    }

    public static void GetNormalAndPenetration(CircleCollider s, PlaneCollider p, out Vector3 normal, out float penetration)
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

    public static void GetNormalAndPenetration(CapsuleCollider c, PlaneCollider p, out Vector3 normal, out float penetration)
    {
        //TODO: rework this, not functional
        normal = Vector3.zero;
        penetration = 0;

        float centerDistance = Vector3.Dot(c.Center, p.Normal);
        
        float bDist = centerDistance + Vector3.Dot(c.BottomPoint, p.Normal);
        float tDist = centerDistance + Vector3.Dot(c.TopPoint, p.Normal);

        if (centerDistance >= p.Offset)
        {
            float bottomDistance = (c.Radius + p. Offset) - bDist;
            float topDistance = (c.Radius + p.Offset) - tDist;
        
            penetration = Mathf.Max(bottomDistance, topDistance);
            normal = p.Normal;
        }
        else
        {
            float bottomDistance = (c.Radius - p. Offset) + bDist;
            float topDistance = (c.Radius - p.Offset) + tDist;
        
            penetration = Mathf.Max(bottomDistance, topDistance);
            normal = -p.Normal;
        }
    }
    
    public static void GetNormalAndPenetration(CircleCollider s, CapsuleCollider c, out Vector3 normal, out float penetration)
    {
        normal = Vector2.zero;
        penetration = 0;

        Vector2 closestPoint = c.ClosestPoint(s.Center);
        Vector2 offset = (Vector2) s.Center - closestPoint;
        normal = offset.normalized;

        penetration = (s.Radius + c.Radius) - offset.magnitude;
    }
    
    public static void GetNormalAndPenetration(CapsuleCollider c1, CapsuleCollider c2, out Vector3 normal, out float penetration)
    {
        Vector2 c2Closest = c2.ClosestPoint(c1.Center);
        Vector2 c1Closest = c1.ClosestPoint(c2Closest);
        Vector2 offset = c1Closest - c2Closest;
        normal = offset.normalized;
        penetration = (c1.Radius + c2.Radius) - offset.magnitude;
    }


    public static void ApplyCollisionResolution(Contact contact)
    {
        if (contact.Penetration < 0)
        {
            return;
        }
        
        float totalInverseMass = contact.Collider1.invMass + contact.Collider2.invMass;
        float inverseTotalInvertedMass = 1.0f / (totalInverseMass + Mathf.Epsilon);
        float deltaPosA = contact.Penetration * contact.Collider1.invMass * inverseTotalInvertedMass;
        float deltaPosB = contact.Penetration * contact.Collider2.invMass * inverseTotalInvertedMass;
    
        contact.Collider1.position += deltaPosA * contact.Normal;
        contact.Collider2.position -= deltaPosB * contact.Normal;

        Vector3 relativeVelocity = (contact.Collider2.velocity - contact.Collider1.velocity);
        float closingVelocity = Vector3.Dot(relativeVelocity, contact.Normal);

        if (closingVelocity < 0.0f)
        {
            return;
        }
        
        float newClosingVelocity = -closingVelocity * restitution;
        float deltaClosingVelocity = newClosingVelocity - closingVelocity;

        float deltaVelA = deltaClosingVelocity * inverseTotalInvertedMass * contact.Collider1.invMass;
        float deltaVelB = deltaClosingVelocity * inverseTotalInvertedMass * contact.Collider2.invMass;

        if (contact.Collider1.TryGetComponent(out CapsuleCollider capsule1))
        {
            Vector3 force = contact.Normal * (deltaVelA / Time.deltaTime);
            capsule1.AddTorque(capsule1.LocalClosestPoint(contact.Normal), force);
        }
        
        if (contact.Collider2.TryGetComponent(out CapsuleCollider capsule2))
        {
            Vector3 force = contact.Normal * (deltaVelA / Time.deltaTime);
            capsule2.AddTorque(capsule2.LocalClosestPoint(contact.Normal), force);
        }
        
        /*contact.Collider1.AddForce(-contact.Normal * (deltaVelA / Time.deltaTime));
        contact.Collider2.AddForce(contact.Normal * (deltaVelB / Time.deltaTime));*/
        
        contact.Collider1.velocity -= deltaVelA * contact.Normal;
        contact.Collider2.velocity += deltaVelB * contact.Normal;
    }

    /*
    public static void ApplyCollisionResolution(CircleCollider s1, CircleCollider s2)
    {
        GetNormalAndPenetration(s1, s2, out Vector3 normal, out float penetration);
        
        //Resolve interpenetration
        if (penetration < 0)
        {
            return;
        }
        
        
        float totalInverseMass = s1.invMass + s2.invMass;
        float inverseTotalInvertedMass = 1.0f / (totalInverseMass + Mathf.Epsilon);
        float deltaPosA = penetration * s1.invMass * inverseTotalInvertedMass;
        float deltaPosB = penetration * s2.invMass * inverseTotalInvertedMass;

        s1.position += deltaPosA * normal;
        s2.position -= deltaPosB * normal;
        
        
        Vector3 relativeVelocity = (s2.velocity - s1.velocity);
        
        float closingVelocity = Vector3.Dot(relativeVelocity, normal);

        if (closingVelocity < 0.0f)
        {
            return;
        }


        float newClosingVelocity = -closingVelocity * restitution;
        float deltaClosingVelocity = newClosingVelocity - closingVelocity;

        float deltaVelA = deltaClosingVelocity * inverseTotalInvertedMass * s1.invMass;
        float deltaVelB = deltaClosingVelocity * inverseTotalInvertedMass * s2.invMass;

        
        
        s1.velocity -= deltaVelA * normal;
        s2.velocity += deltaVelB * normal;
    }

    
    
    public static void ApplyCollisionResolution(CircleCollider s, PlaneCollider p)
    {
        GetNormalAndPenetration(s, p, out Vector3 normal, out float penetration);

        //Need for collisions
        /*
         normal
         penetration
         particle 1 (physics collider maybe?)
         particle 2
         #1#
        
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
        
        float newClosingVelocity = -closingVelocity * restitution;
        float deltaClosingVelocity = newClosingVelocity - closingVelocity;

        float deltaVelA = deltaClosingVelocity * inverseTotalInvertedMass * s.invMass;
        float deltaVelB = deltaClosingVelocity * inverseTotalInvertedMass * p.invMass;

        s.velocity -= deltaVelA * normal;
        p.velocity += deltaVelB * normal;
        
    }
    
    public static void ApplyCollisionResolution(CircleCollider s, CapsuleCollider c)
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

        //const float restitution = 0.7f;
        float newClosingVelocity = -closingVelocity * restitution;
        float deltaClosingVelocity = newClosingVelocity - closingVelocity;

        float deltaVelA = deltaClosingVelocity * inverseTotalInvertedMass * s.invMass;
        float deltaVelB = deltaClosingVelocity * inverseTotalInvertedMass * c.invMass;

        if (c.TryGetComponent(out Particle2D particle))
        {
            Vector2 closestPoint = c.ClosestPoint(s.Center) - c.Center;
            Vector2 forceB = normal * (deltaVelB * (totalInverseMass));
            
            if (Vector2.Dot(normal, c.transform.right) >= 0)
            {
                particle.AddTorque(closestPoint.magnitude, normal, forceB.magnitude);
            }
            else
            {
                particle.AddTorque(closestPoint.magnitude, normal, forceB.magnitude);
            }
        }
        
        s.velocity -= deltaVelA * normal;
        c.velocity += deltaVelB * normal;
    }
    
    public static void ApplyCollisionResolution(PlaneCollider p, CapsuleCollider c)
    {
        GetNormalAndPenetration(c, p, out Vector3 normal, out float penetration);
        
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

        //const float restitution = 0.7f;
        float newClosingVelocity = -closingVelocity * restitution;
        float deltaClosingVelocity = newClosingVelocity - closingVelocity;

        float deltaVelA = deltaClosingVelocity * inverseTotalInvertedMass * p.invMass;
        float deltaVelB = deltaClosingVelocity * inverseTotalInvertedMass * c.invMass;
        
        if (c.TryGetComponent(out Particle2D particle))
        {
            Vector2 closestPoint = c.ClosestPoint(p.Normal) - c.Center;
            Vector2 forceB = normal * (deltaVelB * (totalInverseMass));

            //float sinAngle = Vector2.Dot(normal, c.transform.up);
            //float sinAngle = dotProduct / normal.magnitude;
            
            if (Vector2.Dot(normal, c.transform.right) >= 0)
            {
                particle.AddTorque(closestPoint.magnitude, normal, forceB.magnitude);
            }
            else
            {
                particle.AddTorque(closestPoint.magnitude, normal, forceB.magnitude);
            }
        }
        
        p.velocity -= deltaVelA * normal;
        c.velocity += deltaVelB * normal;
    }
    
     public static void ApplyCollisionResolution(CapsuleCollider c1, CapsuleCollider c2)
    {
        GetNormalAndPenetration(c1, c2, out Vector3 normal, out float penetration);

        if (penetration < 0)
        {
            return;
        }

        float totalInverseMass = c2.invMass + c1.invMass;
        float inverseTotalInvertedMass = 1.0f / (totalInverseMass + Mathf.Epsilon);
        float deltaPosA = penetration * c1.invMass * inverseTotalInvertedMass;
        float deltaPosB = penetration * c2.invMass * inverseTotalInvertedMass;

        c1.position += deltaPosA * normal;
        c2.position -= deltaPosB * normal;
        
        
        Vector3 relativeVelocity = (c2.velocity - c1.velocity);
        
        float closingVelocity = Vector3.Dot(relativeVelocity, normal);

        if (closingVelocity < 0.0f)
        {
            return;
        }

        //const float restitution = 0.7f;
        float newClosingVelocity = -closingVelocity * restitution;
        float deltaClosingVelocity = newClosingVelocity - closingVelocity;

        float deltaVelA = deltaClosingVelocity * inverseTotalInvertedMass * c1.invMass;
        float deltaVelB = deltaClosingVelocity * inverseTotalInvertedMass * c2.invMass;

        if (c2.TryGetComponent(out Particle2D particle))
        {
            Vector2 closestPoint = c2.ClosestPoint(c1.Center) - c2.Center;
            Vector2 forceB = normal * (deltaVelB * (totalInverseMass));

            //float sinAngle = Vector2.Dot(normal, c.transform.up);
            //float sinAngle = dotProduct / normal.magnitude;
            
            if (Vector2.Dot(normal, c2.transform.right) >= 0)
            {
                particle.AddTorque(closestPoint.magnitude, normal, forceB.magnitude);
            }
            else
            {
                particle.AddTorque(closestPoint.magnitude, normal, forceB.magnitude);
            }
        }
        
        c1.velocity -= deltaVelA * normal;
        c2.velocity += deltaVelB * normal;
    }*/
}
