using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class HairStrand : MonoBehaviour
{
    public GameObject emitter;
    public HairPhysicsSettings settings;

    public List<StrandVertex> Vertices;
    public List<StrandSpring> Springs;

    public LayerMask collisionMask;
    public float vertexCollisionRadius = 0.02f;

    private Rigidbody emitterRb;
    private TransformMotionTracker motionTracker;

    void Start()
    {
        if (emitter != null)
        {
            emitterRb = emitter.GetComponent<Rigidbody>();
            motionTracker = emitter.GetComponent<TransformMotionTracker>();
        }
    }

    public void InitializeHairStrand(Vector3 rootPosition, float segmentLength, int numberOfVertices, float curlFrequency, float curlDiameter)
    {
        if (Vertices == null) Vertices = new List<StrandVertex>();
        if (Springs == null) Springs = new List<StrandSpring>();

        Vertices.Clear();
        Springs.Clear();

        Vertices = HairGenerationUtil.GenerateCurledStrand(rootPosition, segmentLength, numberOfVertices, curlFrequency, curlDiameter, settings.HairMass);

        for (int i = 0; i < Vertices.Count - 1; i++)
        {
            AddSpring(i, i + 1);
        }

        ApplyForceThresholdGradient();
    }

    public void InitializeHairStrandFromVertices(List<Vector3> importedVertices)
    {
        if (Vertices == null) Vertices = new List<StrandVertex>();
        if (Springs == null) Springs = new List<StrandSpring>();

        Vertices.Clear();
        Springs.Clear();

        Vertices = HairGenerationUtil.GenerateFromImportedVertices(importedVertices, settings.HairMass);

        for (int i = 0; i < Vertices.Count - 1; i++)
        {
            AddSpring(i, i + 1);
        }

        ApplyForceThresholdGradient();
    }

    void ApplyForceThresholdGradient()
    {
        int count = Vertices.Count;

        for (int i = 0; i < count; i++)
        {
            int verticesBelow = count - i - 1;
            float extraThreshold = settings.ForceThresholdLoadFactor * (settings.HairMass * verticesBelow);
            Vertices[i].ForceThreshold = settings.BaseForceThreshold + extraThreshold;
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

    void ApplyCollisionImpulse(ref StrandVertex vertex, float deltaTime)
    {
        Collider[] hits = Physics.OverlapSphere(vertex.Position, vertexCollisionRadius, collisionMask);
        foreach (var hit in hits)
        {
            Vector3 colliderCenter = hit.bounds.center;
            Vector3 toVertex = vertex.Position - colliderCenter;
            float distance = toVertex.magnitude;
            if (distance < 0.0001f) continue;

            Vector3 collisionNormal = toVertex.normalized;
            float desiredDistance = hit.bounds.extents.magnitude + vertexCollisionRadius;
            float penetrationDepth = desiredDistance - distance;
            if (penetrationDepth < 0.001f) continue;

            Vector3 colliderVelocity = Vector3.zero;
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                colliderVelocity = rb.linearVelocity;
            }

            Vector3 relativeVelocity = vertex.Velocity - colliderVelocity;
            float colliderIntoHair = Vector3.Dot(colliderVelocity, collisionNormal);
            float vertexIntoCollider = -Vector3.Dot(vertex.Velocity, collisionNormal);
            float combinedImpact = Mathf.Max(0f, vertexIntoCollider + colliderIntoHair);

            if (combinedImpact > 0f)
            {
                float impulseStrength;

                if (penetrationDepth > 1f)
                {
                    impulseStrength = settings.CollisionForceMultiplier;
                }
                else
                {
                    float t = Mathf.Clamp01(combinedImpact / settings.CollisionFriction);
                    impulseStrength = Mathf.Lerp(8f, settings.MaxCollisionImpulse, t);
                }

                float finalImpulse = impulseStrength * penetrationDepth;
                Vector3 impulse = collisionNormal * finalImpulse;

                vertex.Velocity += impulse * deltaTime;
            }
        }
    }

    void UpdateVertices()
    {
        float deltaTime = Time.deltaTime;

        for (int i = 0; i < Vertices.Count; i++)
        {
            StrandVertex vertex = Vertices[i];

            if (vertex.isRoot)
            {
                Vector3 emitterLinearVelocity = Vector3.zero;
                Vector3 emitterAngularVelocity = Vector3.zero;
                Vector3 rootLocalOffset = vertex.Position - emitter.transform.position;

                if (emitterRb != null)
                {
                    emitterLinearVelocity = emitterRb.linearVelocity;
                    emitterAngularVelocity = emitterRb.angularVelocity;
                    rootLocalOffset = vertex.Position - emitterRb.worldCenterOfMass;
                }
                else if (motionTracker != null)
                {
                    emitterLinearVelocity = motionTracker.LinearVelocity;
                    emitterAngularVelocity = motionTracker.AngularVelocity;
                }

                Vector3 angularContribution = Vector3.Cross(emitterAngularVelocity, rootLocalOffset);
                vertex.Velocity = emitterLinearVelocity + angularContribution;
                vertex.Position += vertex.Velocity * deltaTime;

                if (Vertices.Count > 1)
                {
                    Vector3 normal = rootLocalOffset.normalized;
                    float segmentLength = Springs[0].Length;
                    Vertices[1].Position = vertex.Position + normal * segmentLength;
                }
            }
            else
            {
                ApplyCollisionImpulse(ref vertex, deltaTime);

                Vector3 gravityForce = settings.Gravity * vertex.Mass;
                Vector3 acceleration = gravityForce / vertex.Mass;

                float totalForceMagnitude = gravityForce.magnitude + Mathf.Abs(vertex.Torque);
                float deltaAngle = vertex.RestAngle - vertex.Angle;

                if (Mathf.Sign(deltaAngle) != 0 && Mathf.Sign(vertex.AngularVelocity) != Mathf.Sign(deltaAngle))
                {
                    float angleRestoration = settings.AngleStiffness * deltaAngle;
                    vertex.Torque += angleRestoration;
                }

                vertex.AngularVelocity *= settings.RotationDamping;
                vertex.AngularVelocity += (vertex.Torque / vertex.Mass) * deltaTime;
                vertex.Angle += vertex.AngularVelocity * deltaTime;
                vertex.Angle = Mathf.Clamp(vertex.Angle, -Mathf.PI, Mathf.PI);

                float rotationOffsetX = Mathf.Sin(vertex.Angle) * settings.CurlinessFactor;
                float rotationOffsetZ = Mathf.Cos(vertex.Angle) * settings.CurlinessFactor;
                Vector3 rotationOffset = new Vector3(rotationOffsetX, 0, rotationOffsetZ);

                if (totalForceMagnitude > vertex.ForceThreshold)
                {
                    float effectiveForce = totalForceMagnitude - vertex.ForceThreshold;
                    vertex.Velocity += (acceleration * deltaTime) * (effectiveForce / totalForceMagnitude);
                }

                vertex.Position += (vertex.Velocity * deltaTime * settings.PositionDamping) + rotationOffset;
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

            float springForceMagnitude = settings.HairStiffness * (currentLength - spring.Length);
            Vector3 springForce = springForceMagnitude * direction;

            Vector3 relativeVelocity = vertexTo.Velocity - vertexFrom.Velocity;
            Vector3 dampingForce = settings.HairDamping * relativeVelocity;

            Vector3 totalForceVector = springForce + dampingForce;
            float totalForceMagnitude = totalForceVector.magnitude;

            if (!vertexFrom.isRoot)
            {
                vertexFrom.Velocity += (totalForceVector / vertexFrom.Mass) * deltaTime;
            }

            vertexTo.Velocity -= (totalForceVector / vertexTo.Mass) * deltaTime;

            if (totalForceMagnitude > vertexTo.ForceThreshold)
            {
                float effectiveForce = totalForceMagnitude - vertexTo.ForceThreshold;
                float torqueEffect = settings.TorqueFactor * (effectiveForce / vertexTo.Mass);
                vertexTo.Torque += torqueEffect;
            }

            float deltaAngle = vertexTo.RestAngle - vertexTo.Angle;

            if (Mathf.Sign(deltaAngle) != 0 && Mathf.Sign(vertexTo.AngularVelocity) != Mathf.Sign(deltaAngle))
            {
                float angleRestoration = settings.AngleStiffness * deltaAngle;
                vertexTo.Torque += angleRestoration;
            }

            vertexTo.AngularVelocity *= settings.RotationDamping;
            vertexTo.AngularVelocity += (vertexTo.Torque / vertexTo.Mass) * deltaTime;
            vertexTo.Angle += vertexTo.AngularVelocity * deltaTime;
            vertexTo.Angle = Mathf.Clamp(vertexTo.Angle, -Mathf.PI, Mathf.PI);
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

            float rotationOffsetX = Mathf.Sin(Vertices[i].Angle) * settings.CurlinessFactor;
            float rotationOffsetZ = Mathf.Cos(Vertices[i].Angle) * settings.CurlinessFactor;
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

    public void UpdateRoot(Vector3 newRootPosition, Vector3 newRootNormal)
    {
        if (Vertices.Count == 0) return;

        Vector3 rootOffset = newRootPosition - Vertices[0].Position;
        for (int i = 0; i < Vertices.Count; i++)
        {
            Vertices[i].Position += rootOffset;
        }

        if (Vertices.Count > 1)
        {
            float restLength = Springs[0].Length;
            Vertices[1].Position = Vertices[0].Position + newRootNormal * restLength;
        }
    }
}
