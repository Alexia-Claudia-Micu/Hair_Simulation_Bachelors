using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of movement
    public float distance = 5f;  // Distance to move back and forth

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingToTarget = true;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
            return;
        }

        startPosition = transform.position;
        targetPosition = startPosition + Vector3.right * distance; // Move along x-axis
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 direction = movingToTarget ? Vector3.right : Vector3.left;
        rb.linearVelocity = direction * moveSpeed;

        if (movingToTarget && Vector3.Distance(transform.position, targetPosition) < 0.1f)
            movingToTarget = false;
        else if (!movingToTarget && Vector3.Distance(transform.position, startPosition) < 0.1f)
            movingToTarget = true;
    }
}
