using System.Collections.Generic;
using UnityEngine;

public class HairSimCore : MonoBehaviour
{
    [Header("Compute & Rendering")]
    public ComputeShader hairComputeShader;
    public Material followerRenderMaterial;

    [Header("Follower Settings")]
    public int followerCount = 5;
    public float spawnRadius = 0.04f;
    [Range(0f, 1f)] public float taperAmount = 0.2f;
    public float rootThickness = 0.02f;
    public float tipThickness = 0.01f;

    protected List<HairStrand> strands = new();
    protected ComputeBuffer leaderBuffer;
    protected ComputeBuffer combinedRenderBuffer;
    protected ComputeBuffer strandInfoBuffer;
    protected ComputeBuffer segmentRenderInfoBuffer;

    public struct GPUVertex
    {
        public Vector3 position;
        public Vector3 velocity;
        public float angle;
    }

    public struct StrandInfo
    {
        public int leaderStartIndex;
        public int vertexCount;
        public int followerStartIndex;
    }

    public struct SegmentRenderInfo
    {
        public int startIndex;
        public int segmentIndex;
        public int segmentCount;
    }

    protected List<StrandInfo> strandInfos = new();
    protected List<SegmentRenderInfo> segmentRenderInfos = new();

    protected int totalGuideVerts = 0;
    protected int totalFollowerVerts = 0;
    protected int totalRenderVerts = 0;
    protected int totalSegmentQuads = 0;

    private int lastFollowerCount = -1;

    public void Initialize(List<HairStrand> strands, int _)
    {
        this.strands = strands;
        BuildStrandInfos();
        InitComputeBuffers();
        lastFollowerCount = followerCount;
    }

    void BuildStrandInfos()
    {
        strandInfos.Clear();
        segmentRenderInfos.Clear();
        totalGuideVerts = 0;
        totalFollowerVerts = 0;

        foreach (var strand in strands)
        {
            int vertexCount = strand.Vertices.Count;

            strandInfos.Add(new StrandInfo
            {
                leaderStartIndex = totalGuideVerts,
                vertexCount = vertexCount,
                followerStartIndex = totalFollowerVerts
            });

            totalGuideVerts += vertexCount;
            totalFollowerVerts += vertexCount * followerCount;
        }

        totalRenderVerts = totalGuideVerts + totalFollowerVerts;

        foreach (var info in strandInfos)
        {
            int segCount = info.vertexCount - 1;

            for (int f = 0; f < followerCount; f++)
            {
                int strandStart = info.followerStartIndex + f * info.vertexCount;
                for (int s = 0; s < segCount; s++)
                {
                    segmentRenderInfos.Add(new SegmentRenderInfo
                    {
                        startIndex = strandStart + s,
                        segmentIndex = s,
                        segmentCount = segCount
                    });
                }
            }

            int guideStart = totalFollowerVerts + info.leaderStartIndex;
            for (int s = 0; s < segCount; s++)
            {
                segmentRenderInfos.Add(new SegmentRenderInfo
                {
                    startIndex = guideStart + s,
                    segmentIndex = s,
                    segmentCount = segCount
                });
            }
        }

        totalSegmentQuads = segmentRenderInfos.Count;
    }

    int ComputeTotalSegmentQuads()
    {
        int quads = 0;
        foreach (var info in strandInfos)
        {
            int segCount = info.vertexCount - 1;
            quads += segCount * (followerCount + 1);
        }
        return quads;
    }

    void InitComputeBuffers()
    {
        leaderBuffer?.Release();
        if (totalGuideVerts > 0)
            leaderBuffer = new ComputeBuffer(totalGuideVerts, sizeof(float) * 7);

        strandInfoBuffer?.Release();
        if (strandInfos.Count > 0)
            strandInfoBuffer = new ComputeBuffer(strandInfos.Count, sizeof(int) * 3);

        totalFollowerVerts = totalGuideVerts * followerCount;
        totalRenderVerts = totalGuideVerts + totalFollowerVerts;

        combinedRenderBuffer?.Release();
        if (totalRenderVerts > 0)
            combinedRenderBuffer = new ComputeBuffer(totalRenderVerts, sizeof(float) * 3);

        totalSegmentQuads = ComputeTotalSegmentQuads();

        segmentRenderInfoBuffer?.Release();
        if (totalSegmentQuads > 0)
            segmentRenderInfoBuffer = new ComputeBuffer(totalSegmentQuads, sizeof(int) * 3);
    }

    void FixedUpdate()
    {
        if (strands.Count == 0 || totalGuideVerts == 0) return;

        if (lastFollowerCount != followerCount)
        {
            BuildStrandInfos();
            InitComputeBuffers();
            lastFollowerCount = followerCount;
        }

        List<GPUVertex> packedLeaders = new();
        foreach (var strand in strands)
        {
            foreach (var vert in strand.Vertices)
            {
                packedLeaders.Add(new GPUVertex
                {
                    position = vert.Position,
                    velocity = vert.Velocity,
                    angle = vert.Angle
                });
            }
        }

        leaderBuffer.SetData(packedLeaders);
        strandInfoBuffer.SetData(strandInfos);

        if (followerCount > 0)
        {
            int kernel = hairComputeShader.FindKernel("CSMain");

            hairComputeShader.SetInt("followerCount", followerCount);
            hairComputeShader.SetFloat("spawnRadius", spawnRadius);
            hairComputeShader.SetFloat("taperAmount", taperAmount);
            hairComputeShader.SetInt("strandCount", strandInfos.Count);

            hairComputeShader.SetBuffer(kernel, "LeaderVertices", leaderBuffer);
            hairComputeShader.SetBuffer(kernel, "StrandInfos", strandInfoBuffer);
            hairComputeShader.SetBuffer(kernel, "FollowerPositions", combinedRenderBuffer);

            int dispatchCount = Mathf.CeilToInt(strandInfos.Count * followerCount / 32f);
            hairComputeShader.Dispatch(kernel, dispatchCount, 1, 1);
        }

        Vector3[] guideData = new Vector3[totalGuideVerts];
        int index = 0;
        foreach (var strand in strands)
        {
            foreach (var vert in strand.Vertices)
            {
                guideData[index++] = vert.Position;
            }
        }

        combinedRenderBuffer.SetData(guideData, 0, totalFollowerVerts, totalGuideVerts);
        segmentRenderInfoBuffer.SetData(segmentRenderInfos);
    }

    void LateUpdate()
    {
        if (combinedRenderBuffer == null || followerRenderMaterial == null || segmentRenderInfoBuffer == null)
            return;

        followerRenderMaterial.SetBuffer("FollowerPositions", combinedRenderBuffer);
        followerRenderMaterial.SetBuffer("SegmentRenderInfos", segmentRenderInfoBuffer);
        followerRenderMaterial.SetFloat("_RootThickness", rootThickness);
        followerRenderMaterial.SetFloat("_TipThickness", tipThickness);

        Graphics.DrawProcedural(
            followerRenderMaterial,
            new Bounds(Vector3.zero, Vector3.one * 100),
            MeshTopology.Triangles,
            totalSegmentQuads * 6
        );
    }

    void OnDestroy()
    {
        leaderBuffer?.Release();
        combinedRenderBuffer?.Release();
        strandInfoBuffer?.Release();
        segmentRenderInfoBuffer?.Release();
    }
}
