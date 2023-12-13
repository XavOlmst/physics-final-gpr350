using System;
using UnityEngine;

namespace Collision
{
    public class CollisionManager : MonoBehaviour
    {
        private PlaneCollider[] _colliders;

        private void Start()
        {
            _colliders = FindObjectsOfType<PlaneCollider>();
        }

        private void FixedUpdate()
        {
            CircleCollider[] spheres = FindObjectsOfType<CircleCollider>();
            CapsuleCollider[] capsules = FindObjectsOfType<CapsuleCollider>();

            for (int i = 0; i < spheres.Length; i++)
            {
                CircleCollider sphere = spheres[i];
                
                // Sphere on sphere
                for (int j = i + 1; j < spheres.Length; j++)
                {
                    CircleCollider sphereB = spheres[j];
                    CollisionDetection.GetNormalAndPenetration(sphere, sphereB, out Contact contact);
                    CollisionDetection.ApplyCollisionResolution(contact);
                }
                
                // Sphere on plane
                foreach (PlaneCollider planeCollider in _colliders)
                {
                    CollisionDetection.GetNormalAndPenetration(sphere, planeCollider, out Contact contact);
                    CollisionDetection.ApplyCollisionResolution(contact);
                }

                // Sphere on capsule
                foreach (CapsuleCollider capsule in capsules)
                {
                    CollisionDetection.GetNormalAndPenetration(sphere, capsule, out Contact contact);
                    CollisionDetection.ApplyCollisionResolution(contact);
                }
            }

            for (var i = 0; i < capsules.Length; i++)
            {
                var capsuleA = capsules[i];
            
                // Capsule on capsule
                for (int j = i + 1; j < capsules.Length; j++)
                {
                    var capsuleB = capsules[j];

                    // Make sure the ragdoll cannot collide with itself
                    if (!capsuleA.CompareTag("Ragdoll") || !capsuleB.CompareTag("Ragdoll"))
                    {
                        CollisionDetection.GetNormalAndPenetration(capsuleA, capsuleB, out Contact contact);
                        CollisionDetection.ApplyCollisionResolution(contact);
                    }
                }

                // Capsule on plane
                foreach (PlaneCollider plane in _colliders)
                {
                    CollisionDetection.GetNormalAndPenetration(capsuleA, plane, out Contact contact);
                    CollisionDetection.ApplyCollisionResolution(contact);
                }
            }
        }
    }
}
