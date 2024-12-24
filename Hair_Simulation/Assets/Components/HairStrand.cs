using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HairStrand : MonoBehaviour
{
    public List<StrandVertex> Vertices;
    public List<StrandSpring> Springs;

    LineRenderer lineRenderer;

    void Start()
    {
        this.Vertices = new List<StrandVertex>();
        this.Springs = new List<StrandSpring>();

        StrandVertex vertex0 = new StrandVertex(new Vector3(0,6,0), new Vector3(0,0,0), 1.0f); this.Vertices.Add(vertex0);
        StrandVertex vertex1 = new StrandVertex(new Vector3(1,5,0), new Vector3(0,0,0), 1.0f); this.Vertices.Add(vertex1);
        StrandVertex vertex2 = new StrandVertex(new Vector3(2,4,0), new Vector3(0,0,0), 1.0f); this.Vertices.Add(vertex2);
        StrandVertex vertex3 = new StrandVertex(new Vector3(3,3,0), new Vector3(0,0,0), 1.0f); this.Vertices.Add(vertex3);
        StrandVertex vertex4 = new StrandVertex(new Vector3(0,2,0), new Vector3(0,0,0), 1.0f); this.Vertices.Add(vertex4);

        StrandSpring spring0 = new StrandSpring(0,1,0.5f, 0.5f, 0.5f); this.Springs.Add(spring0);
        StrandSpring spring1 = new StrandSpring(1,2,0.5f, 0.5f, 0.5f); this.Springs.Add(spring1);
        StrandSpring spring2 = new StrandSpring(2,3,0.5f, 0.5f, 0.5f); this.Springs.Add(spring2);
        StrandSpring spring3 = new StrandSpring(3,4,0.5f, 0.5f, 0.5f); this.Springs.Add(spring3);

        // Get the LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("No LineRenderer component found!");
        }

        lineRenderer.positionCount = this.Vertices.Count;
        for(int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, this.Vertices[i].Position);
        }
    }

    void Update()
    {
        
    }
}
