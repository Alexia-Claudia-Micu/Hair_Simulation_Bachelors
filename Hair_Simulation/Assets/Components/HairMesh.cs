using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class HairMeshBatchRendererGPU : MonoBehaviour
{
    public HairSim hairSim;
    public Material hairMaterial;
    public int radialSegments = 4;
    public float rootThickness = 0.05f;
    public float tipThickness = 0.03f;

    private Mesh batchedMesh;
    private Vector3[] gpuFollowerPositions;

    void Start()
    {
        batchedMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = batchedMesh;
        GetComponent<MeshRenderer>().material = hairMaterial;
    }

    void Update()
    {
        if (hairSim == null || hairSim.Strands.Count == 0) return;
        if (hairSim.followerBuffer == null) return;

        int totalVertices = hairSim.Strands.Count * hairSim.followerCount * hairSim.vertexCountPerStrand;

        if (gpuFollowerPositions == null || gpuFollowerPositions.Length != totalVertices)
            gpuFollowerPositions = new Vector3[totalVertices];

        // Pull the GPU data back into CPU array
        hairSim.followerBuffer.GetData(gpuFollowerPositions);

        GenerateBatchedMesh();
    }

    void GenerateBatchedMesh()
    {
        List<Vector3> vertices = new();
        List<int> triangles = new();
        int vertexOffset = 0;

        int strandCount = hairSim.Strands.Count * hairSim.followerCount;
        int verticesPerStrand = hairSim.vertexCountPerStrand;

        for (int s = 0; s < strandCount; s++)
        {
            for (int v = 0; v < verticesPerStrand - 1; v++)
            {
                int idx0 = s * verticesPerStrand + v;
                int idx1 = s * verticesPerStrand + v + 1;

                Vector3 from = gpuFollowerPositions[idx0];
                Vector3 to = gpuFollowerPositions[idx1];
                Vector3 dir = (to - from).normalized;

                Vector3 p1 = Vector3.Cross(dir, Vector3.up).normalized;
                Vector3 p2 = Vector3.Cross(dir, p1).normalized;
                if (p1 == Vector3.zero) p1 = Vector3.right;
                if (p2 == Vector3.zero) p2 = Vector3.forward;

                float t = (float)v / (verticesPerStrand - 1);
                float thickness = Mathf.Lerp(rootThickness, tipThickness, t);

                for (int j = 0; j < radialSegments; j++)
                {
                    float angle = (j / (float)radialSegments) * Mathf.PI * 2;
                    Vector3 radial = (p1 * Mathf.Cos(angle) + p2 * Mathf.Sin(angle)) * thickness;

                    vertices.Add(from + radial);
                    vertices.Add(to + radial);
                }

                for (int j = 0; j < radialSegments; j++)
                {
                    int next = (j + 1) % radialSegments;
                    int baseIndex = vertexOffset + j * 2;
                    int nextBase = vertexOffset + next * 2;

                    triangles.Add(baseIndex);
                    triangles.Add(baseIndex + 1);
                    triangles.Add(nextBase);

                    triangles.Add(nextBase);
                    triangles.Add(baseIndex + 1);
                    triangles.Add(nextBase + 1);
                }

                vertexOffset += radialSegments * 2;
            }
        }

        batchedMesh.Clear();
        batchedMesh.SetVertices(vertices);
        batchedMesh.SetTriangles(triangles, 0);
        batchedMesh.RecalculateNormals();
    }
}
