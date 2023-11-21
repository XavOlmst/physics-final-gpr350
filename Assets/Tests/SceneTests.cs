using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using static MovementTest;

public class SceneTests : InputTestFixture
{
    Mouse mouse { get => Mouse.current; }
    const string testSceneId = "Assets/Scenes/TestScene.unity";

    public override void Setup()
    {
        base.Setup();
        InputSystem.AddDevice<Keyboard>();
        InputSystem.AddDevice<Mouse>();
        SceneManager.LoadScene(testSceneId);
    }

    // Test parser expects all tests to be iterated, so we add this
    // variable to make each test run once.
    public static int[] dummyData = new int[] { 0 };

    public static SphereTestData[] sphereTestData = new SphereTestData[]
    {
        SphereTestData.Blank
            .Particle(IntegratorTestData.Blank
                .StartPosition(Vector3.right * 0.5f)
                .StartVelocity(Vector3.left * 0.25f)
                .InverseMass(2f)
                .ExpectedPosition(new Vector3(1.71667f, 0.00000f, 0.00000f))
                .ExpectedVelocity(new Vector2(0.88333f, 0.00000f))
                .ExpectedPositionWoMass(new Vector3(0.75000f, 0.00000f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(0.60000f, 0.00000f)))
            .OtherParticle(IntegratorTestData.Blank
                .StartVelocity(new Vector3(0.6f, 0.2f))
                .InverseMass(1f)
                .ExpectedPosition(new Vector3(-0.13333f, 0.20000f, 0.00000f))
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
                .ExpectedPosition(new Vector3(-0.60601f, 1.18126f, 0.00000f))
                .ExpectedVelocity(new Vector2(0.20899f, 1.04438f))
                .ExpectedPositionWoMass(new Vector3(-0.36400f, -0.14500f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(0.07865f, 1.12584f)))
            .OtherParticle(IntegratorTestData.Blank
                .StartPosition(new Vector3(0.38f, -0.61f))
                .StartVelocity(new Vector2(0, 1f))
                .InverseMass(5f)
                .ExpectedPosition(new Vector3(1.95669f, -0.59543f, 0.00000f))
                .ExpectedVelocity(new Vector2(0.65169f, 0.59270f))
                .ExpectedPositionWoMass(new Vector3(0.48400f, -0.67500f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(0.52135f, 0.67416f)))
            .AddSphere(SphereData.Blank.Radius(1.6f))
            .AddSphereToOther(SphereData.Blank.Radius(0.9f))
            .ExpectedNormal(new Vector3(-0.84800f, 0.53000f, 0.00000f))
            .ExpectedPenetration(0.24528f)
            .Name("Different axes"),
        SphereTestData.Blank
            .Particle(IntegratorTestData.Blank
                .StartPosition(new Vector3(-0.71f, -0.58f))
                .StartVelocity(new Vector3(-.5f, -.8f))
                .InverseMass(0.01f)
                .ExpectedPosition(new Vector3(-1.13798f, -1.29951f, 0.00000f))
                .ExpectedVelocity(new Vector2(-0.44687f, -0.74062f))
                .ExpectedPositionWoMass(new Vector3(-0.46160f, -0.30238f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(0.85477f, 0.71415f)))
            .OtherParticle(IntegratorTestData.Blank
                .StartPosition(new Vector3(-0.88f, -0.77f))
                .StartVelocity(new Vector2(0.2f, 1.3f))
                .InverseMass(0.5f)
                .ExpectedPosition(new Vector3(-4.28107f, -3.49473f, 0.00000f))
                .ExpectedVelocity(new Vector2(-2.45641f, -1.66893f))
                .ExpectedPositionWoMass(new Vector3(-1.12840f, -1.04762f, 0.00000f))
                .ExpectedVelocityWoMass(new Vector2(-1.15477f, -0.21415f)))
            .AddSphere(SphereData.Blank.Radius(1.2f))
            .ExpectedNormal(new Vector3(0.66680f, 0.74524f, 0.00000f))
            .ExpectedPenetration(0.74505f)
            .Name("Extreme overlap"),
        SphereTestData.Blank
            .Particle(IntegratorTestData.Blank
                .StartPosition(Vector2.right * 0.25f)
                .StartVelocity(Vector2.left * 1.25f)
                .ExpectedPosition(new Vector3(2.25000f, 0.00000f, 0.00000f))
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

    public static PlaneTestData[] planeTestData = new PlaneTestData[]
{
        PlaneTestData.Blank
            .Particle(IntegratorTestData.Blank
                .StartPosition(new Vector3(0f, 0.25f))
                .StartVelocity(Vector3.down * .5f + Vector3.right)
                .ExpectedPosition(new Vector3(1.00000f, 1.00000f, 0.00000f))
                .ExpectedVelocity(new Vector2(1.00000f, 0.50000f)))
            .Radius(0.7f)
            .ExpectedNormal(new Vector3(0.00000f, 1.00000f, 0.00000f))
            .ExpectedPenetration(0.25000f)
            .Name("Upwards-pointing plane"),
        PlaneTestData.Blank
            .Particle(IntegratorTestData.Blank
                .StartPosition(new Vector3(0f, 0.25f))
                .StartVelocity(Vector3.down * .5f + Vector3.right)
                .ExpectedPosition(new Vector3(1.00000f, -0.50000f, 0.00000f))
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
                .ExpectedPosition(new Vector3(0.67799f, -0.32201f, -0.00534f))
                .ExpectedVelocity(new Vector2(0.41667f, -0.83333f)))
            .ExpectedNormal(new Vector3(-0.57735f, -0.57735f, -0.57735f))
            .ExpectedPenetration(-0.22169f)
            .Displacement(0.5f)
            .Normal(Vector3.one)
            .Name("Tilted plane")
    };

    [UnityTest]
    public IEnumerator TestSpherePlaneCollision([ValueSource("planeTestData")] PlaneTestData data)
    {
        var particle = SetUpParticle(data.particle);
        particle.gameObject.AddComponent<Sphere>();
        var plane = new GameObject().AddComponent<PlaneCollider>();
        plane.transform.up = data.normal;
        plane.transform.position = data.displacement * data.normal;

        int iterations = Mathf.RoundToInt(data.particle.dt / Time.fixedDeltaTime);

        for (int i = 0; i < iterations; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        TestParticle(data.particle, particle);
    }


    [UnityTest]
    public IEnumerator TestSphereSphereCollision([ValueSource("sphereTestData")] SphereTestData data)
    {
        var p1 = SetUpParticle(data.particle1);
        var s1 = p1.gameObject.AddComponent<Sphere>();
        s1.Radius = data.sphere1.radius;

        var p2 = SetUpParticle(data.particle2);
        var s2 = p2.gameObject.AddComponent<Sphere>();
        s2.Radius = data.sphere2.radius;

        int iterations = Mathf.RoundToInt(data.particle1.dt / Time.fixedDeltaTime);

        for (int i = 0; i < iterations; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        TestParticle(data.particle1, p1.GetComponent<Particle2D>());
        TestParticle(data.particle2, p2.GetComponent<Particle2D>());
    }
}
