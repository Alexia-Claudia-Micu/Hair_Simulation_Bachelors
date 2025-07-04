//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(MeshFilter))]
//public class HairMeshBatchRenderer : MonoBehaviour
//{
//    public HairSimCore hairSim;
//    public Material hairMaterial;
//    public int radialSegments = 4;
//    public float rootThickness = 0.05f;
//    public float tipThickness = 0.03f;

//    private Mesh batchedMesh;

//    void Start()
//    {
//        batchedMesh = new Mesh();
//        GetComponent<MeshFilter>().mesh = batchedMesh;
//        GetComponent<MeshRenderer>().material = hairMaterial;
//    }

//    void Update()
//    {
//        if (hairSim == null) return;
//        GenerateBatchedMesh();
//    }

//    void GenerateBatchedMesh()
//    {
//        List<Vector3> vertices = new();
//        List<int> triangles = new();
//        int vertexOffset = 0;

//        foreach (HairStrand strand in hairSim.Strands)
//        {
//            if (strand.Vertices.Count < 2) continue;

//            for (int i = 0; i < strand.Vertices.Count - 1; i++)
//            {
//                Vector3 from = strand.Vertices[i].Position;
//                Vector3 to = strand.Vertices[i + 1].Position;
//                Vector3 dir = (to - from).normalized;

//                Vector3 p1 = Vector3.Cross(dir, Vector3.up).normalized;
//                Vector3 p2 = Vector3.Cross(dir, p1).normalized;
//                if (p1 == Vector3.zero) p1 = Vector3.right;
//                if (p2 == Vector3.zero) p2 = Vector3.forward;

//                float t = (float)i / (strand.Vertices.Count - 1);
//                float thickness = Mathf.Lerp(rootThickness, tipThickness, t);

//                for (int j = 0; j < radialSegments; j++)
//                {
//                    float angle = (j / (float)radialSegments) * Mathf.PI * 2;
//                    Vector3 radial = (p1 * Mathf.Cos(angle) + p2 * Mathf.Sin(angle)) * thickness;

//                    vertices.Add(from + radial);
//                    vertices.Add(to + radial);
//                }

//                for (int j = 0; j < radialSegments; j++)
//                {
//                    int next = (j + 1) % radialSegments;
//                    int baseIndex = vertexOffset + j * 2;
//                    int nextBase = vertexOffset + next * 2;

//                    triangles.Add(baseIndex);
//                    triangles.Add(baseIndex + 1);
//                    triangles.Add(nextBase);

//                    triangles.Add(nextBase);
//                    triangles.Add(baseIndex + 1);
//                    triangles.Add(nextBase + 1);
//                }

//                vertexOffset += radialSegments * 2;
//            }
//        }

//        batchedMesh.Clear();
//        batchedMesh.SetVertices(vertices);
//        batchedMesh.SetTriangles(triangles, 0);
//        batchedMesh.RecalculateNormals();
//    }
//}
