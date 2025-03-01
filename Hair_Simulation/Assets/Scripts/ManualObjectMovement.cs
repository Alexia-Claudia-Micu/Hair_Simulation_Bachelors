using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of movement
    public float rotationSpeed = 100f; // Speed of rotation
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
            return;
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Get movement input
        float moveX = 0; // Left/Right
        float moveY = 0; // Up/Down
        float moveZ = 0; // Forward/Backward

        if (Input.GetKey(KeyCode.RightArrow)) moveX = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) moveX = -1;
        if (Input.GetKey(KeyCode.UpArrow)) moveY = 1; // Move Up
        if (Input.GetKey(KeyCode.DownArrow)) moveY = -1; // Move Down
        if (Input.GetKey(KeyCode.F)) moveZ = 1; // Move Forward
        if (Input.GetKey(KeyCode.B)) moveZ = -1; // Move Backward

        // Apply movement
        Vector3 moveDirection = new Vector3(moveX, moveY, moveZ).normalized;
        rb.linearVelocity = moveDirection * moveSpeed;

        // Get rotation input
        float rotationY = 0;
        if (Input.GetKey(KeyCode.Comma)) rotationY = -1; // Rotate Left ("<" key)
        if (Input.GetKey(KeyCode.Period)) rotationY = 1;  // Rotate Right (">" key)

        // Apply rotation
        if (rotationY != 0)
        {
            Quaternion deltaRotation = Quaternion.Euler(0, rotationY * rotationSpeed * Time.fixedDeltaTime, 0);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }
}
