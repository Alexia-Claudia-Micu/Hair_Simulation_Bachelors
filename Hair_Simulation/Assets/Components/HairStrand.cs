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
            float offsetX = Mathf.Sin(i * Mathf.PI * 0.5f) * curlinessFactor;
            float offsetZ = Mathf.Cos(i * Mathf.PI * 0.5f) * curlinessFactor;
            Vector3 offset = new Vector3(offsetX, 0, offsetZ);

            float initialAngle = Mathf.Atan2(offsetZ, offsetX);

            StrandVertex newVertex = new StrandVertex(currentPosition + offset, Constants.HairMass, isRoot)
            {
                Angle = initialAngle,
                RestAngle = initialAngle,
                Torque = 0f
            };

            Vertices.Add(newVertex);

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
                Rigidbody emitterRb = emitter.GetComponent<Rigidbody>();
                Vector3 emitterLinearVelocity = emitterRb.linearVelocity;
                Vector3 emitterAngularVelocity = emitterRb.angularVelocity;
                Vector3 rootLocalOffset = vertex.Position - emitterRb.worldCenterOfMass;

                Vector3 angularContribution = Vector3.Cross(emitterAngularVelocity, rootLocalOffset);
                vertex.Velocity = emitterLinearVelocity + angularContribution;
                vertex.Position += vertex.Velocity * deltaTime;

                // NEW: force the first segment to align with emitter surface normal
                if (Vertices.Count > 1)
                {
                    Vector3 normal = rootLocalOffset.normalized;

                    // Get spring length between vertex[0] and vertex[1]
                    float segmentLength = Springs[0].Length;

                    // Reposition vertex[1] directly along the normal
                    Vertices[1].Position = vertex.Position + normal * segmentLength;
                }
            }

            else
            {
                Vector3 gravityForce = Constants.Gravity * vertex.Mass;
                Vector3 acceleration = gravityForce / vertex.Mass;

                float totalForceMagnitude = gravityForce.magnitude + Mathf.Abs(vertex.Torque);

                float deltaAngle = vertex.RestAngle - vertex.Angle;

                //  Only apply restoring torque if it's pulling back toward RestAngle
                if (Mathf.Sign(deltaAngle) != 0 && Mathf.Sign(vertex.AngularVelocity) != Mathf.Sign(deltaAngle))
                {
                    float angleRestoration = Constants.AngleStiffness * deltaAngle;
                    vertex.Torque += angleRestoration;
                }

                vertex.AngularVelocity *= Constants.RotationDamping;
                vertex.AngularVelocity += (vertex.Torque / vertex.Mass) * deltaTime;
                vertex.Angle += vertex.AngularVelocity * deltaTime;
                vertex.Angle = Mathf.Clamp(vertex.Angle, -Mathf.PI, Mathf.PI); // Optional clamp

                float rotationOffsetX = Mathf.Sin(vertex.Angle) * Constants.CurlinessFactor;
                float rotationOffsetZ = Mathf.Cos(vertex.Angle) * Constants.CurlinessFactor;
                Vector3 rotationOffset = new Vector3(rotationOffsetX, 0, rotationOffsetZ);

                if (totalForceMagnitude > Constants.ForceThreshold)
                {
                    float effectiveForce = totalForceMagnitude - Constants.ForceThreshold;
                    vertex.Velocity += (acceleration * deltaTime) * (effectiveForce / totalForceMagnitude);
                }

                vertex.Position += (vertex.Velocity * deltaTime * Constants.PositionDamping) + rotationOffset;
            }

            vertex.Torque = 0f;
        }
    }

    void SimulateStrand()
    {
        float deltaTime = Time.deltaTime;

        foreach (var spring in Springs)
        {
            StrandVertex vertexFrom = Vertices[spring.VertexFrom];
            StrandVertex vertexTo = Vertices[spring.VertexTo];

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

            if (totalForceMagnitude > Constants.ForceThreshold)
            {
                float effectiveForce = totalForceMagnitude - Constants.ForceThreshold;
                float torqueEffect = Constants.TorqueFactor * (effectiveForce / vertexTo.Mass);
                vertexTo.Torque += torqueEffect;
            }

            float deltaAngle = vertexTo.RestAngle - vertexTo.Angle;

            if (Mathf.Sign(deltaAngle) != 0 && Mathf.Sign(vertexTo.AngularVelocity) != Mathf.Sign(deltaAngle))
            {
                float angleRestoration = Constants.AngleStiffness * deltaAngle;
                vertexTo.Torque += angleRestoration;
            }

            vertexTo.AngularVelocity *= Constants.RotationDamping;
            vertexTo.AngularVelocity += (vertexTo.Torque / vertexTo.Mass) * deltaTime;
            vertexTo.Angle += vertexTo.AngularVelocity * deltaTime;
            vertexTo.Angle = Mathf.Clamp(vertexTo.Angle, -Mathf.PI, Mathf.PI); // Optional clamp

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

            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i].Position += rootOffset;
            }
        }
    }
}
