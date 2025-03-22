using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float torqueAmount = .1f;          // Smaller = slower rotation
    public float angularDamping = 5f;        // How quickly it stops spinning
    public float maxAngularSpeed = 2f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
            return;
        }

        rb.maxAngularVelocity = 100f; // Optional: ensure not clamped
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // --- Movement ---
        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.RightArrow)) moveDirection.x = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) moveDirection.x = -1;
        if (Input.GetKey(KeyCode.UpArrow)) moveDirection.y = 1;
        if (Input.GetKey(KeyCode.DownArrow)) moveDirection.y = -1;
        if (Input.GetKey(KeyCode.F)) moveDirection.z = 1;
        if (Input.GetKey(KeyCode.B)) moveDirection.z = -1;

        rb.linearVelocity = moveDirection.normalized * moveSpeed;

        // --- Rotation ---
        float rotationInput = 0;
        if (Input.GetKey(KeyCode.Comma)) rotationInput = -1;
        if (Input.GetKey(KeyCode.Period)) rotationInput = 1;

        if (rotationInput != 0)
        {
            // Only apply torque if we're under the max spin speed
            if (rb.angularVelocity.magnitude < maxAngularSpeed)
            {
                Vector3 torque = Vector3.up * rotationInput * torqueAmount;
                rb.AddTorque(torque, ForceMode.Force);
            }
        }
        else
        {
            // Smooth stop when no input
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime * angularDamping);
        }

    }
}
