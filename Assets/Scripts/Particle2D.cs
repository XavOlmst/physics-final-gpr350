using UnityEngine;

public class Particle2D : MonoBehaviour
{
    public Vector2 velocity;
    public Vector3 angularVelocity;
    public float damping = 0.999f;
    public Vector2 acceleration;
    public Vector3 angularAcceleration;
    public Vector2 gravity = new Vector2(0, -9.8f);
    public float inverseMass = 1.0f;
    public Vector2 accumulatedForces { get; private set; }
    public Vector3 accumulatedTorque { get; private set; }

    public float mass => 1 / inverseMass;

    public void FixedUpdate()
    {
        if(TryGetComponent(out Sphere sphere))
        {
            DoFixedUpdate(sphere.Radius, Time.deltaTime);
        }
        else if(TryGetComponent(out CapsuleCollider capsule))
        {
            DoFixedUpdate(capsule.Radius, Time.deltaTime);
        }
        else
        {
            DoFixedUpdate(Time.deltaTime);
        }
    }

    public void DoFixedUpdate(float dt)
    {
        acceleration = gravity + accumulatedForces * inverseMass;
        Integrator.Integrate(this, dt);
        ClearForces();
    }
    
    public void DoFixedUpdate(float radius, float dt)
    {
        acceleration = gravity + accumulatedForces * inverseMass;
        Integrator.Integrate(radius,this, dt);
        ClearForces();
    }

    public void ClearForces()
    {
        accumulatedForces = Vector2.zero;
    }

    public void AddForce(Vector2 force)
    {
        accumulatedForces += force;
    }

    public void AddTorque(float radius, Vector3 force, float angle)
    {
        accumulatedTorque += radius * force * Mathf.Sin(angle);
    }
}
