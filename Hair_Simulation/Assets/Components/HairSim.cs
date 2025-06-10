using UnityEngine;
using System.Collections.Generic;

public class HairSim : HairSimCore
{
    [Header("Hair Strand Prefab & Emitter")]
    public GameObject hairStrandPrefab;
    public GameObject sphere;

    [Header("Settings JSON")]
    public TextAsset defaultSettingsJson;

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

    void Start()
    {
        LoadSettingsFromJson();

        var generatedStrands = new List<HairStrand>();
        int vertexCount = 0;

        for (int i = 0; i < strandCount; i++)
        {
            Vector3 root = GetRandomPointOnSphereSurface();
            float segmentLen = baseSegmentLength * Random.Range(1f - segmentLengthRandomness, 1f + segmentLengthRandomness);
            int verts = Mathf.RoundToInt(baseVertexCount * Random.Range(1f - vertexCountRandomness, 1f + vertexCountRandomness));
            verts = Mathf.Max(2, verts);

            float freq = baseCurlFrequency * Random.Range(1f - curlFrequencyRandomness, 1f + curlFrequencyRandomness);
            float diameter = baseCurlDiameter * Random.Range(1f - curlDiameterRandomness, 1f + curlDiameterRandomness);

            var strandObj = Instantiate(hairStrandPrefab, Vector3.zero, Quaternion.identity);
            var strand = strandObj.GetComponent<HairStrand>();
            if (strand != null)
            {
                strand.emitter = sphere;
                strand.InitializeHairStrand(root, segmentLen, verts, freq, diameter);
                generatedStrands.Add(strand);
                vertexCount = verts;
            }
        }

        Initialize(generatedStrands, vertexCount);
    }

    void LoadSettingsFromJson()
    {
        TextAsset jsonToLoad = InterSceneStatics.SelectedSettingsJson != null
            ? InterSceneStatics.SelectedSettingsJson
            : defaultSettingsJson;

        if (jsonToLoad == null)
        {
            Debug.LogWarning("No settings JSON provided.");
            return;
        }

        HairSimSettings settings = JsonUtility.FromJson<HairSimSettings>(jsonToLoad.text);

        base.followerCount = settings.followerCount;
        base.spawnRadius = settings.spawnRadius;
        base.taperAmount = settings.taperAmount;
        base.rootThickness = settings.rootThickness;
        base.tipThickness = settings.tipThickness;

        strandCount = settings.strandCount;
        baseSegmentLength = settings.baseSegmentLength;
        segmentLengthRandomness = settings.segmentLengthRandomness;

        baseVertexCount = settings.baseVertexCount;
        vertexCountRandomness = settings.vertexCountRandomness;

        baseCurlFrequency = settings.baseCurlFrequency;
        curlFrequencyRandomness = settings.curlFrequencyRandomness;

        baseCurlDiameter = settings.baseCurlDiameter;
        curlDiameterRandomness = settings.curlDiameterRandomness;

        // Updated UI sync using FindFirstObjectByType
        HairSimSettingsUI ui = Object.FindFirstObjectByType<HairSimSettingsUI>();
        if (ui != null && ui.hairSim == this)
        {
            ui.SyncWithSim();
        }
    }

    Vector3 GetRandomPointOnSphereSurface()
    {
        SphereCollider col = sphere.GetComponent<SphereCollider>();
        float radius = col.radius * sphere.transform.lossyScale.x;
        return sphere.transform.position + Random.onUnitSphere * radius;
    }
}
