using System.Collections.Generic;
using UnityEngine;

// TODO: the strand is not initialized with all the correct parameters (defaults)
public class HairSim : MonoBehaviour
{
    public GameObject hairStrandPrefab;
    public GameObject sphere; // The moving object (emitter)
    public int strandCount = 20;
    public float minSegmentLength = 0.4f;
    public float maxSegmentLength = 0.7f;
    public int minVertices = 3;
    public int maxVertices = 5;
    public float minCurliness = 0.6f;
    public float maxCurliness = 1.0f;

    private List<HairStrand> strands = new List<HairStrand>();
    private List<Vector3> localRootPositions = new List<Vector3>(); // Stores initial root offsets

    void Start()
    {
        if (sphere == null)
        {
            Debug.LogError("Sphere object (emitter) is not assigned to HairCluster.");
            return;
        }

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
            // Get a random position on the sphere's surface
            Vector3 rootPosition = GetRandomPointOnSphereSurface();
            Vector3 localRootPosition = sphere.transform.InverseTransformPoint(rootPosition);

            float segmentLength = Random.Range(minSegmentLength, maxSegmentLength);
            int numberOfVertices = Random.Range(minVertices, maxVertices);
            float curlinessFactor = Random.Range(minCurliness, maxCurliness);

            // Instantiate hair strand
            GameObject strandObject = Instantiate(hairStrandPrefab, Vector3.zero, Quaternion.identity);
            HairStrand hairStrand = strandObject.GetComponent<HairStrand>();

            if (hairStrand != null)
            {
                hairStrand.emitter = sphere; // Attach emitter reference
                hairStrand.InitializeHairStrand(rootPosition, segmentLength, numberOfVertices, curlinessFactor);
                strands.Add(hairStrand);
                localRootPositions.Add(localRootPosition); // Store relative position
            }
        }
    }

    void FixedUpdate()
    {
        // Update each strand's root position based on the emitter's movement
        for (int i = 0; i < strands.Count; i++)
        {
            if (strands[i] != null)
            {
                Vector3 newWorldRoot = sphere.transform.TransformPoint(localRootPositions[i]);
                strands[i].UpdateRootPosition(newWorldRoot);
            }
        }
    }

    /// <summary>
    /// Generates a random point on the surface of the sphere.
    /// </summary>
    Vector3 GetRandomPointOnSphereSurface()
    {
        SphereCollider sphereCollider = sphere.GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            Debug.LogError("Sphere does not have a SphereCollider.");
            return sphere.transform.position;
        }

        float sphereRadius = sphereCollider.radius * sphere.transform.lossyScale.x;

        // Generate a random point on a unit sphere
        Vector3 randomDirection = Random.onUnitSphere;

        // Convert to world space by scaling with the sphere's radius and position
        return sphere.transform.position + randomDirection * sphereRadius;
    }

}
