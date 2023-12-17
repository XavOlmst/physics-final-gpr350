using UnityEngine;

namespace Collision
{
    public class CircleCollider : PhysicsCollider // ok
    {
        public Vector3 Center => transform.position;
        public float Radius = .5f;
    }
}
