using UnityEngine;

public class StrandSpring
{
    public int VertexFrom;
    public int VertexTo;
    public float Length;

    public StrandSpring(int VertexFrom, int VertexTo, float length)
    {
        this.VertexFrom = VertexFrom;
        this.VertexTo = VertexTo;
        this.Length = length;
    }

    public StrandSpring()
    {
        this.VertexFrom=0;
        this.VertexTo=0;
        this.Length = 0;
    }

}
