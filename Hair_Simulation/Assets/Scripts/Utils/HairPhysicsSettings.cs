using UnityEngine;

[CreateAssetMenu(fileName = "HairPhysicsSettings", menuName = "Hair/Physics Settings")]
public class HairPhysicsSettings : ScriptableObject
{
    [Header("Hair Physics")]
    public float HairMass = 0.1f;
    public float HairStiffness = 100f;
    public float HairDamping = 0.8f;

    [Header("Angle & Curl")]
    public float AngleStiffness = 4.5f;
    public float RotationDamping = 0.93f;
    public float CurlinessFactor = 0.001f;

    [Header("Forces")]
    public float BaseForceThreshold = 0.6f;
    public float ForceThresholdLoadFactor = 0.2f;
    public float TorqueFactor = 0.01f;
    public float PositionDamping = 0.98f;
    public float RootAngleFollowSpeed = 12f;
    public float AngularForceThreshold = 0.3f;

    [Header("Collision")]
    public float CollisionForceMultiplier = 100f;
    public float CollisionFriction = 5f;
    public float MaxCollisionImpulse = 100f;

    [Header("Environment")]
    public Vector3 Gravity = new Vector3(0, -9.81f, 0);
}
