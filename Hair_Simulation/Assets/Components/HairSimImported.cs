using UnityEngine;
using System.Collections.Generic;

public class HairSimFromImported : HairSimCore
{
    public TextAsset importedHairJson;
    public GameObject hairStrandPrefab;
    public GameObject emitter;

    void Start()
    {
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
            }
        }

        Initialize(importedStrands, vertexCount);
    }
}