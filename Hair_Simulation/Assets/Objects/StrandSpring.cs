using UnityEngine;

public class StrandSpring
{
    public int VertexFrom;
    public int VertexTo;
    public float RestLength;
    public float Stiffness;
    public float Damping;

    public StrandSpring(int VertexFrom, int VertexTo, float RestLength, float Stiffness, float Damping)
    {
        this.VertexFrom = VertexFrom;
        this.VertexTo = VertexTo;
        this.RestLength = RestLength;
        this.Stiffness = Stiffness;
        this.Damping = Damping;
    }

    public StrandSpring()
    {
        this.VertexFrom=0;
        this.VertexTo=0;
        this.RestLength=0;
        this.Stiffness=0;
        this.Damping=0;
    }

}
