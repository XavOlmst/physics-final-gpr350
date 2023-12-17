using UnityEngine;

namespace Collision
{
    // having contact information is excellent
    public class Contact
    {
        public Vector3 Normal;
        public readonly float Penetration;
        public readonly PhysicsCollider Collider1;
        public readonly PhysicsCollider Collider2;
        public readonly Vector3 Collider1ContactPoint;
        public readonly Vector3 Collider2ContactPoint;

        public Contact(PhysicsCollider collider1, PhysicsCollider collider2, Vector3 normal, 
            float penetration, Vector3 collider1ContactPoint, Vector3 collider2ContactPoint)
        {
            Collider1 = collider1;
            Collider2 = collider2;
            Normal = normal;
            Penetration = penetration;
            Collider1ContactPoint = collider1ContactPoint;
            Collider2ContactPoint = collider2ContactPoint;
        }
    }

    public static class CollisionDetection
    {
        private const float Restitution = 0.7f;
    
        public static void GetNormalAndPenetration(CircleCollider s1, CircleCollider s2, out Contact contact)
        {
            Vector3 offset = s1.Position - s2.Position;
            Vector3 normal = offset / offset.magnitude;
            float penetration = (s1.Radius + s2.Radius) - offset.magnitude;

            contact = new(s1, s2, normal, penetration, s1.Center, s2.Center);
        }

        public static void GetNormalAndPenetration(CircleCollider s, PlaneCollider p, out Contact contact)
        {
            float distance = Vector3.Dot(s.Position, p.Normal);
            float penetration;
            Vector3 normal;
            
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
            
            contact = new(s, p, normal, penetration, s.Center, p.Normal);
        }

        public static void GetNormalAndPenetration(CapsuleCollider c, PlaneCollider p, out Contact contact)
        {
            Vector3 normal;
            float penetration = 0;
            Vector3 capsuleContactPoint;
            
            float centerDistance = Vector3.Dot(c.Center, p.Normal);
        
            float bottomDistance = centerDistance + Vector3.Dot(c.BottomPoint, p.Normal);
            float topDistance = centerDistance + Vector3.Dot(c.TopPoint, p.Normal);

            float distance;

            // this does work
            if (bottomDistance > topDistance)
            {
                distance = bottomDistance;
                capsuleContactPoint = c.Center + c.BottomPoint;
            }
            else
            {
                distance = topDistance;
                capsuleContactPoint = c.Center + c.TopPoint;
            }
            
            // in your case it would be safer to use half spaces instead
            if (centerDistance >= p.Offset)
            {
                penetration = (c.Radius + p.Offset) - distance;
                normal = p.Normal;
            }
            else
            {
                penetration = (c.Radius - p.Offset) + distance;
                normal = -p.Normal;
            }

            contact = new(c, p, normal, penetration, capsuleContactPoint, p.Normal);
        }
    
        public static void GetNormalAndPenetration(CircleCollider s, CapsuleCollider c, out Contact contact)
        {
            Vector3 normal;
            float penetration = 0;

            // this does work
            Vector2 closestPoint = c.ClosestPoint(s.Center);
            Vector2 offset = (Vector2) s.Center - closestPoint;
            normal = offset.normalized;

            penetration = (s.Radius + c.Radius) - offset.magnitude;

            contact = new(s, c, normal, penetration, s.Center, closestPoint);
        }
    
        public static void GetNormalAndPenetration(CapsuleCollider c1, CapsuleCollider c2, out Contact contact)
        {
            // this does NOT work, but is probably close enough
            // for it to truly work, you would need to iterate until the closestPoint you
            // get stops moving, meaning you found a minimum
            // if you looked in the book or online, you could have found a better solution
            Vector2 c2Closest = c2.ClosestPoint(c1.Center);
            Vector2 c1Closest = c1.ClosestPoint(c2Closest);
            Vector2 offset = c1Closest - c2Closest;
            Vector3 normal = offset.normalized;
            float penetration = (c1.Radius + c2.Radius) - offset.magnitude;

            contact = new(c1, c2, normal, penetration, c1Closest, c2Closest);
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

                // this should be the root if you hit the ragdoll
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
                capsule1.AddTorque(capsule1.ClosestPoint(contact.Collider1ContactPoint) - capsule1.Center, force);
            }
        
            if (contact.Collider2.TryGetComponent(out CapsuleCollider capsule2))
            {
                Vector3 force = contact.Normal * (deltaVelB / Time.deltaTime);
                capsule2.AddTorque(capsule2.ClosestPoint(contact.Collider2ContactPoint) - capsule2.Center, force);
            }
        
            contact.Collider1.Velocity -= deltaVelA * contact.Normal;
            contact.Collider2.Velocity += deltaVelB * contact.Normal;
        }
    }
}