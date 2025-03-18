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

            // Compute initial angle based on curliness
            float initialAngle = Mathf.Atan2(offsetZ, offsetX);

            // Create a new vertex with a RestAngle
            StrandVertex newVertex = new StrandVertex(currentPosition + offset, Constants.HairMass, isRoot)
            {
                Angle = initialAngle,
                RestAngle = initialAngle,  // Store the initial angle as its "resting" state
                Torque = 0f
            };

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

    void FixedUpdate()
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
                // Gravity force
                Vector3 gravityForce = Constants.Gravity * vertex.Mass;
                Vector3 acceleration = gravityForce / vertex.Mass;

                // Compute total force magnitude
                float totalForceMagnitude = gravityForce.magnitude + Mathf.Abs(vertex.Torque);

                // Apply force only if it surpasses the resistance threshold
                if (totalForceMagnitude > Constants.ForceThreshold)
                {
                    float effectiveForce = totalForceMagnitude - Constants.ForceThreshold; // Only excess force is applied

                    // Apply angular restoration before position updates
                    float angleRestoration = Constants.AngleStiffness * (vertex.RestAngle - vertex.Angle);
                    vertex.Torque += angleRestoration;

                    // Update angular velocity
                    vertex.AngularVelocity *= Constants.RotationDamping;
                    vertex.AngularVelocity += (vertex.Torque / vertex.Mass) * deltaTime;
                    vertex.Angle += vertex.AngularVelocity * deltaTime;

                    // Calculate positional adjustment from angle
                    float rotationOffsetX = Mathf.Sin(vertex.Angle) * Constants.CurlinessFactor;
                    float rotationOffsetZ = Mathf.Cos(vertex.Angle) * Constants.CurlinessFactor;
                    Vector3 rotationOffset = new Vector3(rotationOffsetX, 0, rotationOffsetZ);

                    // Apply velocity and rotation offset
                    vertex.Velocity += (acceleration * deltaTime) * (effectiveForce / totalForceMagnitude);
                    vertex.Position += (vertex.Velocity * deltaTime * Constants.PositionDamping) + rotationOffset;
                }
            }

            // Reset torque after applying it
            vertex.Torque = 0f;
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

            Vector3 totalForceVector = springForce + dampingForce;
            float totalForceMagnitude = totalForceVector.magnitude;

            if (!vertexFrom.isRoot)
            {
                vertexFrom.Velocity += (totalForceVector / vertexFrom.Mass) * deltaTime;
            }
            vertexTo.Velocity -= (totalForceVector / vertexTo.Mass) * deltaTime;

            // Apply torque only if the force surpasses the resistance threshold
            if (totalForceMagnitude > Constants.ForceThreshold)
            {
                float effectiveForce = totalForceMagnitude - Constants.ForceThreshold; // Only apply excess force
                float torqueEffect = Constants.TorqueFactor * (effectiveForce / vertexTo.Mass);
                vertexTo.Torque += torqueEffect;
            }

            // Angular restoring force (bringing angle back to rest)
            float angleRestoringForce = Constants.AngleStiffness * (vertexTo.RestAngle - vertexTo.Angle);
            vertexTo.Torque += angleRestoringForce;

            // Apply rotational damping to prevent excessive spinning
            vertexTo.AngularVelocity *= Constants.RotationDamping;
            vertexTo.AngularVelocity += (vertexTo.Torque / vertexTo.Mass) * deltaTime;
            vertexTo.Angle += vertexTo.AngularVelocity * deltaTime;

            // Reset torque after applying it
            vertexTo.Torque = 0f;
        }
    }


    void ApplyFollowTheLeader()
    {
        for (int i = 1; i < Vertices.Count; i++)
        {
            Vector3 previousPosition = Vertices[i - 1].Position;
            Vector3 direction = (Vertices[i].Position - previousPosition).normalized;
            float restLength = Springs[i - 1].Length;

            // Adjust position while considering the vertex's rest angle for curliness
            float rotationOffsetX = Mathf.Sin(Vertices[i].Angle) * Constants.CurlinessFactor;
            float rotationOffsetZ = Mathf.Cos(Vertices[i].Angle) * Constants.CurlinessFactor;
            Vector3 curlOffset = new Vector3(rotationOffsetX, 0, rotationOffsetZ);

            Vertices[i].Position = previousPosition + direction * restLength + curlOffset;
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
