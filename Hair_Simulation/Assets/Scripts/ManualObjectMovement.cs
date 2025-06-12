using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode forwardKey;
    public KeyCode backwardKey;
    public KeyCode spinRightKey;
    public KeyCode spinLeftKey;

    public float moveSpeed = 2f;
    public float torqueAmount = .1f;         
    public float angularDamping = 5f;      
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

        rb.maxAngularVelocity = 100f; 
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(rightKey)) moveDirection.x = 1;
        if (Input.GetKey(leftKey)) moveDirection.x = -1;
        if (Input.GetKey(upKey)) moveDirection.y = 1;
        if (Input.GetKey(downKey)) moveDirection.y = -1;
        if (Input.GetKey(forwardKey)) moveDirection.z = 1;
        if (Input.GetKey(backwardKey)) moveDirection.z = -1;

        rb.linearVelocity = moveDirection.normalized * moveSpeed;

        float rotationInput = 0;
        if (Input.GetKey(spinLeftKey)) rotationInput = -1;
        if (Input.GetKey(spinRightKey)) rotationInput = 1;

        if (rotationInput != 0)
        {
            if (rb.angularVelocity.magnitude < maxAngularSpeed)
            {
                Vector3 torque = Vector3.up * rotationInput * torqueAmount;
                rb.AddTorque(torque, ForceMode.Force);
            }
        }
        else
        {
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime * angularDamping);
        }

    }
}
