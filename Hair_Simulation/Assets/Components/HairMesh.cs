using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class HairMesh : MonoBehaviour
{
    public HairStrand Strand;
    private Mesh hairMesh;

    public int radialSegments = 4; // Number of vertices around each segment (more = smoother tube)
    public float rootThickness = 0.05f; // Thickness at the root
    public float tipThickness = 0.03f;  // Thickness at the tip

    void Start()
    {
        if (Strand == null)
        {
            Debug.LogError("HairStrand reference is missing!");
            return;
        }

        hairMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = hairMesh;
    }

    void Update()
    {
        if (Strand.Vertices.Count > 1) // Ensure enough vertices exist
        {
            GenerateMesh();
        }
    }

    void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < Strand.Vertices.Count - 1; i++)
        {
            // Hair segment start and end positions
            Vector3 vertexFromPosition = Strand.Vertices[i].Position;
            Vector3 vertexToPosition = Strand.Vertices[i + 1].Position;
            Vector3 segmentDirection = (vertexToPosition - vertexFromPosition).normalized;

            // Generate a perpendicular basis for the tube shape
            Vector3 perpendicular1 = Vector3.Cross(segmentDirection, Vector3.up).normalized;
            Vector3 perpendicular2 = Vector3.Cross(segmentDirection, perpendicular1).normalized;

            if (perpendicular1 == Vector3.zero) perpendicular1 = Vector3.right; // Fix degeneracy
            if (perpendicular2 == Vector3.zero) perpendicular2 = Vector3.forward; // Fix degeneracy

            // Compute tapering factor (interpolates between rootThickness and tipThickness)
            float taperFactor = (float)i / (Strand.Vertices.Count - 1);
            float taperedThickness = Mathf.Lerp(rootThickness, tipThickness, taperFactor);

            // Generate circular cross-section with tapering
            for (int j = 0; j < radialSegments; j++)
            {
                float angle = (j / (float)radialSegments) * Mathf.PI * 2;
                Vector3 radialOffset = (perpendicular1 * Mathf.Cos(angle) + perpendicular2 * Mathf.Sin(angle)) * taperedThickness;

                vertices.Add(vertexFromPosition + radialOffset);
                vertices.Add(vertexToPosition + radialOffset);
            }

            // Create triangles connecting cross-sections
            int baseIndex = i * radialSegments * 2;
            for (int j = 0; j < radialSegments; j++)
            {
                int nextJ = (j + 1) % radialSegments;

                int v1 = baseIndex + j * 2;
                int v2 = baseIndex + nextJ * 2;
                int v3 = baseIndex + j * 2 + 1;
                int v4 = baseIndex + nextJ * 2 + 1;

                triangles.Add(v1);
                triangles.Add(v3);
                triangles.Add(v2);

                triangles.Add(v2);
                triangles.Add(v3);
                triangles.Add(v4);
            }
        }

        // Update the mesh data
        hairMesh.Clear();
        hairMesh.SetVertices(vertices);
        hairMesh.SetTriangles(triangles, 0);
        hairMesh.RecalculateNormals();
        hairMesh.RecalculateBounds();
    }
}
