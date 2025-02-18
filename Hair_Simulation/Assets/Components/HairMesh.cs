using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class HairMesh : MonoBehaviour
{
    public HairStrand Strand;
    public Camera Camera;
    private Mesh hairMesh;

    void Start()
    {
        hairMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = hairMesh;
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
            // Hair segment vertices
            Vector3 vertexFromPosition = Strand.Vertices[i].Position;
            Vector3 vertexToPosition = Strand.Vertices[i + 1].Position;

            // Offset perpendicular to the camera's view direction
            Vector3 parallelToCamera = Camera.transform.forward;
            Vector3 direction = (vertexToPosition - vertexFromPosition).normalized;
            Vector3 offset = Vector3.Cross(direction, parallelToCamera).normalized;

            // **Tapering Factor**: Prevents going to 0 by clamping
            float taperFactor = 1.0f - ((float)i / (Strand.Vertices.Count - 1));
            float clampedTaper = Mathf.Max(Constants.MinHairTaper, taperFactor); // Ensures min thickness

            // Adjusted thickness with tapering
            float segmentThickness = Constants.HairThickness * clampedTaper;

            // Adjust vertex positions based on tapering
            Vector3 v1 = vertexFromPosition - offset * segmentThickness;
            Vector3 v2 = vertexFromPosition + offset * segmentThickness;
            Vector3 v3 = vertexToPosition - offset * segmentThickness;
            Vector3 v4 = vertexToPosition + offset * segmentThickness;

            // Add vertices to the mesh
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);

            // Create triangles (two per quad)
            int startIndex = i * 4;
            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 1);

            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 3);
        }

        // Update mesh data
        hairMesh.Clear();
        hairMesh.SetVertices(vertices);
        hairMesh.SetTriangles(triangles, 0);
        hairMesh.RecalculateNormals();
    }
}
