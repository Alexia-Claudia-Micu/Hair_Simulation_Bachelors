using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of movement
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
    }
}
