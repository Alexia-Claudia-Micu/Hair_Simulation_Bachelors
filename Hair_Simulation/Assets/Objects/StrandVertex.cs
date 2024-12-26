using UnityEngine;

public class StrandVertex
{
    public Vector3 Position;
    public Vector3 Velocity;
    public float Mass;
    public bool isRoot;

    public StrandVertex(Vector3 position, float mass)
    {
        this.Position = position;
        this.Velocity = new Vector3(0, 0, 0);
        this.Mass = mass;
    }

    public StrandVertex(Vector3 position, float mass, bool isRoot)
    {
        this.Position = position;
        this.Velocity = new Vector3(0, 0, 0);
        this.Mass = mass;
        this.isRoot = isRoot;
    }

    public StrandVertex()
    {
        this.Position = new Vector3(0, 0, 0);
        this.Velocity = new Vector3(0, 0, 0);
        this.Mass = 0;
        this.isRoot = false;
    }

}
