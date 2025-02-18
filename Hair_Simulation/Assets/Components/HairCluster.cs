using System.Collections.Generic;
using UnityEngine;

public class HairCluster : MonoBehaviour
{
    public GameObject hairStrandPrefab; // Prefab containing HairStrand component
    public int strandCount = 20; // Number of strands per cluster
    public Vector3 clusterSize = new Vector3(0.2f, 0f, 0.2f); // Area in which strands are distributed

    public float minSegmentLength = 0.4f;
    public float maxSegmentLength = 0.7f;
    public int minVertices = 8;
    public int maxVertices = 12;
    public float minCurliness = 0.1f;
    public float maxCurliness = 0.5f;

    private List<HairStrand> strands = new List<HairStrand>();

    void Start()
    {
        GenerateHairCluster();
    }

    void GenerateHairCluster()
    {
        if (hairStrandPrefab == null)
        {
            Debug.LogError("HairStrand prefab is not assigned to HairCluster.");
            return;
        }

        for (int i = 0; i < strandCount; i++)
        {
            // Generate a random position within the cluster area
            Vector3 rootPosition = transform.position + new Vector3(
                Random.Range(-clusterSize.x / 2, clusterSize.x / 2),
                0, // Hair grows from the same height
                Random.Range(-clusterSize.z / 2, clusterSize.z / 2)
            );

            // Randomize strand properties
            float segmentLength = Random.Range(minSegmentLength, maxSegmentLength);
            int numberOfVertices = Random.Range(minVertices, maxVertices);
            float curlinessFactor = Random.Range(minCurliness, maxCurliness);

            // Instantiate hair strand
            GameObject strandObject = Instantiate(hairStrandPrefab, rootPosition, Quaternion.identity, transform);
            HairStrand hairStrand = strandObject.GetComponent<HairStrand>();

            if (hairStrand != null)
            {
                hairStrand.InitializeHairStrand(rootPosition, segmentLength, numberOfVertices, curlinessFactor);
                strands.Add(hairStrand);
            }
        }
    }
}
