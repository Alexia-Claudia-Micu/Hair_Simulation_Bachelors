using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HairStrand : MonoBehaviour
{
    public GameObject emitter;

    public List<StrandVertex> Vertices;
    public List<StrandSpring> Springs;

    private ComputeShader hairSolver;
    private ComputeBuffer vertexBuffer;
    private ComputeBuffer springBuffer;
    private ComputeBuffer volumeBuffer;

    void Start()
    {
        this.Vertices = new List<StrandVertex>();
        this.Springs = new List<StrandSpring>();

        InitializeHairStrand();
        InitializeComputeBuffers();
    }

    void InitializeHairStrand()
    {
        CurlyStrand();

        for (int i = 0; i < Vertices.Count - 1; i++)
        {
            AddSpring(i, i + 1);
        }
    }

    void CurlyStrand()
    {
        StrandVertex vertex0 = new StrandVertex(new Vector3(0, 6f, 0), Constants.HairMass, true); this.Vertices.Add(vertex0);
        StrandVertex vertex1 = new StrandVertex(new Vector3(0.4f, 5.6f, 0), Constants.HairMass); this.Vertices.Add(vertex1);
        StrandVertex vertex2 = new StrandVertex(new Vector3(0, 5.2f, 0), Constants.HairMass); this.Vertices.Add(vertex2);
        StrandVertex vertex3 = new StrandVertex(new Vector3(-0.4f, 4.8f, 0), Constants.HairMass); this.Vertices.Add(vertex3);
        StrandVertex vertex4 = new StrandVertex(new Vector3(0.0f, 4.4f, 0), Constants.HairMass); this.Vertices.Add(vertex4);
        StrandVertex vertex5 = new StrandVertex(new Vector3(0.4f, 4.0f, 0), Constants.HairMass); this.Vertices.Add(vertex5);
        StrandVertex vertex6 = new StrandVertex(new Vector3(0.0f, 3.6f, 0), Constants.HairMass); this.Vertices.Add(vertex6);
        StrandVertex vertex7 = new StrandVertex(new Vector3(-0.4f, 3.2f, 0), Constants.HairMass); this.Vertices.Add(vertex7);
        StrandVertex vertex8 = new StrandVertex(new Vector3(0.0f, 2.8f, 0), Constants.HairMass); this.Vertices.Add(vertex8);
        StrandVertex vertex9 = new StrandVertex(new Vector3(0.4f, 2.4f, 0), Constants.HairMass); this.Vertices.Add(vertex9);
        StrandVertex vertex10 = new StrandVertex(new Vector3(0.0f, 2.0f, 0), Constants.HairMass); this.Vertices.Add(vertex10);
    }

    void StraightStrand()
    {
        StrandVertex vertex0 = new StrandVertex(new Vector3(0, 6f, 0), Constants.HairMass, true); this.Vertices.Add(vertex0);
        StrandVertex vertex1 = new StrandVertex(new Vector3(0, 5.4f, 0), Constants.HairMass); this.Vertices.Add(vertex1);
        StrandVertex vertex2 = new StrandVertex(new Vector3(0, 4.8f, 0), Constants.HairMass); this.Vertices.Add(vertex2);
        StrandVertex vertex3 = new StrandVertex(new Vector3(0, 4.2f, 0), Constants.HairMass); this.Vertices.Add(vertex3);
        StrandVertex vertex4 = new StrandVertex(new Vector3(0, 3.6f, 0), Constants.HairMass); this.Vertices.Add(vertex4);
        StrandVertex vertex5 = new StrandVertex(new Vector3(0, 3.0f, 0), Constants.HairMass); this.Vertices.Add(vertex5);
        StrandVertex vertex6 = new StrandVertex(new Vector3(0, 2.4f, 0), Constants.HairMass); this.Vertices.Add(vertex6);
        StrandVertex vertex7 = new StrandVertex(new Vector3(0, 1.8f, 0), Constants.HairMass); this.Vertices.Add(vertex7);
        StrandVertex vertex8 = new StrandVertex(new Vector3(0, 1.2f, 0), Constants.HairMass); this.Vertices.Add(vertex8);
        StrandVertex vertex9 = new StrandVertex(new Vector3(0, 0.6f, 0), Constants.HairMass); this.Vertices.Add(vertex9);
        StrandVertex vertex10 = new StrandVertex(new Vector3(0, 0f, 0), Constants.HairMass); this.Vertices.Add(vertex10);
    }

    void InitializeComputeBuffers()
    {
        vertexBuffer = new ComputeBuffer(Vertices.Count, sizeof(float) * 6);
        springBuffer = new ComputeBuffer(Springs.Count, sizeof(float) * 4);
        volumeBuffer = new ComputeBuffer(1, sizeof(float) * 3);  // For external forces like wind

        hairSolver = Resources.Load<ComputeShader>("HairSolver");
    }

    void AddSpring(int from, int to)
    {
        float restLength = (Vertices[to].Position - Vertices[from].Position).magnitude;
        Springs.Add(new StrandSpring(from, to, restLength));
    }

    void Update()
    {
        SimulateStrand();
        ApplyFollowTheLeader();
        UpdateVertices();
    }

    void UpdateVertices()
    {
        float deltaTime = Time.deltaTime;

        for (int i = 0; i < Vertices.Count; i++)
        {
            StrandVertex vertex = Vertices[i];

            if (vertex.isRoot)
            {
                Vector3 emitterVelocity = emitter.GetComponent<Rigidbody>().linearVelocity;
                vertex.Velocity = emitterVelocity;
                vertex.Position += vertex.Velocity * deltaTime;
            }
            else
            {
                Vector3 gravityForce = Constants.Gravity * vertex.Mass;

                Vector3 acceleration = gravityForce / vertex.Mass;
                vertex.Velocity += acceleration * deltaTime;
                vertex.Position += vertex.Velocity * deltaTime;
            }
        }
    }

    void SimulateStrand()
    {
        float deltaTime = Time.deltaTime;

        foreach (var spring in this.Springs)
        {
            StrandVertex vertexFrom = this.Vertices[spring.VertexFrom];
            StrandVertex vertexTo = this.Vertices[spring.VertexTo];

            Vector3 direction = vertexTo.Position - vertexFrom.Position;
            float currentLength = direction.magnitude;
            direction.Normalize();

            float springForceMagnitude = Constants.HairStiffness * (currentLength - spring.Length);
            Vector3 springForce = springForceMagnitude * direction;

            Vector3 relativeVelocity = vertexTo.Velocity - vertexFrom.Velocity;
            Vector3 dampingForce = Constants.HairDamping * relativeVelocity;

            Vector3 totalForce = springForce + dampingForce;

            if (!vertexFrom.isRoot)
            {
                vertexFrom.Velocity += (totalForce / vertexFrom.Mass) * deltaTime;
            }
            vertexTo.Velocity -= (totalForce / vertexTo.Mass) * deltaTime;
        }
    }

    void ApplyFollowTheLeader()
    {
        for (int i = 1; i < Vertices.Count; i++)
        {
            Vector3 previousPosition = Vertices[i - 1].Position;
            Vector3 direction = (Vertices[i].Position - previousPosition).normalized;
            float restLength = Springs[i - 1].Length;

            Vertices[i].Position = previousPosition + direction * restLength;
        }
    }

    void OnDestroy()
    {
        vertexBuffer.Release();
        springBuffer.Release();
        volumeBuffer.Release();
    }

}