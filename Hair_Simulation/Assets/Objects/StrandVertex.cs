using UnityEngine;

public class StrandVertex
{
    public Vector3 Position;
    public Vector3 Velocity;
    public float Mass;
    public bool isRoot;

    // 3D Rotational properties
    public Quaternion Rotation;           // Current 3D orientation of the strand at this vertex
    public Vector3 AngularVelocity;       // Angular velocity in 3D (radians/sec)
    public Vector3 Torque;                // Accumulated torque to apply
    public Quaternion RestRotation;       // The target rotation the strand wants to return to

    public float ForceThreshold;          // Individual force threshold

    public StrandVertex(Vector3 position, float mass, bool isRoot)
    {
        this.Position = position;
        this.Velocity = Vector3.zero;
        this.Mass = mass;
        this.isRoot = isRoot;

        this.Rotation = Quaternion.identity;
        this.AngularVelocity = Vector3.zero;
        this.Torque = Vector3.zero;
        this.RestRotation = Quaternion.identity;

        this.ForceThreshold = 0.6f;
    }

    public StrandVertex(Vector3 position, float mass)
        : this(position, mass, false) { }

    public StrandVertex()
        : this(Vector3.zero, 0f, false) { }
}
