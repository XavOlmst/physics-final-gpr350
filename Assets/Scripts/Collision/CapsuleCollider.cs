using UnityEngine;

namespace Collision // btw, use of namespace is good
{
    // this class is fine
    public class CapsuleCollider : PhysicsCollider
    {
        public float LengthOffset = 0.5f;
        public Vector3 Center => transform.position;
        public float Radius = .5f;

        public Vector3 TopPoint => (transform.up * LengthOffset);

        public Vector3 BottomPoint => -TopPoint;

        // this seems good
        public Vector3 ClosestPoint(Vector3 pos)
        {
            Vector3 distance = pos - Center;
            Vector3 up = transform.up;
        
            float localLength = Vector3.Dot(distance, up);
            localLength = Mathf.Clamp(localLength, -LengthOffset, LengthOffset);
        
            Vector3 closestPoint = (up * localLength);
        
            return closestPoint + Center;
        }
        
        /// <summary>
        /// Adds torque to a capsule if it has a rigidbody
        /// </summary>
        /// <param name="closestPoint"></param>
        /// <param name="force"></param>
        /// <returns>returns force not used for rotation</returns>
        public Vector3 AddTorque(Vector3 closestPoint, Vector3 force)
        {
            if (TryGetComponent(out PhysicsRigidbody2D particle))
            {
                // this is the gist of it !
                return particle.Bone ? particle.AddTorque((Center - particle.Bone.position) - closestPoint, force) 
                    : particle.AddTorque(closestPoint, force);
            }

            return force;
        }
    }
}
