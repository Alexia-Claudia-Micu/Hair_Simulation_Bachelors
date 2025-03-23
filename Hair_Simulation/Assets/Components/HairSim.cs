using System.Collections.Generic;
using UnityEngine;

public class HairSim : MonoBehaviour
{
    public GameObject sphere;
    public GameObject hairStrandPrefab;
    public int strandCount = 20;
    public float minSegmentLength = 0.4f;
    public float maxSegmentLength = 0.7f;
    public int minVertices = 3;
    public int maxVertices = 5;
    public float minCurliness = 0.6f;
    public float maxCurliness = 1.0f;

    public List<HairStrand> Strands { get; private set; } = new();
    private List<Vector3> localRootPositions = new();

    void Start()
    {
        GenerateHairCluster();
    }

    void FixedUpdate()
    {
        for (int i = 0; i < Strands.Count; i++)
        {
            if (Strands[i] != null)
            {
                Vector3 newWorldRoot = sphere.transform.TransformPoint(localRootPositions[i]);
                Strands[i].UpdateRootPosition(newWorldRoot);
            }
        }
    }

    void GenerateHairCluster()
    {
        if (sphere == null || hairStrandPrefab == null) return;

        for (int i = 0; i < strandCount; i++)
        {
            Vector3 rootPosition = GetRandomPointOnSphereSurface();
            Vector3 localRootPosition = sphere.transform.InverseTransformPoint(rootPosition);

            float segmentLength = Random.Range(minSegmentLength, maxSegmentLength);
            int numberOfVertices = Random.Range(minVertices, maxVertices);
            float curlinessFactor = Random.Range(minCurliness, maxCurliness);

            GameObject strandObject = Instantiate(hairStrandPrefab, Vector3.zero, Quaternion.identity);
            HairStrand strand = strandObject.GetComponent<HairStrand>();
            strand.emitter = sphere;
            strand.InitializeHairStrand(rootPosition, segmentLength, numberOfVertices, curlinessFactor);

            Strands.Add(strand);
            localRootPositions.Add(localRootPosition);
        }
    }

    Vector3 GetRandomPointOnSphereSurface()
    {
        SphereCollider sphereCollider = sphere.GetComponent<SphereCollider>();
        float radius = sphereCollider.radius * sphere.transform.lossyScale.x;
        return sphere.transform.position + Random.onUnitSphere * radius;
    }
}
