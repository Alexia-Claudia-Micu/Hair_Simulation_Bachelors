using UnityEngine;
using System.Collections.Generic;

public class HairSimFromImported : MonoBehaviour
{
    public TextAsset importedHairJson;
    public GameObject hairStrandPrefab;
    public GameObject emitter;

    private List<HairStrand> strands = new List<HairStrand>();

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
                //  No root transform handling — keep original world positions
            }
        }
    }

    void FixedUpdate()
    {
        //  No position updates — everything stays as-is
    }
}
