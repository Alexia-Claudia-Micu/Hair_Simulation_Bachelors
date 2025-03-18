//using UnityEngine;
//using System.Collections.Generic;

//public class HairGenerator : MonoBehaviour
//{
//    public int NumStrands = 100000; // 100K hairs
//    public int NumSegmentsPerStrand = 10;
//    public float SegmentLength = 0.01f;
//    public float Mass = 0.02f;
//    public float Stiffness = 50f;
//    public float Dampness = 0.9f;
//    public Vector3 RootPosition = new Vector3(0, 2, 0);
//    public float Spread = 0.1f; // Spread of hair on the scalp

//    public List<HairStrand> Strands = new List<HairStrand>();
//    private HairMesh hairMesh;

//    void Start()
//    {
//        hairMesh = GetComponent<HairMesh>();
//        GenerateStrands();
//        hairMesh.SetHairStrands(Strands);
//    }

//    void GenerateStrands()
//    {
//        for (int i = 0; i < NumStrands; i++)
//        {
//            Vector3 strandRootPosition = RootPosition + new Vector3(
//                Random.Range(-Spread, Spread),
//                0,
//                Random.Range(-Spread, Spread)
//            );

//            HairStrand strand = new HairStrand(NumSegmentsPerStrand, SegmentLength, Mass, Stiffness, Dampness, strandRootPosition);
//            Strands.Add(strand);
//        }
//    }

//    void FixedUpdate()
//    {
//        // Run physics updates efficiently in batches
//        foreach (HairStrand strand in Strands)
//        {
//            strand.UpdateStrand();
//        }

//        hairMesh.UpdateMesh();
//    }
//}
