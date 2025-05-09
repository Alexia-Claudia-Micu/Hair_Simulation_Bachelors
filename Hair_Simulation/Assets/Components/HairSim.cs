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

    [Header("Compute & Rendering")]
    public ComputeShader hairComputeShader;
    public Material followerRenderMaterial;

    public ComputeBuffer leaderBuffer { get; private set; }
    public ComputeBuffer followerBuffer { get; private set; }
    public ComputeBuffer combinedRenderBuffer { get; private set; }
    public int vertexCountPerStrand { get; private set; }

    [Header("Follower Settings")]
    public int followerCount = 5;
    public float spawnRadius = 0.04f;
    [Range(0f, 1f)]
    public float taperAmount = 0.2f;
    public float hairThickness = 0.01f; // extra thickness param

    [Header("Hair Visual Thickness")]
    public float rootThickness = 0.02f;
    public float tipThickness = 0.01f;

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
            Debug.LogError("vertexCountPerStrand was not set.");
            return;
        }

        InitComputeBuffers();
    }

    void FixedUpdate()
    {
        int strandVertexCount = Strands.Count * vertexCountPerStrand;
        int followerCountTotal = Strands.Count * followerCount * vertexCountPerStrand;
        int guideCountTotal = strandVertexCount;
        int combinedCount = followerCountTotal + guideCountTotal;

        // Fill leader data (for compute shader)
        GPUVertex[] leaderVertices = new GPUVertex[strandVertexCount];
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

        // Allocate combined buffer
        if (combinedRenderBuffer == null || combinedRenderBuffer.count != combinedCount)
        {
            combinedRenderBuffer?.Release();
            combinedRenderBuffer = new ComputeBuffer(combinedCount, sizeof(float) * 3);
        }

        // Dispatch compute shader if followers are needed
        if (followerCount > 0)
        {
            int kernel = hairComputeShader.FindKernel("CSMain");
            hairComputeShader.SetInt("vertexCountPerStrand", vertexCountPerStrand);
            hairComputeShader.SetInt("followerCount", followerCount);
            hairComputeShader.SetFloat("spawnRadius", spawnRadius);
            hairComputeShader.SetFloat("taperAmount", taperAmount);
            hairComputeShader.SetBuffer(kernel, "LeaderVertices", leaderBuffer);
            hairComputeShader.SetBuffer(kernel, "FollowerPositions", combinedRenderBuffer);
            hairComputeShader.Dispatch(kernel, Mathf.CeilToInt(followerCountTotal / 32f), 1, 1);
        }

        // Copy guide data into the second half of the combined buffer
        Vector3[] guideData = new Vector3[guideCountTotal];
        for (int s = 0; s < Strands.Count; s++)
        {
            for (int v = 0; v < vertexCountPerStrand; v++)
            {
                int idx = s * vertexCountPerStrand + v;
                guideData[idx] = Strands[s].Vertices[v].Position;
            }
        }
        combinedRenderBuffer.SetData(guideData, 0, followerCountTotal, guideCountTotal);

        // Update root positions
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
        if (followerRenderMaterial == null || combinedRenderBuffer == null) return;

        int strandCount = Strands.Count * (followerCount + 1); // +1 for guide
        int segmentCount = vertexCountPerStrand - 1;

        followerRenderMaterial.SetBuffer("FollowerPositions", combinedRenderBuffer);
        followerRenderMaterial.SetInt("_VertexCountPerStrand", vertexCountPerStrand);
        followerRenderMaterial.SetFloat("_RootThickness", rootThickness);
        followerRenderMaterial.SetFloat("_TipThickness", tipThickness);

        Graphics.DrawProcedural(
            followerRenderMaterial,
            new Bounds(Vector3.zero, Vector3.one * 100f),
            MeshTopology.Triangles,
            strandCount * segmentCount * 6
        );
    }


    void OnDestroy()
    {
        leaderBuffer?.Release();
        followerBuffer?.Release();
        combinedRenderBuffer?.Release();
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
            Vector3 localRootNormal = sphere.transform.InverseTransformDirection((rootPosition - sphere.transform.position).normalized);

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
        int followerVertexCount = Strands.Count * followerCount * vertexCountPerStrand;
        int totalCombinedVertices = leaderVertexCount + followerVertexCount;

        // Protect against invalid buffer sizes
        if (leaderVertexCount <= 0)
        {
            Debug.LogWarning("Leader buffer not initialized: no strands or vertices.");
            return;
        }

        leaderBuffer?.Release();
        leaderBuffer = new ComputeBuffer(leaderVertexCount, sizeof(float) * 7); // pos+vel+angle

        if (totalCombinedVertices > 0)
        {
            followerBuffer?.Release();
            followerBuffer = new ComputeBuffer(totalCombinedVertices, sizeof(float) * 3); // position only
        }
        else
        {
            Debug.LogWarning("Follower buffer not created: zero total vertices.");
        }
    }


    Vector3 GetRandomPointOnSphereSurface()
    {
        Rigidbody rb = sphere.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on emitter object.");
            return sphere.transform.position;
        }

        float radius = sphere.transform.lossyScale.x * 0.5f; // assuming uniform scale
        Vector3 direction = Random.onUnitSphere;
        return sphere.transform.position + direction * radius;
    }

    public struct GPUVertex
    {
        public Vector3 position;
        public Vector3 velocity;
        public float angle;
    }
}
