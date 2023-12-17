using System;
using UnityEngine;

namespace Collision
{
    public class CollisionManager : MonoBehaviour
    {
        private PlaneCollider[] _colliders;

        private void Start()
        {
            // good thing to compute them only once since they do not change
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
                    // this is good
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

                    // using a tag is not the best solution but it works here
                    // usually you would collide the ragdoll a whole with the outside world
                    // so by default there would not be internal collisions
                    // you would have something like "CollisionGroup" that contains the ragdoll colls
                    // then you COULD (that's what I tend to do) and have "internal" collisions
                    // and when going through the hierarchy avoid the test with direct children (for example)

                    // for your case, what you did works

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
                    // this is good too
                    CollisionDetection.GetNormalAndPenetration(capsuleA, plane, out Contact contact);
                    CollisionDetection.ApplyCollisionResolution(contact);
                }
            }
        }
    }
}
