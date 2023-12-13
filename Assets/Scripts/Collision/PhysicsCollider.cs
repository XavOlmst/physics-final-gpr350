using UnityEngine;

namespace Collision
{
    public class PhysicsCollider : MonoBehaviour
    {
        public float InverseMass
        {
            get
            {
                if (TryGetComponent(out PhysicsRigidbody2D physicsRigidbody))
                {
                    return physicsRigidbody.InverseMass;
                }

                return 0;
            }
            set
            {
                if (TryGetComponent(out PhysicsRigidbody2D physicsRigidbody))
                {
                    physicsRigidbody.InverseMass = value;
                }
            }
        }

        public Vector3 Velocity
        {
            get => TryGetComponent(out PhysicsRigidbody2D physicsRigidbody) ? physicsRigidbody.Velocity : Vector3.zero;
            set
            {
                if (TryGetComponent(out PhysicsRigidbody2D physicsRigidbody))
                {
                    physicsRigidbody.Velocity = value;
                }
            }
        }

        public Vector3 Position
        {
            get => transform.position;
            set
            {
                if (TryGetComponent(out PhysicsRigidbody2D physicsRigidbody))
                {
                    physicsRigidbody.transform.position = value;
                }
            }
        }
    }
}