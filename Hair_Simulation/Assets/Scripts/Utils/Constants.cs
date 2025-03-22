using UnityEngine;

public class Constants
{
    public const float HairMass = 0.1f;
    public const float HairStiffness = 100f;
    public const float HairDamping = 0.8f;
    public const float AngleStiffness = 2.5f;       // Softer spring
    public const float RotationDamping = 0.93f;     // Less stiff
    public const float CurlinessFactor = 0.05f;
    public const float ForceThreshold = 0.6f;
    public const float TorqueFactor = 0.01f;
    public const float PositionDamping = 0.98f;
    public const float RootAngleFollowSpeed = 12f; // Speed for root angle to follow surface normal (tweakable)

    public const float CollisionForceMultiplier = 100f;  // Higher = stronger reaction
    public const float CollisionFriction = 5f;         // Helps damp after impact
    public const float MaxCollisionImpulse = 100f;          // Clamp to avoid big spikes


    // **GRAVITY & GENERAL PHYSICS**
    public static readonly Vector3 Gravity = new Vector3(0, -9.81f, 0);

    // **HAIR PHYSICAL PROPERTIES**
    public static readonly float HairThickness = 0.05f;
    public static readonly float HairDensity = 0.5f; // Determines mass per unit length

    // **STIFFNESS & DAMPING PARAMETERS**

    public static readonly float HairBendingStiffness = 1.5f; // Resistance to bending
    public static readonly float BendingDamping = 1f; // Damping for bending motion


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


    public const float AngularForceThreshold = 0.3f;


}
