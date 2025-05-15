using UnityEngine;
using System.Collections.Generic;

public class HairSimFromImported : MonoBehaviour
{
    public TextAsset importedHairJson;
    public GameObject hairStrandPrefab;
    public GameObject emitter;

    private List<HairStrand> strands = new List<HairStrand>();
    private List<Vector3> localRootPositions = new List<Vector3>();
    private List<Vector3> localRootNormals = new List<Vector3>();

    void Start()
    {
        if (importedHairJson == null || hairStrandPrefab == null || emitter == null)
        {
            Debug.LogError("Assign all required references in inspector.");
            return;
        }

        HairStrandData[] strandData = JsonHelper.FromJson<HairStrandData>(importedHairJson.text);

        foreach (var strand in strandData)
        {
            List<Vector3> points = new List<Vector3>();
            foreach (var v in strand.vertices)
                points.Add(v.ToVector3());

            if (points.Count < 2) continue;

            GameObject strandObj = Instantiate(hairStrandPrefab);
            HairStrand strandComponent = strandObj.GetComponent<HairStrand>();

            if (strandComponent != null)
            {
                strandComponent.emitter = emitter;
                strandComponent.InitializeHairStrandFromVertices(points);
                strands.Add(strandComponent);

                // Store local root position and direction
                Vector3 worldRoot = points[0];
                Vector3 direction = (points[1] - points[0]).normalized;

                Vector3 localRoot = emitter.transform.InverseTransformPoint(worldRoot);
                Vector3 localNormal = emitter.transform.InverseTransformDirection(direction);

                localRootPositions.Add(localRoot);
                localRootNormals.Add(localNormal);
            }
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < strands.Count; i++)
        {
            if (strands[i] != null)
            {
                Vector3 newWorldRoot = emitter.transform.TransformPoint(localRootPositions[i]);
                Vector3 newWorldNormal = emitter.transform.TransformDirection(localRootNormals[i]);
                strands[i].UpdateRoot(newWorldRoot, newWorldNormal);
            }
        }
    }
}
