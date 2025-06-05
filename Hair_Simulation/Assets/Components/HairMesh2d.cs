//using System.Collections.Generic;
//using UnityEngine;

//// TODO doesnt work for rotation

//[RequireComponent(typeof(MeshFilter))]
//public class HairMesh2D : MonoBehaviour
//{
//    public HairStrand Strand;
//    public Camera Camera;
//    private Mesh hairMesh;

//    void Start()
//    {
//        if (Strand == null)
//        {
//            Debug.LogError("HairStrand reference is missing!");
//            return;
//        }
//        if (Camera == null)
//        {
//            Camera = Camera.main; // Assign main camera if not set
//        }

//        hairMesh = new Mesh();
//        GetComponent<MeshFilter>().mesh = hairMesh;
//    }

//    void Update()
//    {
//        if (Strand.Vertices.Count > 1) // Ensure enough vertices exist
//        {
//            GenerateMesh();
//        }
//    }

//    void GenerateMesh()
//    {
//        List<Vector3> vertices = new List<Vector3>();
//        List<int> triangles = new List<int>();

//        float minThickness = Constants.HairThickness * 0.5f; // Ensures strands remain visible

//        for (int i = 0; i < Strand.Vertices.Count - 1; i++)
//        {
//            // Hair segment start and end positions
//            Vector3 vertexFromPosition = Strand.Vertices[i].Position;
//            Vector3 vertexToPosition = Strand.Vertices[i + 1].Position;

//            // Correct way to find perpendicular offset so that the hair is always facing the camera
//            Vector3 viewDirection = (Camera.transform.position - vertexFromPosition).normalized; // Direction towards camera
//            Vector3 strandDirection = (vertexToPosition - vertexFromPosition).normalized; // Hair strand direction
//            Vector3 perpendicularOffset = Vector3.Cross(viewDirection, strandDirection).normalized; // Adjusted perpendicular

//            // Compute tapering factor and clamp it
//            float taperFactor = 1.0f - ((float)i / (Strand.Vertices.Count - 1));
//            float clampedTaper = Mathf.Max(Constants.MinHairTaper, taperFactor); // Ensures min taper
//            float segmentThickness = Mathf.Max(Constants.HairThickness * clampedTaper, minThickness);

//            // Offset hair segment vertices to create a quad
//            Vector3 v1 = vertexFromPosition - perpendicularOffset * segmentThickness;
//            Vector3 v2 = vertexFromPosition + perpendicularOffset * segmentThickness;
//            Vector3 v3 = vertexToPosition - perpendicularOffset * segmentThickness;
//            Vector3 v4 = vertexToPosition + perpendicularOffset * segmentThickness;

//            // Add vertices to the mesh
//            vertices.Add(v1);
//            vertices.Add(v2);
//            vertices.Add(v3);
//            vertices.Add(v4);

//            // Add triangles to connect the vertices into quads
//            int startIndex = i * 4;
//            triangles.Add(startIndex + 0);
//            triangles.Add(startIndex + 2);
//            triangles.Add(startIndex + 1);

//            triangles.Add(startIndex + 1);
//            triangles.Add(startIndex + 2);
//            triangles.Add(startIndex + 3);
//        }

//        // Update the mesh data
//        hairMesh.Clear();
//        hairMesh.SetVertices(vertices);
//        hairMesh.SetTriangles(triangles, 0);
//        hairMesh.RecalculateNormals();
//        hairMesh.RecalculateBounds();
//    }

//}
