using UnityEngine;

public class StrandVertex
{
    public Vector3 Position;
    public Vector3 Velocity;
    public float Mass;
    public bool isRoot;

    // Rotational properties
    public float Angle;            // Current angle of the strand at this vertex
    public float AngularVelocity;  // Speed of rotation
    public float Torque;           // Rotational force applied to the vertex

    public float RestAngle;        // The angle the vertex wants to return to
    public float ForceThreshold;   //  Individual force threshold


    public StrandVertex(Vector3 position, float mass, bool isRoot)
    {
        this.Position = position;
        this.Velocity = Vector3.zero;
        this.Mass = mass;
        this.isRoot = isRoot;
        this.Angle = 0f;
        this.AngularVelocity = 0f;
        this.Torque = 0f;
        this.RestAngle = 0f;  // Initially set to 0, will be adjusted later
        this.ForceThreshold = 0.6f;
    }


    public StrandVertex(Vector3 position, float mass)
    {
        this.Position = position;
        this.Velocity = Vector3.zero;
        this.Mass = mass;
        this.isRoot = false;
        this.Angle = 0f;
        this.AngularVelocity = 0f;
        this.Torque = 0f;
        this.RestAngle = 0f;  // Initially set to 0, will be adjusted later
        this.ForceThreshold = 0.6f;
    }

    public StrandVertex()
    {
        this.Position = Vector3.zero;
        this.Velocity = Vector3.zero;
        this.Mass = 0;
        this.isRoot = false;
        this.Angle = 0f;
        this.AngularVelocity = 0f;
        this.Torque = 0f;
        this.RestAngle = 0f;  // Initially set to 0, will be adjusted later
        this.ForceThreshold = 0.6f;
    }
}