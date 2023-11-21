using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;
using UnityEngine.InputSystem;
using UnityEngine.TestTools.Utils;
using static TestHelpers;
using System.Collections.Generic;
using System.Linq;

public class MovementTest : InputTestFixture
{
    public class IntegratorTestData
    {
        public Vector3 startPosition;
        public Vector2 startVelocity;
        public List<Vector2> constantForces;
        public float damping;
        public Vector2 gravity;
        public float inverseMass;
        public Vector3 expectedPosition;
        public Vector2 expectedVelocity;
        public Vector3 expectedPositionWoMass;
        public Vector2 expectedVelocityWoMass;
        public float dt;
        public Dictionary<int, List<Vector2>> frameSpecificForces;

        public static IntegratorTestData Blank
        {
            get => new IntegratorTestData
            {
                startPosition = Vector3.zero,
                startVelocity = Vector2.zero,
                constantForces = new List<Vector2>(),
                damping = 1f,
                gravity = Vector2.zero,
                inverseMass = 1f,
                expectedPosition = Vector3.zero,
                expectedVelocity = Vector2.zero,
                dt = 1f,
                frameSpecificForces = new Dictionary<int, List<Vector2>>()
            };
        }

        public IntegratorTestData ExpectedPositionWoMass(Vector3 position)
        {
            expectedPositionWoMass = position;
            return this;
        }

        public IntegratorTestData ExpectedVelocityWoMass(Vector3 velocity)
        {
            expectedVelocityWoMass = velocity;
            return this;
        }

        public IntegratorTestData StartPosition(Vector3 position)
        {
            this.startPosition = position;
            return this;
        }

        public IntegratorTestData StartVelocity(Vector2 velocity)
        {
            this.startVelocity = velocity;
            return this;
        }

        public IntegratorTestData AddConstantForce(Vector2 force)
        {
            constantForces.Add(force);
            return this;
        }

        public IntegratorTestData ConstantForces(Vector2[] forces)
        {
            this.constantForces = new List<Vector2>(forces);
            return this;
        }

        public IntegratorTestData AddConstantForces(Vector2[] forces)
        {
            constantForces.AddRange(forces);
            return this;
        }

        public IntegratorTestData Damping(float damping)
        {
            this.damping = damping;
            return this;
        }

        public IntegratorTestData Gravity(Vector2 gravity)
        {
            this.gravity = gravity;
            return this;
        }

        public IntegratorTestData InverseMass(float invMass)
        {
            this.inverseMass = invMass;
            return this;
        }

        public IntegratorTestData Mass(float mass)
        {
            this.inverseMass = 1.0f / mass;
            return this;
        }

        public IntegratorTestData ExpectedPosition(Vector3 position)
        {
            this.expectedPosition = position;
            return this;
        }

        public IntegratorTestData ExpectedVelocity(Vector2 velocity)
        {
            this.expectedVelocity = velocity;
            return this;
        }

        public IntegratorTestData Dt(float dt)
        {
            this.dt = dt;
            return this;
        }

        public IntegratorTestData AddFrameSpecificForce(int frame, Vector2 force, int duration=1)
        {
            for (int i = 0; i < duration; i++)
            {
                List<Vector2> forcesOnFrame;
                if (!frameSpecificForces.TryGetValue(frame + i, out forcesOnFrame))
                {
                    forcesOnFrame = new List<Vector2>();
                }
                forcesOnFrame.Add(force);
                frameSpecificForces[frame + i] = forcesOnFrame;
            }

            return this;
        }

    }

    public static Particle2D SetUpParticle(IntegratorTestData testData)
    {
        Particle2D particle = new GameObject().AddComponent<Particle2D>();

        particle.transform.position = testData.startPosition;
        particle.velocity = testData.startVelocity;
        particle.damping = testData.damping;
        particle.gravity = testData.gravity;
        particle.inverseMass = testData.inverseMass;

        return particle;
    }

    public static void TestParticle(IntegratorTestData testData, Particle2D particle)
    {
        //LogSolution(particle.transform.position, particle.velocity);
        AssertVector3sEqual(particle.transform.position, testData.expectedPosition);
        AssertVector3sEqual(particle.velocity, testData.expectedVelocity);
        Object.Destroy(particle);
    }

    public class PlaneTestData
    {
        public IntegratorTestData particle = IntegratorTestData.Blank;
        public float radius = 1f;
        public Vector3 normal = Vector3.up;
        public Vector3 expectedNormal;
        public float expectedPenetration;
        public float displacement = 0f;
        public string name = null;

        public override string ToString()
        {
            if (name != null)
                return name;
            return base.ToString();
        }

        public static PlaneTestData Blank
        {
            get => new PlaneTestData();
        }

        public PlaneTestData Name(string name)
        {
            this.name = name;
            return this;
        }

        public PlaneTestData Radius(float radius)
        {
            this.radius = radius;
            return this;
        }

        public PlaneTestData Particle(IntegratorTestData data)
        {
            particle = data;
            return this;
        }

        public PlaneTestData Normal(Vector3 normal)
        {
            this.normal = normal;
            return this;
        }

        public PlaneTestData Displacement(float displacement)
        {
            this.displacement = displacement;
            return this;
        }

        public PlaneTestData ExpectedNormal(Vector3 normal)
        {
            expectedNormal = normal;
            return this;
        }

        public PlaneTestData ExpectedPenetration(float penetration)
        {
            expectedPenetration = penetration;
            return this;
        }
    }

    public static PlaneTestData[] planeTestData = new PlaneTestData[]
    {
        PlaneTestData.Blank
            .Particle(IntegratorTestData.Blank
                .StartPosition(new Vector3(0f, 0.25f))
                .StartVelocity(Vector3.down * .5f + Vector3.right)
                .ExpectedPosition(new Vector3(0.00000f, 0.50000f, 0.00000f))
                .ExpectedVelocity(new Vector2(1.00000f, 0.50000f)))
            .Radius(0.7f)
            .ExpectedNormal(new Vector3(0.00000f, 1.00000f, 0.00000f))
            .ExpectedPenetration(0.25000f)
            .Name("Upwards-pointing plane"),
        PlaneTestData.Blank
            .Particle(IntegratorTestData.Blank
                .StartPosition(new Vector3(0f, 0.25f))
                .StartVelocity(Vector3.down * .5f + Vector3.right)
                .ExpectedPosition(new Vector3(0.00000f, 0.00000f, 0.00000f))
                .ExpectedVelocity(new Vector2(1.00000f, -0.50000f)))
            .Radius(1.2f)
            .ExpectedNormal(new Vector3(0.00000f, -1.00000f, 0.00000f))
            .ExpectedPenetration(0.25000f)
            .Displacement(0.5f)
            .Name("Particle Beneath Upwards-Pointing plane"),
        PlaneTestData.Blank
            .Particle(IntegratorTestData.Blank
                .StartPosition(new Vector3(0f, 0.25f))
                .StartVelocity(Vector3.right * 1.25f)
                .ExpectedPosition(new Vector3(0.00000f, 0.25000f, 0.00000f))
                .ExpectedVelocity(new Vector2(1.25000f, 0.00000f)))
            .ExpectedNormal(new Vector3(-0.57735f, -0.57735f, -0.57735f))
            .ExpectedPenetration(-0.22169f)
            .Displacement(0.5f)
            .Normal(Vector3.one)
            .Name("Tilted plane")
    };

    [Test]
    public void PlanePenetrationTest([ValueSource("planeTestData")] PlaneTestData data)
    {
        var particle = SetUpParticle(data.particle);
        var sphere = particle.gameObject.AddComponent<Sphere>();
        var plane = new GameObject().AddComponent<PlaneCollider>();
        plane.transform.up = data.normal;
        plane.transform.position = data.displacement * data.normal;

        float penetration;
        CollisionDetection.GetNormalAndPenetration(sphere, plane, out _, out penetration);

        //Debug.Log($".ExpectedPenetration({penetration:0.00000}f)");
        Assert.That(penetration, Is.EqualTo(data.expectedPenetration).Using(FloatEqualityComparer.Instance));
    }

    [Test]
    public void PlaneNormalTest([ValueSource("planeTestData")] PlaneTestData data)
    {
        var particle = SetUpParticle(data.particle);
        var sphere = particle.gameObject.AddComponent<Sphere>();
        var plane = new GameObject().AddComponent<PlaneCollider>();
        plane.transform.up = data.normal;
        plane.transform.position = data.displacement * data.normal;

        Vector3 normal;
        CollisionDetection.GetNormalAndPenetration(sphere, plane, out normal, out _);

        //Debug.Log($".ExpectedNormal(new Vector3({normal.x:0.00000}f, {normal.y:0.00000}f, {normal.z:0.00000}f))");
        Assert.That(normal, Is.EqualTo(data.expectedNormal).Using(Vector3EqualityComparer.Instance));
    }

    [Test]
    public void PlaneCollisionTest([ValueSource("planeTestData")] PlaneTestData data)
    {
        var particle = SetUpParticle(data.particle);
        var sphere = particle.gameObject.AddComponent<Sphere>();
        var plane = new GameObject().AddComponent<PlaneCollider>();
        plane.transform.up = data.normal;
        plane.transform.position = data.displacement * data.normal;

        CollisionDetection.ApplyCollisionResolution(sphere, plane);

        TestParticle(data.particle, particle);
    }

    public class SphereData
    {
        public float radius = .5f;

        public static SphereData Blank => new SphereData();

        public SphereData Radius(float radius)
        {
            this.radius = radius;
            return this;
        }
    }

    public class SphereTestData
    {
        public IntegratorTestData particle1 = IntegratorTestData.Blank;
        public IntegratorTestData particle2 = IntegratorTestData.Blank;
        public SphereData sphere1 = SphereData.Blank;
        public SphereData sphere2 = SphereData.Blank;
        public Vector3 expectedNormal = Vector3.zero;
        public float expectedPenetration = 0f;
        public string name = null;

        public override string ToString()
        {
            if (name != null)
                return name;
            return base.ToString();
        }

        public static SphereTestData Blank
        {
            get => new SphereTestData();
        }

        public SphereTestData Name(string name)
        {
            this.name = name;
            return this;
        }

        public SphereTestData Particle(IntegratorTestData data)
        {
            particle1 = data;
            return this;
        }

        public SphereTestData OtherParticle(IntegratorTestData data)
        {
            particle2 = data;
            return this;
        }

        public SphereTestData AddSphere(SphereData data)
        {
            sphere1 = data;
            return this;
        }

        public SphereTestData AddSphereToOther(SphereData data)
        {
            sphere2 = data;
            return this;
        }

        public SphereTestData ExpectedNormal(Vector3 normal)
        {
            expectedNormal = normal;
            return this;
        }

        public SphereTestData ExpectedPenetration(float penetration)
        {
            expectedPenetration = penetration;
            return this;
        }
    }

    public static SphereTestData[] sphereTestData = new SphereTestData[]
    {
        SphereTestData.Blank
            .Particle(IntegratorTestData.Blank
                .StartPosition(Vector3.right * 0.5f)
                .StartVelocity(Vector3.left * 0.25f)
                .InverseMass(2f)
                .ExpectedPosition(new Vector3(0.83333f, 0.00000f, 0.00000f))
                .ExpectedVelocity(new Vector2(0.88333f, 0.00000f))
                .ExpectedPositionWoMass(new Vector3(0.75000f, 0.00000f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(0.60000f, 0.00000f)))
            .OtherParticle(IntegratorTestData.Blank
                .StartVelocity(new Vector3(0.6f, 0.2f))
                .InverseMass(1f)
                .ExpectedPosition(new Vector3(-0.16667f, 0.00000f, 0.00000f))
                .ExpectedVelocity(new Vector2(0.03333f, 0.20000f))
                .ExpectedPositionWoMass(new Vector3(-0.25000f, 0.00000f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(-0.25000f, 0.20000f)))
            .ExpectedNormal(Vector3.right)
            .ExpectedPenetration(0.5f)
            .Name("Same axes"),
        SphereTestData.Blank
            .Particle(IntegratorTestData.Blank
                .StartPosition(new Vector3(-0.26f, -0.21f))
                .StartVelocity(new Vector2(0.6f, 0.8f))
                .InverseMass(3f)
                .ExpectedPosition(new Vector3(-0.81500f, 0.13687f, 0.00000f))
                .ExpectedVelocity(new Vector2(0.20899f, 1.04438f))
                .ExpectedPositionWoMass(new Vector3(-1.00000f, 0.25250f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(0.07865f, 1.12584f)))
            .OtherParticle(IntegratorTestData.Blank
                .StartPosition(new Vector3(0.38f, -0.61f))
                .StartVelocity(new Vector2(0, 1f))
                .InverseMass(5f)
                .ExpectedPosition(new Vector3(1.30500f, -1.18812f, 0.00000f))
                .ExpectedVelocity(new Vector2(0.65169f, 0.59270f))
                .ExpectedPositionWoMass(new Vector3(1.12000f, -1.07250f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(0.52135f, 0.67416f)))
            .AddSphere(SphereData.Blank.Radius(1.6f))
            .AddSphereToOther(SphereData.Blank.Radius(0.9f))
            .ExpectedNormal(new Vector3(-0.84800f, 0.53000f, 0.00000f))
            .ExpectedPenetration(1.74528f)
            .Name("Different axes"),
        SphereTestData.Blank
            .Particle(IntegratorTestData.Blank
                .StartPosition(new Vector3(-0.71f, -0.58f))
                .StartVelocity(new Vector3(-.5f, -.8f))
                .InverseMass(0.01f)
                .ExpectedPosition(new Vector3(-0.69111f, -0.55888f, 0.00000f))
                .ExpectedVelocity(new Vector2(-0.44687f, -0.74062f))
                .ExpectedPositionWoMass(new Vector3(-0.22822f, -0.04154f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(0.85477f, 0.71415f)))
            .OtherParticle(IntegratorTestData.Blank
                .StartPosition(new Vector3(-0.88f, -0.77f))
                .StartVelocity(new Vector2(0.2f, 1.3f))
                .InverseMass(0.5f)
                .ExpectedPosition(new Vector3(-1.82466f, -1.82579f, 0.00000f))
                .ExpectedVelocity(new Vector2(-2.45641f, -1.66893f))
                .ExpectedPositionWoMass(new Vector3(-1.36178f, -1.30846f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(-1.15477f, -0.21415f)))
            .AddSphere(SphereData.Blank.Radius(1.2f))
            .ExpectedNormal(new Vector3(0.66680f, 0.74524f, 0.00000f))
            .ExpectedPenetration(1.44505f)
            .Name("Extreme overlap"),
        SphereTestData.Blank
            .Particle(IntegratorTestData.Blank
                .StartPosition(Vector2.right * 0.25f)
                .StartVelocity(Vector2.left * 1.25f)
                .ExpectedPosition(new Vector3(1.00000f, 0.00000f, 0.00000f))
                .ExpectedVelocity(new Vector2(1.25000f, 0.00000f))
                .ExpectedPositionWoMass(new Vector3(1.00000f, 0.00000f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(1.25000f, 0.00000f)))
            .OtherParticle(IntegratorTestData.Blank
                .InverseMass(0)
                .ExpectedPosition(new Vector3(0.00000f, 0.00000f, 0.00000f))
                .ExpectedVelocity(new Vector2(0.00000f, 0.00000f))
                .ExpectedPositionWoMass(new Vector3(0.00000f, 0.00000f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(0.00000f, 0.00000f)))
            .ExpectedNormal(new Vector3(1.00000f, 0.00000f, 0.00000f))
            .ExpectedPenetration(0.75000f)
            .Name("One Particle Has Infinite Mass"),
        SphereTestData.Blank
            .Particle(IntegratorTestData.Blank
                .StartPosition(Vector2.right * 0.25f)
                .InverseMass(0)
                .ExpectedPosition(new Vector3(0.25000f, 0.00000f, 0.00000f))
                .ExpectedVelocity(new Vector2(0.00000f, 0.00000f))
                .ExpectedPositionWoMass(new Vector3(0.25000f, 0.00000f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(0.00000f, 0.00000f)))
            .OtherParticle(IntegratorTestData.Blank
                .InverseMass(0)
                .ExpectedPosition(new Vector3(0.00000f, 0.00000f, 0.00000f))
                .ExpectedVelocity(new Vector2(0.00000f, 0.00000f))
                .ExpectedPositionWoMass(new Vector3(0.00000f, 0.00000f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(0.00000f, 0.00000f)))
            .ExpectedNormal(new Vector3(1.00000f, 0.00000f, 0.00000f))
            .ExpectedPenetration(0.75000f)
            .Name("Whole System Has Infinite Mass")
    };

    [Test]
    public void NormalCalculationTest([ValueSource("sphereTestData")] SphereTestData data)
    {
        var p1 = SetUpParticle(data.particle1);
        var s1 = p1.gameObject.AddComponent<Sphere>();
        s1.Radius = data.sphere1.radius;

        var p2 = SetUpParticle(data.particle2);
        var s2 = p2.gameObject.AddComponent<Sphere>();
        s2.Radius = data.sphere2.radius;

        Vector3 normal;
        CollisionDetection.GetNormalAndPenetration(s1, s2, out normal, out _);

        //Debug.Log($".ExpectedNormal(new Vector3({normal.x:0.00000}f, {normal.y:0.00000}f, {normal.z:0.00000}f))");
        AssertVector3sEqual(normal, data.expectedNormal);
    }

    [Test]
    public void PenetrationCalculationTest([ValueSource("sphereTestData")] SphereTestData data)
    {
        var p1 = SetUpParticle(data.particle1);
        var s1 = p1.gameObject.AddComponent<Sphere>();
        s1.Radius = data.sphere1.radius;

        var p2 = SetUpParticle(data.particle2);
        var s2 = p2.gameObject.AddComponent<Sphere>();
        s2.Radius = data.sphere2.radius;

        float penetration;
        CollisionDetection.GetNormalAndPenetration(s1, s2, out _, out penetration);

        //Debug.Log($".ExpectedPenetration({penetration:0.00000}f)");
        Assert.That(penetration, Is.EqualTo(data.expectedPenetration).Using(new FloatEqualityComparer(1e-5f)));
    }

    [Test]
    public void DisplacementTestIgnoringMass([ValueSource("sphereTestData")] SphereTestData data)
    {
        var p1 = SetUpParticle(data.particle1);
        var s1 = p1.gameObject.AddComponent<Sphere>();
        s1.Radius = data.sphere1.radius;
        if (s1.invMass != 0) s1.invMass = 1f;

        var p2 = SetUpParticle(data.particle2);
        var s2 = p2.gameObject.AddComponent<Sphere>();
        s2.Radius = data.sphere2.radius;
        if (s2.invMass != 0) s2.invMass = 1f;

        CollisionDetection.ApplyCollisionResolution(s1, s2);

        //Debug.Log("Particle1:");
        //LogSolution(s1.position, s1.velocity, "ExpectedPositionWoMass", "ExpectedVelocityWoMass");
        //Debug.Log("Particle2:");
        //LogSolution(s2.position, s2.velocity, "ExpectedPositionWoMass", "ExpectedVelocityWoMass");
        AssertVector3sEqual(s1.position, data.particle1.expectedPositionWoMass);
        AssertVector3sEqual(s2.position, data.particle2.expectedPositionWoMass);
    }

    [Test]
    public void DisplacementTest([ValueSource("sphereTestData")] SphereTestData data)
    {
        var p1 = SetUpParticle(data.particle1);
        var s1 = p1.gameObject.AddComponent<Sphere>();
        s1.Radius = data.sphere1.radius;

        var p2 = SetUpParticle(data.particle2);
        var s2 = p2.gameObject.AddComponent<Sphere>();
        s2.Radius = data.sphere2.radius;

        CollisionDetection.ApplyCollisionResolution(s1, s2);

        //Debug.Log("Particle1:");
        //LogSolution(s1.position, s1.velocity);
        //Debug.Log("Particle2:");
        //LogSolution(s2.position, s2.velocity);
        AssertVector3sEqual(s1.position, data.particle1.expectedPosition);
        AssertVector3sEqual(s2.position, data.particle2.expectedPosition);
    }

    [Test]
    public void VelocityTestIgnoringMass([ValueSource("sphereTestData")] SphereTestData data)
    {
        var p1 = SetUpParticle(data.particle1);
        var s1 = p1.gameObject.AddComponent<Sphere>();
        s1.Radius = data.sphere1.radius;

        if (s1.invMass != 0) s1.invMass = 1;

        var p2 = SetUpParticle(data.particle2);
        var s2 = p2.gameObject.AddComponent<Sphere>();
        s2.Radius = data.sphere2.radius;

        if (s2.invMass != 0) s2.invMass = 1;

        CollisionDetection.ApplyCollisionResolution(s1, s2);

        AssertVector3sEqual(s1.velocity, data.particle1.expectedVelocityWoMass);
        AssertVector3sEqual(s2.velocity, data.particle2.expectedVelocityWoMass);
    }

    [Test]
    public void VelocityTest([ValueSource("sphereTestData")] SphereTestData data)
    {
        var p1 = SetUpParticle(data.particle1);
        var s1 = p1.gameObject.AddComponent<Sphere>();
        s1.Radius = data.sphere1.radius;

        var p2 = SetUpParticle(data.particle2);
        var s2 = p2.gameObject.AddComponent<Sphere>();
        s2.Radius = data.sphere2.radius;

        CollisionDetection.ApplyCollisionResolution(s1, s2);

        AssertVector3sEqual(s1.velocity, data.particle1.expectedVelocity);
        AssertVector3sEqual(s2.velocity, data.particle2.expectedVelocity);
    }

    /*
    [Test]
    public void SphereCollisionTest([ValueSource("sphereTestData")] SphereTestData data)
    {
        var p1 = SetUpParticle(data.particle1);
        var s1 = p1.gameObject.AddComponent<Sphere>();
        s1.Radius = data.sphere1.radius;

        var p2 = SetUpParticle(data.particle2);
        var s2 = p2.gameObject.AddComponent<Sphere>();
        s2.Radius = data.sphere2.radius;

        Debug.Log($"r1: {s1.Radius}, r2: {s2.Radius}");

        float penetration;
        Vector3 normal;
        CollisionDetection.GetNormalAndPenetration(s1, s2, out normal, out penetration);

        Assert.That(penetration, Is.EqualTo(0.5f), "Penetration value is miscalculated.");

        Assert.That(normal, Is.EqualTo((s1.position - s2.position).normalized), "Normal value is miscalculated.");

        Assert.That(normal.sqrMagnitude, Is.EqualTo(1), "Normal's magnitude is not 1.");

        Vector3 s1OldPos = s1.position;
        Vector3 s2OldPos = s2.position;
        float preMomentum = CalculateMomentum(s1, s2);
        CollisionDetection.ApplyCollisionResolution(s1, s2);
        float postMomentum = CalculateMomentum(s1, s2);

        Vector3 s1Displacement = s1.position - s1OldPos;
        Vector3 s2Displacement = s2.position - s2OldPos;
        float totalMovement = s1Displacement.magnitude + s2Displacement.magnitude;
        float s1PctMovement = s1Displacement.magnitude / totalMovement;
        float s2PctMovement = s2Displacement.magnitude / totalMovement;

        Assert.That(s1PctMovement + s2PctMovement, Is.EqualTo(1f), "Total movement percentage is not 1 -- this is a bug in the test!");

        float totalInvMass = s1.invMass + s2.invMass;
        Assert.That(s1PctMovement, Is.EqualTo(s1.invMass / totalInvMass).Using(new FloatEqualityComparer(1e-4f)),
            "Percentage of total movement by one particle is not proportional to that particle's share of total mass.");
        Assert.That(s2PctMovement, Is.EqualTo(s2.invMass / totalInvMass).Using(new FloatEqualityComparer(1e-4f)),
            "Percentage of total movement by one particle is not proportional to that particle's share of total mass.");

    }
    */

    private float CalculateMomentum(params PhysicsCollider[] colliders)
    {
        float momentum = 0;
        foreach (PhysicsCollider c in colliders)
        {
            if (c.invMass != 0)
                momentum += (1.0f / c.invMass) * c.velocity.magnitude;
        }
        return momentum;
    }
}
