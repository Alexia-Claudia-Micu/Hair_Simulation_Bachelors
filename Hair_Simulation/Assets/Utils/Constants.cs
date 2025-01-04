using UnityEngine;

public class Constants
{
    public static readonly Vector3 Gravity = new Vector3(0, -9.81f, 0);

    public static readonly float StraightHairStiffness = 0.99f;
    public static readonly float StraightHairDamping = 0.3f;
    public static readonly float StraightHairMass = 0.01f;
    public static readonly float StraightHairThickness = 0.05f;

    public static readonly float BendingStiffness = 1f;
    public static readonly float BendingDamping = 1f;
}
