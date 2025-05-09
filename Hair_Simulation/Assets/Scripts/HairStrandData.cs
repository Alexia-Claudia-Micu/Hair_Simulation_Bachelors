using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class HairStrandData
{
    public int strand_id;
    public List<Vector3Data> vertices;
}

[System.Serializable]
public class Vector3Data
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public class HairStrandList
{
    public List<HairStrandData> strands;
}
