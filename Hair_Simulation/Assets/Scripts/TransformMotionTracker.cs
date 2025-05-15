using UnityEngine;

public class TransformMotionTracker : MonoBehaviour
{
    public Vector3 LinearVelocity { get; private set; }
    public Vector3 AngularVelocity { get; private set; }

    private Vector3 previousPosition;
    private Quaternion previousRotation;

    void Start()
    {
        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }

    void LateUpdate()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime <= 0f) return;

        LinearVelocity = (transform.position - previousPosition) / deltaTime;

        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(previousRotation);
        deltaRotation.ToAngleAxis(out float angleInDegrees, out Vector3 axis);
        if (angleInDegrees > 180f) angleInDegrees -= 360f;

        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        AngularVelocity = (axis * angleInRadians) / deltaTime;

        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }
}
