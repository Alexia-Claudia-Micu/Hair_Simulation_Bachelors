using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// TODO: fix spinning

public class HairStrand : MonoBehaviour
{
    public GameObject emitter;

    public List<StrandVertex> Vertices;
    public List<StrandSpring> Springs;


    public void InitializeHairStrand(Vector3 rootPosition, float segmentLength, int numberOfVertices, float curlinessFactor)
    {
        // Ensure the lists are properly initialized before use
        if (Vertices == null) Vertices = new List<StrandVertex>();
        if (Springs == null) Springs = new List<StrandSpring>();

        Vertices.Clear();
        Springs.Clear();

        GenerateStrand(rootPosition, segmentLength, numberOfVertices, curlinessFactor);

        for (int i = 0; i < Vertices.Count - 1; i++)
        {
            AddSpring(i, i + 1);
        }
    }

    void GenerateStrand(Vector3 rootPosition, float segmentLength, int numberOfVertices, float curlinessFactor)
    {
        Vector3 currentPosition = rootPosition;
        bool isRoot = true;

        for (int i = 0; i < numberOfVertices; i++)
        {
            // Add slight variation for curly effect
            float offsetX = Mathf.Sin(i * Mathf.PI * 0.5f) * curlinessFactor;
            float offsetZ = Mathf.Cos(i * Mathf.PI * 0.5f) * curlinessFactor;
            Vector3 offset = new Vector3(offsetX, 0, offsetZ);

            // Create a new vertex
            StrandVertex newVertex = new StrandVertex(currentPosition + offset, Constants.HairMass, isRoot);
            Vertices.Add(newVertex);

            // Move to the next position in a straight downward line
            currentPosition.y -= segmentLength;
            isRoot = false;
        }
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

    public void UpdateRootPosition(Vector3 newRootPosition)
    {
        if (Vertices.Count > 0)
        {
            Vector3 rootOffset = newRootPosition - Vertices[0].Position;

            // Update all vertices based on the root movement
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i].Position += rootOffset;
            }
        }
    }
}
