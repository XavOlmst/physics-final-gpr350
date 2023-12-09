using UnityEngine;

namespace Collision
{
    public class CollisionManager : MonoBehaviour
    {
        private void FixedUpdate()
        {
            CircleCollider[] spheres = FindObjectsOfType<CircleCollider>();
            PlaneCollider[] colliders = FindObjectsOfType<PlaneCollider>();
            CapsuleCollider[] capsules = FindObjectsOfType<CapsuleCollider>();

            for (int i = 0; i < spheres.Length; i++)
            {
                CircleCollider sphere = spheres[i];
                for (int j = i + 1; j < spheres.Length; j++)
                {
                    CircleCollider sphereB = spheres[j];
                    CollisionDetection.GetNormalAndPenetration(sphere, sphereB, out Vector3 normal, out float penetration);
                    Contact contact = new(sphere, sphereB, normal, penetration);
                    CollisionDetection.ApplyCollisionResolution(contact);
                }

                foreach (PlaneCollider planeCollider in colliders)
                {
                    CollisionDetection.GetNormalAndPenetration(sphere, planeCollider, out Vector3 normal, out float penetration);
                    Contact contact = new(sphere, planeCollider, normal, penetration);
                    CollisionDetection.ApplyCollisionResolution(contact);
                }

                foreach (CapsuleCollider capsule in capsules)
                {
                    CollisionDetection.GetNormalAndPenetration(sphere, capsule, out Vector3 normal, out float penetration);
                    Contact contact = new(sphere, capsule, normal, penetration);
                    CollisionDetection.ApplyCollisionResolution(contact);
                }
            }

            for (var i = 0; i < capsules.Length; i++)
            {
                var capsuleA = capsules[i];
            
                for (int j = i + 1; j < capsules.Length; j++)
                {
                    var capsuleB = capsules[j];

                    if (!capsuleA.CompareTag("Ragdoll") || !capsuleB.CompareTag("Ragdoll"))
                    {
                        CollisionDetection.GetNormalAndPenetration(capsuleA, capsuleB, out Vector3 normal, out float penetration);
                        Contact contact = new(capsuleA, capsuleB, normal, penetration);
                        CollisionDetection.ApplyCollisionResolution(contact);
                    }
                }

                foreach (PlaneCollider plane in colliders)
                {
                    CollisionDetection.GetNormalAndPenetration(capsuleA, plane, out Vector3 normal, out float penetration);
                    Contact contact = new(capsuleA, plane, normal, penetration);
                    CollisionDetection.ApplyCollisionResolution(contact);
                }
            }
        }
    }
}
