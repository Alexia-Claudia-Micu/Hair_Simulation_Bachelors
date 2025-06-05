using UnityEngine;
using System.Collections.Generic;

public class HairSimFromImported : HairSimCore
{
    public TextAsset importedHairJson;
    public GameObject hairStrandPrefab;
    public GameObject emitter;

    private List<Vector3> localRootPositions = new();
    private List<Vector3> localRootNormals = new();



    void Start()
    {
        if (InterSceneStatics.SelectedHairJson != null)
        {
            importedHairJson = InterSceneStatics.SelectedHairJson;
        }

        if (importedHairJson == null || hairStrandPrefab == null || emitter == null)
        {
            Debug.LogError("Assign all required references in inspector.");
            return;
        }
        if (importedHairJson == null || hairStrandPrefab == null || emitter == null)
        {
            Debug.LogError("Assign all required references in inspector.");
            return;
        }

        HairStrandData[] strandData = JsonHelper.FromJson<HairStrandData>(importedHairJson.text);
        List<HairStrand> importedStrands = new();
        int vertexCount = 0;

        foreach (var strand in strandData)
        {
            List<Vector3> points = new();
            foreach (var v in strand.vertices)
                points.Add(v.ToVector3());

            if (points.Count < 2) continue;

            GameObject strandObj = Instantiate(hairStrandPrefab);
            HairStrand strandComp = strandObj.GetComponent<HairStrand>();
            if (strandComp != null)
            {
                strandComp.emitter = emitter;
                strandComp.InitializeHairStrandFromVertices(points);
                importedStrands.Add(strandComp);
                vertexCount = Mathf.Max(vertexCount, points.Count);

                // Store local-space root data for update
                Vector3 worldRoot = points[0];
                Vector3 direction = (points[1] - points[0]).normalized;

                Vector3 localRoot = emitter.transform.InverseTransformPoint(worldRoot);
                Vector3 localNormal = emitter.transform.InverseTransformDirection(direction);

                localRootPositions.Add(localRoot);
                localRootNormals.Add(localNormal);
            }
        }

        Initialize(importedStrands, vertexCount);
    }

    new void FixedUpdate()
    {
        base.FixedUpdate(); // run HairSimCore simulation logic

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
