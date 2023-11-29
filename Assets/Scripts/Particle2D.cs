using UnityEngine;

public class Particle2D : MonoBehaviour
{
    public Vector2 velocity;
    public float angularVelocity;
    public float damping = 0.999f;
    public float angularDamping = 0.99f;
    public Vector2 acceleration;
    public float angularAcceleration;
    public Vector2 gravity = new Vector2(0, -9.8f);
    public float inverseMass = 1.0f;
    public Vector2 accumulatedForces { get; private set; }
    public float accumulatedTorque { get; private set; }

    public float mass => 1 / inverseMass;

    public void FixedUpdate()
    {
        if(TryGetComponent(out CircleCollider sphere))
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
        ClearTorque();
    }

    public void ClearForces()
    {
        accumulatedForces = Vector2.zero;
    }

    public void ClearTorque()
    {
        accumulatedTorque = 0;
    }
    
    public void AddForce(Vector2 force)
    {
        accumulatedForces += force;
    }

    public void AddTorque(float radius, float force, float angle)
    {
        //float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        accumulatedTorque += radius * force * Mathf.Sin(angle);
    }

    public void AddTorque(float radius, Vector2 forceDirection, float force)
    {

        float cosAngle = Vector2.Dot(forceDirection, transform.up); //something here is wrong
        //float sinAngle = dotProduct / normal.magnitude;

        float dot = Vector2.Dot(forceDirection, transform.right);
        if (dot > 0)
        {
            AddTorque(radius, force, cosAngle);
        }
        else if (dot < 0)
        {
            AddTorque(radius, force, -cosAngle);
        }
    }
}
