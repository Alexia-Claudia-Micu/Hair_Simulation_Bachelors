using UnityEngine;

public class Constants
{
    public const float HairMass = 0.1f;
    public const float HairStiffness = 100f;
    public const float HairDamping = 0.8f;
    public const float AngleStiffness = 4.5f;       // Softer spring
    public const float RotationDamping = 0.93f;     // Less stiff
    public const float CurlinessFactor = 0.05f;
    public const float BaseForceThreshold = 0.6f;
    public const float ForceThresholdLoadFactor = 0.2f;
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


    public const float AngularForceThreshold = 0.3f;

}