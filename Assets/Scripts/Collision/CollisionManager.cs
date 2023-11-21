using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private void FixedUpdate()
    {
        Sphere[] spheres = FindObjectsOfType<Sphere>();
        PlaneCollider[] colliders = FindObjectsOfType<PlaneCollider>();
        CapsuleCollider[] capsules = FindObjectsOfType<CapsuleCollider>();

        for (int i = 0; i < spheres.Length; i++)
        {
            Sphere sphereA = spheres[i];
            for (int j = i + 1; j < spheres.Length; j++)
            {
                Sphere sphereB = spheres[j];
                CollisionDetection.ApplyCollisionResolution(sphereA, sphereB);
            }

            foreach (PlaneCollider planeCollider in colliders)
            {
                CollisionDetection.ApplyCollisionResolution(sphereA, planeCollider);
            }

            foreach (CapsuleCollider capsule in capsules)
            {
                CollisionDetection.ApplyCollisionResolution(sphereA, capsule);
            }
        }

        for (var i = 0; i < capsules.Length; i++)
        {
            var capsule = capsules[i];

            foreach (PlaneCollider plane in colliders)
            {
                CollisionDetection.ApplyCollisionResolution(plane, capsule);
            }
        }
    }
}
