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
    [Range(0f, 1f)]
    public float segmentLengthRandomness = 0.2f;

    [Header("Vertex Count")]
    public int baseVertexCount = 4;
    [Range(0f, 1f)]
    public float vertexCountRandomness = 0.25f;

    [Header("Curl Frequency")]
    public float baseCurlFrequency = 0.6f;
    [Range(0f, 1f)]
    public float curlFrequencyRandomness = 0.5f;

    [Header("Curl Diameter")]
    public float baseCurlDiameter = 0.02f;
    [Range(0f, 1f)]
    public float curlDiameterRandomness = 0.5f;

    private List<HairStrand> strands = new List<HairStrand>();
    private List<Vector3> localRootPositions = new List<Vector3>();
    private List<Vector3> localRootNormals = new List<Vector3>(); // NEW

    void Start()
    {
        if (sphere == null)
        {
            Debug.LogError("Sphere object (emitter) is not assigned to HairSim.");
            return;
        }

        GenerateHairCluster();
    }

    void GenerateHairCluster()
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
            numberOfVertices = Mathf.Max(2, numberOfVertices); // ensure at least 2

            float curlFrequency = baseCurlFrequency * Random.Range(1f - curlFrequencyRandomness, 1f + curlFrequencyRandomness);
            float curlDiameter = baseCurlDiameter * Random.Range(1f - curlDiameterRandomness, 1f + curlDiameterRandomness);

            GameObject strandObject = Instantiate(hairStrandPrefab, Vector3.zero, Quaternion.identity);
            HairStrand hairStrand = strandObject.GetComponent<HairStrand>();

            if (hairStrand != null)
            {
                hairStrand.emitter = sphere;
                hairStrand.InitializeHairStrand(rootPosition, segmentLength, numberOfVertices, curlFrequency, curlDiameter);
                strands.Add(hairStrand);
                localRootPositions.Add(localRootPosition);
                localRootNormals.Add(localRootNormal); // Save local normal
            }
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < strands.Count; i++)
        {
            if (strands[i] != null)
            {
                Vector3 newWorldRoot = sphere.transform.TransformPoint(localRootPositions[i]);
                Vector3 newWorldNormal = sphere.transform.TransformDirection(localRootNormals[i]);

                strands[i].UpdateRoot(newWorldRoot, newWorldNormal);
            }
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
}
