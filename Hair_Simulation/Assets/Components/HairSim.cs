using System.Collections.Generic;
using UnityEngine;

public class HairSim : MonoBehaviour
{
    [Header("Hair Strand Prefab & Emitter")]
    public GameObject hairStrandPrefab;
    public GameObject sphere;

    [Header("Hair Cluster Settings")]
    public int strandCount = 20;

    [Header("Segment Settings")]
    public float baseSegmentLength = 0.5f;
    [Range(0f, 1f)] public float segmentLengthRandomness = 0.2f;

    [Header("Vertex Count")]
    public int baseVertexCount = 4;
    [Range(0f, 1f)] public float vertexCountRandomness = 0.25f;

    [Header("Curl Frequency")]
    public float baseCurlFrequency = 0.6f;
    [Range(0f, 1f)] public float curlFrequencyRandomness = 0.5f;

    [Header("Curl Diameter")]
    public float baseCurlDiameter = 0.02f;
    [Range(0f, 1f)] public float curlDiameterRandomness = 0.5f;

    private List<Vector3> localRootPositions = new List<Vector3>();

    public List<HairStrand> Strands { get; private set; } = new();

    public ComputeShader hairComputeShader;
    public Material followerRenderMaterial;

    public ComputeBuffer leaderBuffer { get; private set; }
    public ComputeBuffer followerBuffer { get; private set; }
    public int vertexCountPerStrand { get; private set; }

    [Header("Follower Settings")]
    public int followerCount = 5;

    public float spawnRadius = 0.04f;

    [Range(0f, 1f)]
    public float taperAmount = 0.2f; // New field, default 0.2


    void Start()
    {
        if (sphere == null)
        {
            Debug.LogError("Sphere object (emitter) is not assigned to HairSim.");
            return;
        }

        GenerateHairStrands();

        if (vertexCountPerStrand == 0)
        {
            Debug.LogError("vertexCountPerStrand was not set. Make sure at least one strand was generated.");
            return;
        }

        InitComputeBuffers();
    }

    void FixedUpdate()
    {
        int leaderCount = Strands.Count * vertexCountPerStrand;
        GPUVertex[] leaderVertices = new GPUVertex[leaderCount];

        for (int s = 0; s < Strands.Count; s++)
        {
            var strand = Strands[s];
            for (int v = 0; v < vertexCountPerStrand; v++)
            {
                int idx = s * vertexCountPerStrand + v;
                var vert = strand.Vertices[v];
                leaderVertices[idx] = new GPUVertex
                {
                    position = vert.Position,
                    velocity = vert.Velocity,
                    angle = vert.Angle
                };
            }
        }

        leaderBuffer.SetData(leaderVertices);

        int totalFollowerVertices = Strands.Count * followerCount * vertexCountPerStrand;
        int kernel = hairComputeShader.FindKernel("CSMain");

        // Set shader constants
        hairComputeShader.SetInt("vertexCountPerStrand", vertexCountPerStrand);
        hairComputeShader.SetInt("followerCount", followerCount);
        hairComputeShader.SetFloat("spawnRadius", spawnRadius); // <-- Here: Set your adjustable follower spawn radius
        hairComputeShader.SetFloat("taperAmount", taperAmount); // <-- add this


        hairComputeShader.SetBuffer(kernel, "LeaderVertices", leaderBuffer);
        hairComputeShader.SetBuffer(kernel, "FollowerPositions", followerBuffer);

        int threadGroups = Mathf.CeilToInt(totalFollowerVertices / 32f);
        hairComputeShader.Dispatch(kernel, threadGroups, 1, 1);

        // Update guide strand roots
        for (int i = 0; i < Strands.Count; i++)
        {
            if (Strands[i] != null)
            {
                Vector3 newWorldRoot = sphere.transform.TransformPoint(localRootPositions[i]);
                Strands[i].UpdateRootPosition(newWorldRoot);
            }
        }
    }

    void LateUpdate()
    {
        if (followerRenderMaterial == null) return;

        followerRenderMaterial.SetBuffer("FollowerPositions", followerBuffer);

        Graphics.DrawProcedural(
            followerRenderMaterial,
            new Bounds(Vector3.zero, Vector3.one * 100f),
            MeshTopology.LineStrip,                        // <--- IMPORTANT
            vertexCountPerStrand,                          // vertices per strand
            Strands.Count * followerCount                  // how many strands
        );
    }


    void OnDestroy()
    {
        if (leaderBuffer != null)
        {
            leaderBuffer.Release();
            leaderBuffer = null;
        }

        if (followerBuffer != null)
        {
            followerBuffer.Release();
            followerBuffer = null;
        }
    }

    void GenerateHairStrands()
    {
        if (hairStrandPrefab == null)
        {
            Debug.LogError("HairStrand prefab is not assigned.");
            return;
        }

        for (int i = 0; i < strandCount; i++)
        {
            Vector3 rootPosition = GetRandomPointOnSphereSurface();
            Vector3 localRootPosition = sphere.transform.InverseTransformPoint(rootPosition);

            float segmentLength = baseSegmentLength * Random.Range(1f - segmentLengthRandomness, 1f + segmentLengthRandomness);
            int numberOfVertices = Mathf.RoundToInt(baseVertexCount * Random.Range(1f - vertexCountRandomness, 1f + vertexCountRandomness));
            numberOfVertices = Mathf.Max(2, numberOfVertices);

            float curlFrequency = baseCurlFrequency * Random.Range(1f - curlFrequencyRandomness, 1f + curlFrequencyRandomness);
            float curlDiameter = baseCurlDiameter * Random.Range(1f - curlDiameterRandomness, 1f + curlDiameterRandomness);

            GameObject strandObject = Instantiate(hairStrandPrefab, Vector3.zero, Quaternion.identity);
            HairStrand hairStrand = strandObject.GetComponent<HairStrand>();

            if (hairStrand != null)
            {
                hairStrand.emitter = sphere;
                hairStrand.InitializeHairStrand(rootPosition, segmentLength, numberOfVertices, curlFrequency, curlDiameter);
                Strands.Add(hairStrand);
                localRootPositions.Add(localRootPosition);

                if (vertexCountPerStrand == 0)
                    vertexCountPerStrand = numberOfVertices;
            }
        }
    }

    void InitComputeBuffers()
    {
        int leaderVertexCount = Strands.Count * vertexCountPerStrand;
        int totalFollowerVertices = Strands.Count * followerCount * vertexCountPerStrand;

        leaderBuffer = new ComputeBuffer(leaderVertexCount, sizeof(float) * 7); // pos(3) + vel(3) + angle(1)
        followerBuffer = new ComputeBuffer(totalFollowerVertices, sizeof(float) * 3); // pos only
    }

    Vector3 GetRandomPointOnSphereSurface()
    {
        SphereCollider sphereCollider = sphere.GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            Debug.LogError("Sphere does not have a SphereCollider.");
            return sphere.transform.position;
        }

        float sphereRadius = sphereCollider.radius * sphere.transform.lossyScale.x;
        Vector3 randomDirection = Random.onUnitSphere;
        return sphere.transform.position + randomDirection * sphereRadius;
    }

    public struct GPUVertex
    {
        public Vector3 position;
        public Vector3 velocity;
        public float angle;
    }
}
