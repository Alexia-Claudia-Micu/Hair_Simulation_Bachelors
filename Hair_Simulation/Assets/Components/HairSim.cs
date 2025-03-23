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
                strands[i].UpdateRootPosition(newWorldRoot);
            }
        }
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
}
