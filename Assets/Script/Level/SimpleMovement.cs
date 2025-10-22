using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(moveX, 0f, moveZ);
        dir = dir.normalized * moveSpeed;
        dir.y = rb.linearVelocity.y; // Keep current vertical velocity (gravity, jumping, etc.)

        rb.linearVelocity = dir;
    }
}
