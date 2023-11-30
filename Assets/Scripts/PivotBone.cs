using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotBone : MonoBehaviour
{
    public List<Particle2D> ChildCapsules;
    [HideInInspector] public List<Vector3> ChildLocalPositions = new();

    private void Start()
    {
        foreach (var capsule in ChildCapsules)
        {
            ChildLocalPositions.Add(capsule.transform.localPosition);
        }
    }
}
