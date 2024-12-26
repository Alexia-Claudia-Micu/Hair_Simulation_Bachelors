using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
public class HairMesh : MonoBehaviour
{
    public HairStrand Strand;
    public Camera Camera;
    private Mesh hairMesh;

    void Start()
    {
        hairMesh = new Mesh ();
        GetComponent<MeshFilter> ().mesh = hairMesh;
    }

    void Update()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < Strand.Vertices.Count - 1; i++)
        {
            // 2 vertices
            Vector3 vertexFromPosition = Strand.Vertices[i].Position;
            Vector3 vertexToPosition = Strand.Vertices[i + 1].Position;

            // offset paralel to the camera so it's always the same thickness
            Vector3 parallelToCamera = Camera.transform.forward;

            // create an offset from those two vertices so we can make a rectangle (the thickness of the strand)
            Vector3 offset = Vector3.Cross(vertexToPosition - vertexFromPosition, parallelToCamera).normalized * 0.05f;

            // calculate points for rectangle
            Vector3 v1 = vertexFromPosition - offset;
            Vector3 v2 = vertexFromPosition + offset;
            Vector3 v3 = vertexToPosition - offset;
            Vector3 v4 = vertexToPosition + offset;

            // add vertices (positioned like a rectangle)
            vertices.Add (v1);
            vertices.Add (v2);
            vertices.Add (v3);
            vertices.Add (v4);

            // Create triangles with the vertices
            int startIndex = i * 4;
            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 1);

            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 3);
        }

        hairMesh.Clear();
        hairMesh.SetVertices(vertices);
        hairMesh.SetTriangles(triangles, 0);
        hairMesh.RecalculateNormals();
    }
}
