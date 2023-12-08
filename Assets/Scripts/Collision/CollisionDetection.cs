using UnityEngine;

namespace Collision
{
    public class Contact
    {
        public Vector3 Normal;
        public readonly float Penetration;
        public readonly PhysicsCollider Collider1;
        public readonly PhysicsCollider Collider2;

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
        private const float Restitution = 0.7f;
    
        public static void GetNormalAndPenetration(CircleCollider s1, CircleCollider s2, out Vector3 normal, out float penetration)
        {
            Vector3 offset = s1.Position - s2.Position;
            normal = offset / offset.magnitude;
            penetration = (s1.Radius + s2.Radius) - offset.magnitude;
        }

        public static void GetNormalAndPenetration(CircleCollider s, PlaneCollider p, out Vector3 normal, out float penetration)
        {
            float distance = Vector3.Dot(s.Position, p.Normal);
        
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
        
            float totalInverseMass = contact.Collider1.InverseMass + contact.Collider2.InverseMass;
            float inverseTotalInvertedMass = 1.0f / (totalInverseMass + Mathf.Epsilon);
        
            float deltaPosA = contact.Penetration * contact.Collider1.InverseMass * inverseTotalInvertedMass;
            float deltaPosB = contact.Penetration * contact.Collider2.InverseMass * inverseTotalInvertedMass;

            Vector3 contact1Vel;
            Vector3 contact2Vel;
        
            if (contact.Collider1.TryGetComponent(out PhysicsRigidbody2D rb1) && rb1.Bone)
            {
                if(rb1.IsRootBone)
                    rb1.Bone.position += deltaPosA * contact.Normal;

                contact1Vel = rb1.GetTotalVelocity();
            }
            else
            {
                contact.Collider1.Position += deltaPosA * contact.Normal;
                contact1Vel = contact.Collider1.Velocity;
            }

            if (contact.Collider2.TryGetComponent(out PhysicsRigidbody2D rb2) && rb2.Bone)
            {
                if(rb2.IsRootBone)
                    rb2.Bone.position -= deltaPosB * contact.Normal;
            
                contact2Vel = rb2.GetTotalVelocity();
            }
            else
            {
                contact.Collider2.Position -= deltaPosB * contact.Normal;
                contact2Vel = contact.Collider2.Velocity;
            }
        
            Vector3 relativeVelocity = (contact2Vel - contact1Vel);
            float closingVelocity = Vector3.Dot(relativeVelocity, contact.Normal);

            if (closingVelocity < 0.0f)
            {
                return;
            }
        
            float newClosingVelocity = -closingVelocity * Restitution;
            float deltaClosingVelocity = newClosingVelocity - closingVelocity;
        
            float deltaVelA = deltaClosingVelocity * inverseTotalInvertedMass * contact.Collider1.InverseMass;
            float deltaVelB = deltaClosingVelocity * inverseTotalInvertedMass * contact.Collider2.InverseMass;
        
            if (contact.Collider1.TryGetComponent(out CapsuleCollider capsule1))
            {
                Vector3 force = -contact.Normal * (deltaVelA / Time.deltaTime);
                capsule1.AddTorque(capsule1.LocalClosestPoint(contact.Normal), force);
            }
        
            if (contact.Collider2.TryGetComponent(out CapsuleCollider capsule2))
            {
                Vector3 force = contact.Normal * (deltaVelB / Time.deltaTime);
                capsule2.AddTorque(capsule2.LocalClosestPoint(contact.Normal), force);
            }
        
            contact.Collider1.Velocity -= deltaVelA * contact.Normal;
            contact.Collider2.Velocity += deltaVelB * contact.Normal;
        }
    }
}