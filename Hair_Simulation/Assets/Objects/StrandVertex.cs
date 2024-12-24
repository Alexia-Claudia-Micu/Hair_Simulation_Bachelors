using UnityEngine;

public class StrandVertex
{
    public Vector3 Position;
    public Vector3 Velocity;
    public float Mass;

    public StrandVertex(Vector3 position, Vector3 velocity, float mass)
    {
        this.Position = position;
        this.Velocity = velocity;
        this.Mass = mass;
    }

    public StrandVertex()
    {
        this.Position = new Vector3();
        this.Velocity = new Vector3();
        this.Mass = 0;
    }

}
