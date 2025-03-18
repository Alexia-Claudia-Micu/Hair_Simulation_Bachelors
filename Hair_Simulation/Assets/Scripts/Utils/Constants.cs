using UnityEngine;

public class Constants
{
    // **GRAVITY & GENERAL PHYSICS**
    public static readonly Vector3 Gravity = new Vector3(0, -9.81f, 0);

    // **HAIR PHYSICAL PROPERTIES**
    public static readonly float HairThickness = 0.05f;
    public static readonly float HairDensity = 0.5f; // Determines mass per unit length

    // **STIFFNESS & DAMPING PARAMETERS**

    public static readonly float HairBendingStiffness = 1.5f; // Resistance to bending
    public static readonly float BendingDamping = 1f; // Damping for bending motion
    public static float AngleStiffness = 0.2f;  // NEW: Controls how much the hair resists angle deformation


    // **VOLUME & ENVIRONMENTAL INTERACTIONS**
    public static readonly float WindInfluence = 0.1f; // Controls how much wind affects hair
    public static readonly float WindStrength = 2.0f; // Wind force applied
    public static readonly float AirFriction = 0.02f; // Air resistance

    // **LOD SETTINGS**
    public static readonly float LODDistanceThreshold = 10f; // Distance at which LOD applies
    public static readonly int LODStrandReduction = 2; // Reduction factor for performance

    // **SOLVER SETTINGS**
    public static readonly int SolverIterations = 10; // How many physics iterations per frame
    public static readonly int SolverSubsteps = 4; // Number of substeps per frame

    public static readonly float MinHairTaper = 0.2f; // Ensures minimum taper width

    public static float HairMass = 0.1f;
    public static float HairStiffness = 0.5f;
    public static float HairDamping = 0.05f;

    // New rotational constants
    public static float TorqueFactor = 0.1f;
    public static float RotationDamping = 0.9f;
    public static float CurlinessFactor = 0.05f;

    public static float ForceThreshold = 0.92f;  // Adjust this to control stability
    public static float PositionDamping = 0.9f; // Reduces linear displacement when curled


}
